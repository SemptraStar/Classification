using Classification.Utility.SQL;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

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

            InitializeTables();
        }

        private void SelectClassifications()
        {
            DataTable dataTable = _SQLClient.SelectClassificationsWithRootConcepts();
            List<string> classifications = new List<string>();

            foreach (DataRow row in dataTable.Rows)
            {
                classifications.Add(
                    string.Format("{0}. Тип: {1}. КП: {2}",
                        row["Id"].ToString().Trim(),
                        row["Type"].ToString().Trim(),
                        row["ConceptRoot"].ToString().Trim()
                    ));
            }

            ClassificationsComboBox.ItemsSource = null;
            ClassificationsComboBox.ItemsSource = classifications;
        }

        private void SelectDefinitions()
        {
            if (ClassificationsComboBox.SelectedItem == null)
                return;

            int classificationId = int.Parse(ClassificationsComboBox
                .SelectedItem
                .ToString()
                .Split('.')[0]);

            DataTables.DefinitiosDataTable.Clear();
            DataTables.DefinitiosDataTable = _SQLClient.SelectClassificationDefinitions(classificationId);

            DefinitionsDataGrid.ItemsSource = null;
            DefinitionsDataGrid.ItemsSource = DataTables.DefinitiosDataTable.DefaultView;
        }

        private void ClassificationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectDefinitions();
        }

        private void InitializeTables()
        {
            DataTables.DefinitiosDataTable = new System.Data.DataTable();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeTables();

            DefinitionsDataGrid.ItemsSource = DataTables.DefinitiosDataTable.DefaultView;

            SelectClassifications();
        }
    }
}
