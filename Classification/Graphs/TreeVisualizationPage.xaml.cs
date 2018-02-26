using Classification.Models;
using Classification.Models.GraphSharp;
using Classification.Utility;
using Classification.Utility.SQL;
using GraphSharp.Controls;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Classification.Graphs
{
    /// <summary>
    /// Interaction logic for TreeVisualizationPage.xaml
    /// </summary>
    public partial class TreeVisualizationPage : Page
    {
        public static TreeVisualizationPage Instance;

        public ConceptGraphViewModel _conceptGraphViewModel;

        private SQLClient _SQLClient;

        private int _selectedClassificationId = -1;
        private Dictionary<int, int> _properties;
        private Dictionary<int, Definition> _definitions;
        private List<ClassificationConcept> _classificationConcepts;

        private bool _isChangingParentConceptActive = false;
        private int _selectedForChangingParentConceptId;
        private int _selectedConceptId = -1;

        public TreeVisualizationPage()
        {
            _conceptGraphViewModel = new ConceptGraphViewModel();
            DataContext = _conceptGraphViewModel;
            InitializeComponent();

            Instance = this;
        }

        public TreeVisualizationPage(SQLClient sqlClient) : this()
        {
            _SQLClient = sqlClient;         
        }

        public void SelectClassifications()
        {
            DataTable dataTable = _SQLClient.SelectClassificationsWithRootConcepts();
            List<string> classifications = new List<string>();

            foreach (DataRow row in dataTable.Rows)
            {
                classifications.Add(
                    string.Format("{0}. Тип: {1}. КП: {2}",
                        row["Id"].ToString().Trim(),
                        row["Type"].ToString().Trim(),
                        row["ConceptRoot"].ToString().Trim()
                    ));
            }

            ClassificationsComboBox.ItemsSource = classifications;
        }

        public void SelectConceptProperties(int conceptId)
        {
            DataTables.PropertiesTreeViewDataTable = _SQLClient.SelectConceptProperties(conceptId);
            _properties = new Dictionary<int, int>();

            int i = 0;
            foreach (DataRow row in DataTables.PropertiesTreeViewDataTable.Rows)
            {
                _properties.Add(int.Parse(row["IdProperty"].ToString()), i);
                i++;
            }

            PropertiesDataGrid.ItemsSource = DataTables.PropertiesTreeViewDataTable.DefaultView;
        }

        public void SelectConceptDefinitions(int classConceptId)
        {
            DataTable conceptDefinitonsDataTable = _SQLClient.SelectClassConceptDefinitions(classConceptId);
            _definitions = new Dictionary<int, Definition>();

            int i = 0;
            foreach (DataRow row in conceptDefinitonsDataTable.Rows)
            {
                _definitions.Add(i, Definition.CreateDefinition(row));
                i++;
            }

            DefinitionsDataGrid.ItemsSource = conceptDefinitonsDataTable.DefaultView;
        }

        public void GenerateGraph()
        {
            _classificationConcepts =
                ClassificationConcept.CreateClassificationConcepts(
                    _SQLClient.SelectClassificationConcepts(_selectedClassificationId)
                    );

            _conceptGraphViewModel.GenerateGraph(_classificationConcepts);

            GenerateVertexEvents();
        }

        private void ChangeConceptParent(int newParentId)
        {
            _isChangingParentConceptActive = false;

            _SQLClient.UpdateClassificationConceptParent(
                _selectedClassificationId,
                _selectedForChangingParentConceptId,
                newParentId
                );

            GenerateGraph();
            GenerateVertexEvents();
        }

        private void ClearConceptLabels()
        {
            ConceptLabel.Text = "";
            SpecDifferenceTextLabel.Text = "";
            ConceptSpecDifferenceLabel.Text = "";
        }                

        private void GenerateVertexEvents()
        {
            foreach (var vertex in graphLayout.Children)
            {
                if (vertex is VertexControl)
                {
                    var vertexControl = (vertex as VertexControl);
                    vertexControl.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Vertex_MouseClick);
                    vertexControl.ContextMenu = CreateVertexContextMenu();
                }
            }
        }

        private ContextMenu CreateVertexContextMenu()
        {
            var contextMenu = new ContextMenu();

            var addConceptItem = new MenuItem
            {
                Header = "Добавить дочернее понятие"
            };

            addConceptItem.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(AddConcept_MouseClick);

            var changeConceptParentItem = new MenuItem
            {
                Header = "Изменить родительское понятие"
            };

            changeConceptParentItem.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ChangeConceptParent_MouseClick);

            contextMenu.Items.Add(addConceptItem);
            contextMenu.Items.Add(changeConceptParentItem);

            return contextMenu;
        }

        private void AddProperty_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedForChangingParentConceptId >= 0)
            {
                var addPropertyToConceptWindow =
                new Windows.AddPropertyToConceptWindow(
                                _SQLClient,
                                _selectedConceptId,
                                this);

                addPropertyToConceptWindow.Show();
            }
        }

        private void DeleteProperty_Click(object sender, RoutedEventArgs e)
        {
            if (PropertiesDataGrid.SelectedItems == null)
            {
                MessageBox.Show("Не было выбрано ни одного свойства", "Ошибка");
                return;
            }

            var result = MessageBox.Show(
                "Вы действительно хотите удалить ({PropertiesDataGrid.SelectedItems.Count}) свойств?",
                "Внимание",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel || result == MessageBoxResult.No)
            {
                e.Handled = true;
                return;
            }

            foreach (var item in PropertiesDataGrid.SelectedItems)
            {
                int propertyId = _properties
                    .First(p => p.Value == PropertiesDataGrid.Items.IndexOf(item))
                    .Key;

                _SQLClient.DeletePropertyFromConcepts(_selectedConceptId, propertyId);
            }

            SelectConceptProperties(_selectedConceptId);
        }

        private void PropertiesDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            int propertyId = _properties
                    .First(p => p.Value == e.Row.GetIndex())
                    .Key;
            int value = int.Parse(
                (e.Row.Item as DataRowView).Row[3].ToString()
                );

            _SQLClient.UpdateConceptPropertyValue(
                _selectedConceptId, propertyId, value
                );
        }

        private void AddConcept_MouseClick(object sender, MouseButtonEventArgs e)
        {
            var concept = ((((
                sender as MenuItem)
                .Parent as ContextMenu)
                .PlacementTarget as VertexControl)
                .Vertex as ConceptVertex)
                .Concept;

            var addClassificaitonToConceptWindow =
                new Windows.AddClassificationToConceptWindow(
                    _SQLClient, _selectedClassificationId, concept.Id)
                {
                    Sender = this
                };

            addClassificaitonToConceptWindow.Show();
        }

        private void ChangeConceptParent_MouseClick(object sender, MouseButtonEventArgs e)
        {
            _isChangingParentConceptActive = true;
            _selectedForChangingParentConceptId = ((((
                sender as MenuItem)
               .Parent as ContextMenu)
               .PlacementTarget as VertexControl)
               .Vertex as ConceptVertex)
               .Concept.Id;
        }

        private void Vertex_MouseClick(object sender, MouseButtonEventArgs e)
        {
            var concept = ((sender as VertexControl).Vertex as ConceptVertex).Concept;

            _selectedConceptId = concept.Id;

            ConceptLabel.Text = concept.Name;

            ConceptSpecDifferenceLabel.Text = concept.SpeciesDifference;

            if (concept.SpeciesDifference != null)
                SpecDifferenceTextLabel.Text = "Видовое отличие:";
            else
                SpecDifferenceTextLabel.Text = "";

            if (_isChangingParentConceptActive)
                ChangeConceptParent(concept.Id);

            SelectConceptProperties(concept.Id);

            SelectConceptDefinitions(_SQLClient.FindClassConcept(_selectedClassificationId, concept.Id).Field<int>("Id"));
        }

        private void ClassificationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearConceptLabels();
            _selectedClassificationId = int.Parse(ClassificationsComboBox.SelectedItem.ToString().Split('.')[0]);

            GenerateGraph();
            GenerateVertexEvents();
        }

        private void AddDefinition_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedClassificationId != -1 && _selectedConceptId != -1)
            {
                var addDefinitionWindow = new Windows.AddDefinitionWindow(_SQLClient, _selectedClassificationId, _selectedConceptId);

                addDefinitionWindow.Show();
            }          
        }

        private void DeleteDefinition_Click(object sender, RoutedEventArgs e)
        {
            if (DefinitionsDataGrid.IsSingleRowSelected())
            {
                Definition selectedDefinition = _definitions[DefinitionsDataGrid.SelectedIndex];

                _SQLClient.DeleteDefinition(selectedDefinition.ClassConceptId, selectedDefinition.SourceId);

                SelectConceptDefinitions(selectedDefinition.ClassConceptId);
            }
        }

        private void root_Loaded(object sender, RoutedEventArgs e)
        {
            SelectClassifications();
        }
    }
}
