using Classification.Utility.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace Classification.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddClassificatonWindow.xaml
    /// </summary>
    public partial class ChangePropertyWindow : Window
    {
        private readonly SQLClient _SQLClient;
        private int _PropertyId;

        public ChangePropertyWindow()
        {
            InitializeComponent();
        }
        public ChangePropertyWindow(SQLClient client) : this()
        {
            _SQLClient = client;

            SelectConcepts();
        }
        public ChangePropertyWindow(SQLClient client, int propertyId) : this(client)
        {
            _PropertyId = propertyId;

            FindProperty();
        }

        private void FindProperty()
        {
            var property = _SQLClient.FindProperty(_PropertyId);

            PropertyNameTextBox.Text = (string)property["Name"];
            PropertyMeasureTextBox.Text = (string)property["Measure"];

            ConceptsRootComboBox.SelectedIndex = ConceptsRootComboBox
                .Items.OfType<string>().ToList()
                .FindIndex(item =>
                int.Parse(item.Split('.').First()) == (int)property["IdConcept"]);
        }
        private void SelectConcepts()
        {
            DataTable dataTable = _SQLClient.SelectConcepts();
            List<string> concepts = new List<string>();

            foreach(DataRow row in dataTable.Rows)
            {
                concepts.Add(
                    row["Id"].ToString() + ". " + 
                    row["Name"].ToString().Trim()
                    );
            }

            ConceptsRootComboBox.ItemsSource = concepts;
        }

        private void ChangePropertyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable tempDT = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter();

                _SQLClient.UpdateProperty(
                        _PropertyId,
                        int.Parse(ConceptsRootComboBox.SelectedValue.ToString().Split('.')[0]),
                        PropertyNameTextBox.Text,
                        PropertyMeasureTextBox.Text
                    );

                Frames.Properties.Instance.RefreshDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }
    }
}
