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

namespace Classification.Frames
{
    /// <summary>
    /// Логика взаимодействия для Sources.xaml
    /// </summary>
    public partial class ClassificationsToConcept : Page
    {
        public static ClassificationsToConcept Instance;

        private readonly SQLClient _SQLClient;

        private SqlDataAdapter _MainViewClassificationsToConceptsAdapter;
        private SqlDataAdapter _QueryClassificationsToConceptsAdapter;
      
        private const string DEFAULT_SQL =
            "SELECT" +
                " (SELECT TOP 1" +
                    " 'ID: ' +" +
                    " REPLACE(Classification.IdClassification, ' ', '') +" +
                    " '. Base: ' +" +
                    " REPLACE(Classification.Base, ' ', '') +" +
                    " '; Type: ' + REPLACE(Classification.Type, ' ', '')" +
                " FROM Classification" +
                " WHERE Classification.IdClassification = ctc.IdClassification) as Classification," +
                " (SELECT TOP 1 Concept.Name" +
                " FROM Concept" +
                " WHERE Concept.IdConcept = ctc.IdConcept)  as Concept," +
                " (SELECT TOP 1 Concept.Name" +
                " FROM Concept" +
                " WHERE Concept.IdConcept = ctc.IdConceptParent) as ParentConcept," +
                " SpecDifference as SpeciesDifference," +
                " Level " +
            "FROM ClassificationToConcept ctc;";


        public ClassificationsToConcept()
        {
            InitializeComponent();

            Instance = this;
        }

        public ClassificationsToConcept(SQLClient client) : this()
        {
            _SQLClient = client;

            DataTables.ClassificationsToConceptsDataTable = new System.Data.DataTable();
            RefreshDataGrid();
            ClassificationsToConceptsDataGrid.ItemsSource = DataTables.ClassificationsToConceptsDataTable?.DefaultView;

            DataTables.ClassificationsToConceptsQueryDataTable = new System.Data.DataTable();
            ClassificationsToConceptsQueryTable.ItemsSource = DataTables.ClassificationsToConceptsQueryDataTable?.DefaultView;
        }

        private void SaveConcepts_Click(object sender, RoutedEventArgs e)
        {
            SaveChangesToDB();
        }

        public void SaveChangesToDB()
        {
            _SQLClient.Update(DataTables.ClassificationsDataTable, _MainViewClassificationsToConceptsAdapter);
        }

        public void RefreshDataGrid()
        {
            DataTables.ClassificationsToConceptsDataTable.Clear();
            _SQLClient.Select(DataTables.ClassificationsToConceptsDataTable, ref _MainViewClassificationsToConceptsAdapter, DEFAULT_SQL);
        }

        private void ExecQueryButton_Click(object sender, RoutedEventArgs e)
        {
            DataTables.ClassificationsToConceptsQueryDataTable?.Clear();

            string sql_query = SelectTextBox.Text;
            sql_query = sql_query.Replace("\n", "").Replace("\t", "").Trim();
            
            try
            {
                _SQLClient.Select(DataTables.ClassificationsToConceptsQueryDataTable, ref _QueryClassificationsToConceptsAdapter, sql_query);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddClassificationToConceptButton_Click(object sender, RoutedEventArgs e)
        {
            var addClassificationToConceptWindow = new Windows.AddClassificationToConceptWindow(_SQLClient);
            addClassificationToConceptWindow.Show();
        }
    }
}
