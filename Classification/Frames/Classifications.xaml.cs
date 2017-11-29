using System;
using System.Collections;
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

        private SqlDataAdapter _MainViewClassificationsAdapter;

        private const string DEFAULT_CLASSIFICATION_SQL = 
            "SELECT Classification.IdClassification as Id, Classification.Type, Concept.Name as ConceptRoot, Classification.Base " +
            "FROM Classification " +
            "INNER JOIN Concept ON Classification.IdConceptRoot = Concept.IdConcept;";


        public Classifications()
        {
            InitializeComponent();

            Instance = this;
        }

        public Classifications(SQLClient client) : this()
        {
            _SQLClient = client;

            DataTables.ClassificationsDataTable = new System.Data.DataTable();
            SelectClassifications();

            DataTables.ClassificationConceptsDataTable = new System.Data.DataTable();
        }

        private void SelectClassifications()
        {
            DataTables.ClassificationsDataTable = (DataTable)_SQLClient
                .ExecuteProcdureScalar("SelectClassificationsWithRootConcepts ", null, null);

            ClassificationsDataGrid.ItemsSource = DataTables.ClassificationsDataTable?.DefaultView;
        }

        private void SelectClassificationConcepts(int classificationId)
        {
            DataTables.ClassificationConceptsDataTable = (DataTable)_SQLClient.ExecuteProcdureScalar(
                "SelectConceptsOfClassification",
                new string[] { "@ClassificationId" },
                new object[] { classificationId }
                );

            ConceptsDataGrid.ItemsSource = DataTables.ClassificationConceptsDataTable?.DefaultView;
        }

        private void SaveConcepts_Click(object sender, RoutedEventArgs e)
        {
            SaveChangesToDB();
        }

        public void SaveChangesToDB()
        {
            _SQLClient.Update(DataTables.ClassificationsDataTable, _MainViewClassificationsAdapter);
        }

        public void RefreshClassificationsDataGrid()
        {
            DataTables.ClassificationsDataTable.Clear();
            SelectClassifications();
        }

        public void ClearConceptsDataGrid()
        {
            DataTables.ClassificationConceptsDataTable.Clear();
            ConceptsDataGrid.Items.Refresh();
        }

        private void AddClassificationButton_Click(object sender, RoutedEventArgs e)
        {
            var addClassificationWindow = new Windows.AddClassificationWindow(_SQLClient);
            addClassificationWindow.Show();
        }

        private void ClassificationsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearConceptsDataGrid();

            if (ClassificationsDataGrid.SelectedItems.Count == 1)
            {
                int id = (int)((DataRowView)ClassificationsDataGrid.SelectedItem)["Id"];
                SelectClassificationConcepts(id);
            }
        }

        private void AddConceptButton_Click(object sender, RoutedEventArgs e)
        {
            var addClassificationToConceptWindow = new Windows.AddClassificationToConceptWindow(_SQLClient);
            addClassificationToConceptWindow.Show();
        }
    }
}
