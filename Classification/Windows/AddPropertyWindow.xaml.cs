using Classification.Utility.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Documents;

namespace Classification.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddClassificatonWindow.xaml
    /// </summary>
    public partial class AddPropertyWindow : Window
    {
        private readonly SQLClient _SQLClient;

        public AddPropertyWindow()
        {
            InitializeComponent();
        }

        public AddPropertyWindow(SQLClient client) : this()
        {
            _SQLClient = client;

            GetConceptsList();
        }

        private void GetConceptsList()
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

        private void AddPropertyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _SQLClient.InsertProperty(
                    PropertyNameTextBox.Text,
                    int.Parse(ConceptsRootComboBox.Text.Split('.')[0]),
                    PropertyMeasureTextBox.Text
                    );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }

            Frames.Properties.Instance.RefreshDataGrid();
        }
    }
}
