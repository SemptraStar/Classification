using System.Windows;
using System.Windows.Controls;

using Classification.Utility.SQL;
using System.Data.SqlClient;
using System.Data;

namespace Classification.Frames
{
    /// <summary>
    /// Логика взаимодействия для Sources.xaml
    /// </summary>
    public partial class Classifications : Page
    {
        public static Classifications Instance;

        private readonly SQLClient _SQLClient;

        private int _SelectedClassificationId;

        public Classifications()
        {
            InitializeComponent();

            Instance = this;
        }
        public Classifications(SQLClient client) : this()
        {
            _SQLClient = client;

            DataTables.ClassificationsDataTable = new DataTable();
            SelectClassifications();

            DataTables.ClassificationConceptsDataTable = new DataTable();
        }

        public void SelectClassifications()
        {
            DataTables.ClassificationsDataTable.Clear();
            DataTables.ClassificationsDataTable = _SQLClient
                .SelectClassificationsWithRootConcepts();

            ClassificationsDataGrid.ItemsSource = null;
            ClassificationsDataGrid.ItemsSource = DataTables.ClassificationsDataTable?.DefaultView;
        }
        public void SelectClassificationConcepts()
        {
            DataTables.ClassificationConceptsDataTable.Clear();
            DataTables.ClassificationConceptsDataTable = _SQLClient
                .SelectClassificationConcepts(_SelectedClassificationId);

            ConceptsDataGrid.ItemsSource = null;
            ConceptsDataGrid.ItemsSource = DataTables.ClassificationConceptsDataTable?.DefaultView;
        }

        public void ClearConceptsDataGrid()
        {
            DataTables.ClassificationConceptsDataTable.Clear();
            ConceptsDataGrid.Items.Refresh();
        }

        private void ClassificationsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearConceptsDataGrid();

            if (ClassificationsDataGrid.SelectedItems.Count == 1)
            {
                _SelectedClassificationId = (int)((DataRowView)ClassificationsDataGrid.SelectedItem)["Id"];
                SelectClassificationConcepts();
            }
        }

        private void AddClassificationButton_Click(object sender, RoutedEventArgs e)
        {
            var addClassificationWindow = new Windows.AddClassificationWindow(_SQLClient);
            addClassificationWindow.Show();
        }      

        private void AddConceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClassificationsDataGrid.SelectedItems.Count == 1)
            {
                int classificationID = (int)((DataRowView)ClassificationsDataGrid.SelectedItem)["Id"];

                var addClassificationToConceptWindow = new Windows.AddClassificationToConceptWindow(_SQLClient, classificationID);
                addClassificationToConceptWindow.Show();
            }
            
        }

        private void AddDefinition_Click(object sender, RoutedEventArgs e)
        {
            if (ClassificationsDataGrid.SelectedItems.Count == 1 && ConceptsDataGrid.SelectedItems.Count == 1)
            {
                int classificationID = (int)((DataRowView)ClassificationsDataGrid.SelectedItem)["Id"];
                int conceptId = (int)((DataRowView)ConceptsDataGrid.SelectedItem)["Id"];

                var addDefinitionWindow = new Windows.AddDefinitionWindow(
                    _SQLClient, classificationID, conceptId);

                addDefinitionWindow.Show();
            }
        }

        private void ChangeDefinition_Click(object sender, RoutedEventArgs e)
        {
            if (ClassificationsDataGrid.SelectedItems.Count == 1 && ConceptsDataGrid.SelectedItems.Count == 1)
            {
                int classificationID = (int)((DataRowView)ClassificationsDataGrid.SelectedItem)["Id"];
                int conceptId = (int)((DataRowView)ConceptsDataGrid.SelectedItem)["Id"];

                var changeDefinitionWindow = new Windows.ChangeDefinitionWindow(
                    _SQLClient, classificationID, conceptId,
                    (string)((DataRowView)ConceptsDataGrid.SelectedItem)["Definition"]);

                changeDefinitionWindow.Show();
            }
        }
    }
}
