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
    public partial class Concepts : Page
    {
        private readonly SQLClient _SQLClient;

        private SqlDataAdapter _MainViewConceptsAdapter;
        private SqlDataAdapter _QueryConceptsAdapter;

        public Concepts()
        {
            InitializeComponent();
        }

        public Concepts(SQLClient client) : this()
        {
            _SQLClient = client;

            DataTables.ConceptsDataTable = new System.Data.DataTable();
            _SQLClient.Select(DataTables.ConceptsDataTable, ref _MainViewConceptsAdapter, new string[] { "Concept" }, new string[] { "*" });
            ConceptsDataGrid.ItemsSource = DataTables.ConceptsDataTable?.DefaultView;

            DataTables.ConceptsQueryDataTable = new System.Data.DataTable();
            ConceptsQueryTable.ItemsSource = DataTables.ConceptsQueryDataTable?.DefaultView;
        }

        private void SaveConcepts_Click(object sender, RoutedEventArgs e)
        {
            _SQLClient.Update(DataTables.ConceptsDataTable, _MainViewConceptsAdapter);
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
                _SQLClient.Select(DataTables.ConceptsQueryDataTable, ref _QueryConceptsAdapter, sql_query);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
