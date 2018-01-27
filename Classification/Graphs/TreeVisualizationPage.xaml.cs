using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphSharp.Algorithms.Layout.Simple.Hierarchical;
using GraphSharp.Algorithms.Layout.Simple.Tree;
using Classification.Models;
using Classification.Models.GraphSharp;
using QuickGraph;
using System.ComponentModel;
using System.Data;
using Classification.Utility.SQL;
using GraphSharp.Controls;

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
        private List<Concept> _classificationConcepts;

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
            SelectClassifications();
        } 
       
        private void Vertex_MouseClick(object sender, MouseButtonEventArgs e)
        {
            var concept = ((sender as VertexControl).Vertex as ConceptVertex).Concept;

            _selectedConceptId = concept.Id;

            ConceptLabel.Text = concept.Name;

            ConceptDefinitionLabel.Text = concept.Definition;

            if (concept.Definition != null)
            {
                DefinitionTextLabel.Text = "Определение:";
            }              
            else
            {
                DefinitionTextLabel.Text = "";
            }           

            ConceptSpecDifferenceLabel.Text = concept.SpeciesDifference;

            if (concept.SpeciesDifference != null)
                SpecDifferenceTextLabel.Text = "Видовое отличие:";
            else
                SpecDifferenceTextLabel.Text = "";

            SourceLabel.Text = concept.Source;

            if (concept.Source != null)
                SourceTextLabel.Text = "Источник:";
            else
                SourceTextLabel.Text = "";

            if (_isChangingParentConceptActive)
                ChangeConceptParent(concept.Id);

            SelectConceptProperties(concept.Id);
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
            DefinitionTextLabel.Text = "";
            ConceptDefinitionLabel.Text = "";
            SpecDifferenceTextLabel.Text = "";
            ConceptSpecDifferenceLabel.Text = "";
            SourceTextLabel.Text = "";
            SourceLabel.Text = "";
        }

        private ContextMenu CreateVertexContextMenu()
        {
            var contextMenu = new ContextMenu();

            var addConceptItem = new MenuItem()
            {
                Header = "Добавить дочернее понятие"
            };

            addConceptItem.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(AddConcept_MouseClick);

            var changeConceptParentItem = new MenuItem()
            {
                Header = "Изменить родительское понятие"
            };

            contextMenu.Items.Add(addConceptItem);
            contextMenu.Items.Add(changeConceptParentItem);

            return contextMenu;
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

        private void SelectClassifications()
        {
            DataTable dataTable = _SQLClient.SelectClassifications();
            List<string> classifications = new List<string>();

            foreach (DataRow row in dataTable.Rows)
            {
                classifications.Add(row["Id"].ToString().Trim() +
                    ". Основание: " + row["Base"].ToString().Trim() +
                    "; Тип: " + row["Type"].ToString().Trim());
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

        private void ClassificationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearConceptLabels();
            _selectedClassificationId = int.Parse(ClassificationsComboBox.SelectedItem.ToString().Split('.')[0]);
           
            GenerateGraph();
            GenerateVertexEvents();
        }

        public void GenerateGraph()
        {
            _classificationConcepts =
                Concept.CreateConcepts(
                    _SQLClient.SelectClassificationConcepts(_selectedClassificationId)
                    );

            _conceptGraphViewModel.GenerateGraph(_classificationConcepts);

            GenerateVertexEvents();
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
                $"Вы действительно хотите удалить ({PropertiesDataGrid.SelectedItems.Count}) свойств?",
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
    }
}
