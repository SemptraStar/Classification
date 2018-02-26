using Classification.Utility.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Classification.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddClassificatonWindow.xaml
    /// </summary>
    public partial class AddDefinitionWindow : Window
    {
        private readonly SQLClient _SQLClient;
        private int _SelectedClassificationId;
        private int _SelectedConceptId;

        public AddDefinitionWindow()
        {
            InitializeComponent();
        }

        public AddDefinitionWindow(SQLClient client) : this()
        {
            _SQLClient = client;

            SelectClassifications();
                
            SelectSources();
        }

        public AddDefinitionWindow(SQLClient client, int selectedClassificationId, int selectedConceptId) : this(client)
        {
            _SelectedClassificationId = selectedClassificationId;
            _SelectedConceptId = selectedConceptId;

            SelectConcepts();

            ClassificationsComboBox.SelectedIndex = ClassificationsComboBox
                .Items.OfType<string>().ToList()
                .FindIndex(item => 
                int.Parse(item.Split('.').First()) == _SelectedClassificationId);

            ConceptsComboBox.SelectedIndex = ConceptsComboBox
                .Items.OfType<string>().ToList()
                .FindIndex(item =>
                int.Parse(item.Split('.').First()) == _SelectedConceptId);
        }

        private void SelectClassifications()
        {
            DataTable dataTable = _SQLClient.SelectClassifications();
            List<string> classifications = new List<string>();

            foreach (DataRow row in dataTable.Rows)
            {
                classifications.Add(
                    row["Id"].ToString().Trim() + ". Основание: " + 
                    row["Base"].ToString().Trim() + "; Тип: " + 
                    row["Type"].ToString().Trim());
            }

            ClassificationsComboBox.ItemsSource = null;
            ClassificationsComboBox.ItemsSource = classifications;
        }
        private void SelectConcepts()
        {
            DataTable dataTable = _SQLClient
                .SelectClassificationConceptsRaw(_SelectedClassificationId);

            List<string> concepts = new List<string>();

            foreach(DataRow row in dataTable.Rows)
            {
                concepts.Add(
                    row["Id"].ToString() + ". " +
                    row["Name"].ToString().Trim());
            }

            ConceptsComboBox.ItemsSource = null;
            ConceptsComboBox.ItemsSource = concepts;
        }
        private void SelectSources()
        {
            DataTable dataTable = _SQLClient.SelectSources();
            List<string> sources = new List<string>();

            foreach (DataRow row in dataTable.Rows)
            {
                sources.Add(
                    row["Id"].ToString() + ". \"" + 
                    row["Name"].ToString().Trim() + "\"Author: " + 
                    row["Author"].ToString().Trim() + ". " + 
                    row["Year"].ToString().Trim());
            }

            SourceComboBox.ItemsSource = null;
            SourceComboBox.ItemsSource = sources;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void AddDefinitionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var classConcept = _SQLClient
                    .FindClassConcept(_SelectedClassificationId, _SelectedConceptId);

                int classificationConceptId = classConcept.Field<int>("Id");

                _SQLClient.InsertDefinition(
                        classificationConceptId,
                        int.Parse(SourceComboBox.Text.Split('.')[0]),
                        DefinitionTextBox.Text,
                        int.Parse(PageTextBox.Text)
                    );

                Frames.Classifications.Instance.SelectClassificationConcepts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void ClassificationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _SelectedClassificationId = int.Parse(ClassificationsComboBox.SelectedItem.ToString().Split('.')[0]);

            SelectConcepts();
        }
    }
}
