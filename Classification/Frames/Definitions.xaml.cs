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
    public partial class Definitions : Page
    {
        private readonly SQLClient _SQLClient;

        private SqlDataAdapter _MainViewDefinitionsAdapter;
        private SqlDataAdapter _QueryDefinitionsAdapter;

        public Definitions()
        {
            InitializeComponent();
        }

        public Definitions(SQLClient client) : this()
        {
            _SQLClient = client;

            DataTables.DefinitiosDataTable = new System.Data.DataTable();         
            DefinitionsDataGrid.ItemsSource = DataTables.DefinitiosDataTable?.DefaultView;

            DataTables.DefinitionsQueryDataTable = new System.Data.DataTable();
            ConceptsQueryTable.ItemsSource = DataTables.DefinitionsQueryDataTable?.DefaultView;

            SelectClassifications();
        }

        private void SelectClassifications()
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable tempDT = new DataTable();
            _SQLClient.Select(tempDT, ref adapter, "SELECT " +
                "Classification.IdClassification as Id, Classification.Base as Base, Classification.Type as Type " +
                "FROM Classification;");

            List<string> classifications = new List<string>();

            foreach (DataRow row in tempDT.Rows)
            {
                classifications.Add(row["Id"].ToString().Trim() + ". Base: " + row["Base"].ToString().Trim() + "; Type: " + row["Type"].ToString().Trim());
            }

            ClassificationsComboBox.ItemsSource = null;
            ClassificationsComboBox.ItemsSource = classifications;
        }

        private void SelectDefinitions()
        {
            DataTables.DefinitiosDataTable = (DataTable)_SQLClient.ExecuteProcdureScalar("SelectDefinitions", 
                new string[] { "@ClassificationId" }, 
                new object[] { ClassificationsComboBox.SelectedItem.ToString().Split('.')[0] }
                );

            DefinitionsDataGrid.ItemsSource = DataTables.DefinitiosDataTable?.DefaultView;
        }

        private void SaveConcepts_Click(object sender, RoutedEventArgs e)
        {
            _SQLClient.Update(DataTables.ConceptsDataTable, _MainViewDefinitionsAdapter);
        }

        private void ExecQueryButton_Click(object sender, RoutedEventArgs e)
        {
            DataTables.ConceptsQueryDataTable?.Clear();

            string sql_query = "SELECT ";

            if (SelectTextBox.Text != "")
                sql_query += SelectTextBox.Text;

            sql_query += " FROM Concept";

            if (WhereTextBox.Text != "")
                sql_query += " WHERE " + WhereTextBox.Text;

            if (OrderByTextBox.Text != "")
                sql_query += " ORDER BY " + OrderByTextBox.Text;

            sql_query += ";";

            try
            {
                _SQLClient.Select(DataTables.ConceptsQueryDataTable, ref _QueryDefinitionsAdapter, sql_query);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ClassificationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectDefinitions();
        }
    }
}
