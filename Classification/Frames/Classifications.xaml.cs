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

        private readonly SQLClient _sqlClient;

        private int _selectedClassificationId;

        public Classifications()
        {
            InitializeComponent();

            Instance = this;
        }
        public Classifications(SQLClient client) : this()
        {
            _sqlClient = client;

            DataTables.ClassificationsDataTable = new DataTable();
            SelectClassifications();

            DataTables.ClassificationConceptsDataTable = new DataTable();
        }

        public void SelectClassifications()
        {
            DataTables.ClassificationsDataTable.Clear();
            DataTables.ClassificationsDataTable = _sqlClient
                .SelectClassificationsWithRootConcepts();

            ClassificationsDataGrid.ItemsSource = null;
            ClassificationsDataGrid.ItemsSource = DataTables.ClassificationsDataTable?.DefaultView;
        }
        public void SelectClassificationConcepts()
        {
            DataTables.ClassificationConceptsDataTable.Clear();
            DataTables.ClassificationConceptsDataTable = _sqlClient
                .SelectClassificationConcepts(_selectedClassificationId);

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
                _selectedClassificationId = (int)((DataRowView)ClassificationsDataGrid.SelectedItem)["Id"];
                SelectClassificationConcepts();
            }
        }

        private void AddClassificationButton_Click(object sender, RoutedEventArgs e)
        {
            var addClassificationWindow = new Windows.AddClassificationWindow(_sqlClient);
            addClassificationWindow.Show();
        }      

        private void AddConceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClassificationsDataGrid.SelectedItems.Count == 1)
            {
                int classificationID = (int)((DataRowView)ClassificationsDataGrid.SelectedItem)["Id"];

                var addClassificationToConceptWindow =
                    new Windows.AddClassificationToConceptWindow(_sqlClient, classificationID)
                    {
                        Sender = this
                    };

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
                    _sqlClient, classificationID, conceptId);

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
                    _sqlClient, classificationID, conceptId,
                    (string)((DataRowView)ConceptsDataGrid.SelectedItem)["Definition"]);

                changeDefinitionWindow.Show();
            }
        }

        private void DeleteClassificationButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClassificationsDataGrid.SelectedItems.Count == 1)
            {
                if (MessageBox.Show(
                    "Вы действительно желаете удалить классификацию? " +
                    "Это действие нельзя будет отменить.",
                    "Удаление классификации",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }

                int classificationId = (int)((DataRowView)ClassificationsDataGrid.SelectedItem)["Id"];

                _sqlClient.DeleteClassification(classificationId);

                SelectClassifications();
            }
        }

        private void ChangeClassificationButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClassificationsDataGrid.SelectedItems.Count == 1)
            {
                var changeClassificationWindow = new Windows.ChangeClassificationWindow(_sqlClient, _selectedClassificationId);

                changeClassificationWindow.Show();
            }
        }
    }
}
