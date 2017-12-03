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

        public Definitions()
        {
            InitializeComponent();
        }

        public Definitions(SQLClient client) : this()
        {
            _SQLClient = client;

            DataTables.DefinitiosDataTable = new System.Data.DataTable();         
            DefinitionsDataGrid.ItemsSource = DataTables.DefinitiosDataTable?.DefaultView;

            SelectClassifications();
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

            ClassificationsComboBox.ItemsSource = null;
            ClassificationsComboBox.ItemsSource = classifications;
        }

        private void SelectDefinitions()
        {
            int classificationId = int.Parse(ClassificationsComboBox
                .SelectedItem
                .ToString()
                .Split('.')[0]);

            DataTables.DefinitiosDataTable.Clear();
            DataTables.DefinitiosDataTable = _SQLClient.SelectClassificationDefinitions(classificationId);

            DefinitionsDataGrid.ItemsSource = null;
            DefinitionsDataGrid.ItemsSource = DataTables.DefinitiosDataTable?.DefaultView;
        }

        private void ClassificationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectDefinitions();
        }
    }
}
