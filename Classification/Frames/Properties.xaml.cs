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
    public partial class Properties : Page
    {
        public static Properties Instance;

        private readonly SQLClient _SQLClient;

        private SqlDataAdapter _MainViewPropertiesAdapter;
        private SqlDataAdapter _QueryPropertiesAdapter;

        public Properties()
        {
            InitializeComponent();

            Instance = this;
        }

        public Properties(SQLClient client) : this()
        {
            _SQLClient = client;

            DataTables.PropertiesDataTable = new System.Data.DataTable();
            _SQLClient.Select(DataTables.PropertiesDataTable, ref _MainViewPropertiesAdapter,
                "SELECT Property.Name, Concept.Name as ConceptRoot, Property.Measure FROM Property INNER JOIN Concept ON Property.IdConceptRoot = Concept.IdConcept;");
            PropertiesDataGrid.ItemsSource = DataTables.PropertiesDataTable?.DefaultView;

            DataTables.PropertiesQueryDataTable = new System.Data.DataTable();
            PropertiesQueryTable.ItemsSource = DataTables.PropertiesQueryDataTable?.DefaultView;
        }

        private void SaveConcepts_Click(object sender, RoutedEventArgs e)
        {
            SaveChangesToDB();
        }

        public void SaveChangesToDB()
        {
            _SQLClient.Update(DataTables.PropertiesDataTable, _MainViewPropertiesAdapter);
        }

        public void RefreshDataGrid()
        {
            DataTables.PropertiesDataTable.Clear();
            _SQLClient.Select(DataTables.PropertiesDataTable, ref _MainViewPropertiesAdapter,
                "SELECT Property.Name, Concept.Name as ConceptRoot, Property.Measure FROM Property INNER JOIN Concept ON Property.IdConceptRoot = Concept.IdConcept;");
        }

        private void ExecQueryButton_Click(object sender, RoutedEventArgs e)
        {
            DataTables.PropertiesQueryDataTable?.Clear();

            string sql_query = "SELECT ";

            if (SelectTextBox.Text != "")
                sql_query += SelectTextBox.Text;

            sql_query += " FROM Property";

            if (WhereTextBox.Text != "")
                sql_query += " WHERE " + WhereTextBox.Text;

            if (OrderByTextBox.Text != "")
                sql_query += " ORDER BY " + OrderByTextBox.Text;

            sql_query += ";";

            try
            {
                _SQLClient.Select(DataTables.PropertiesQueryDataTable, ref _QueryPropertiesAdapter, sql_query);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddProperty_Click(object sender, RoutedEventArgs e)
        {
            var addProperyWindow = new Windows.AddPropertyWindow(_SQLClient);
            addProperyWindow.Show();
        }
    }
}
