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
    public partial class Sources : Page
    {
        private readonly SQLClient _SQLClient;
        private SqlDataAdapter _MainViewSourcesAdapter;
        private SqlDataAdapter _QuerySourcesAdapter;

        public Sources()
        {
            InitializeComponent();
        }

        public Sources(SQLClient client) : this()
        {
            _SQLClient = client;

            DataTables.SourcesDataTable = new System.Data.DataTable();
            _SQLClient.Select(DataTables.SourcesDataTable, ref _MainViewSourcesAdapter, new string[] { "Source" }, new string[] { "*" });
            SourcesDataGrid.ItemsSource = DataTables.SourcesDataTable?.DefaultView;

            DataTables.SourcesQueryDataTable = new System.Data.DataTable();
            SourcesQueryTable.ItemsSource = DataTables.SourcesQueryDataTable?.DefaultView;
        }

        private void SaveSources_Click(object sender, RoutedEventArgs e)
        {
            _SQLClient.Update(DataTables.SourcesDataTable, _MainViewSourcesAdapter);
        }

        private void ExecQueryButton_Click(object sender, RoutedEventArgs e)
        {
            DataTables.SourcesQueryDataTable?.Clear();

            string sql_query = "SELECT ";

            if (SelectTextBox.Text != "")
                sql_query += SelectTextBox.Text;

            sql_query += " FROM Source";

            if (WhereTextBox.Text != "")
                sql_query += " WHERE " + WhereTextBox.Text;

            if (OrderByTextBox.Text != "")
                sql_query += " ORDER BY " + OrderByTextBox.Text;

            sql_query += ";";

            try
            {
                _SQLClient.Select(DataTables.SourcesQueryDataTable, ref _QuerySourcesAdapter, sql_query);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
