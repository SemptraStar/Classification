using Classification.Graphs;
using Classification.Utility;
using Classification.Utility.SQL;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Classification.Windows
{
    /// <summary>
    /// Interaction logic for AddPropertyToConceptWindow.xaml
    /// </summary>
    public partial class AddPropertyToConceptWindow : Window
    {
        private readonly SQLClient _SQLClient;

        private TreeVisualizationPage SenderPage;

        // Key - DB ID
        // Value - ComboBox index
        private Dictionary<int, int> _concepts;
        private Dictionary<int, int> _properties;

        public AddPropertyToConceptWindow()
        {
            InitializeComponent();
        }

        public AddPropertyToConceptWindow(SQLClient client) : this()
        {
            _SQLClient = client;

            SelectConcepts();
        }

        public AddPropertyToConceptWindow(SQLClient client, int conceptId, TreeVisualizationPage page) : this(client)
        {
            SenderPage = page;

            ConceptsComboBox.SelectedIndex = _concepts
                    .First(c => c.Key == conceptId)
                    .Value;

            SelectProperties(conceptId);
        }

        private void SelectConcepts()
        {
            var concepts = new List<string>();
            var dataTable = _SQLClient.SelectConcepts();
            _concepts = new Dictionary<int, int>();

            foreach (DataRow row in dataTable.Rows)
            {
                concepts.Add(
                    row["Name"].ToString().Trim()
                    );

                _concepts.Add(
                    int.Parse(row["Id"].ToString()),
                    concepts.Count() - 1
                    );
            }

            ConceptsComboBox.ItemsSource = concepts;
        }

        private void SelectProperties(int conceptId)
        {
            var properties = new List<string>();
            var dataTable = _SQLClient.SelectPropertiesNotOfConcept(conceptId);
            _properties = new Dictionary<int, int>();

            foreach (DataRow row in dataTable.Rows)
            {
                properties.Add(
                    row["Name"].ToString().Trim() + ". Е.и.: " +
                    row["Measure"].ToString().Trim()
                    );

                _properties.Add(
                    int.Parse(row["IdProperty"].ToString()),
                    properties.Count() - 1);
            }

            PropertiesComboBox.ItemsSource = properties;
        }

        private void ConceptsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ConceptsComboBox.SelectedValue != null)
            {
                SelectProperties(_concepts
                                .First(c => c.Value == ConceptsComboBox.SelectedIndex)
                                .Key);
            }
        }

        private void AddPropertyToConceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConceptsComboBox.SelectedItem == null)
            {
                MessageBox.Show("Понятие не выбрано или не существует.", "Ошибка");
                return;
            }

            if (PropertiesComboBox.SelectedItem == null)
            {
                MessageBox.Show("Свойство не выбрано или не существует.", "Ошибка");
                return;
            }

            _SQLClient.InsertPropertyToConcept(
                    _concepts
                        .First(c => c.Value == ConceptsComboBox.SelectedIndex)
                        .Key,
                    _properties
                        .First(p => p.Value == PropertiesComboBox.SelectedIndex)
                        .Key,
                    PropertyValueTextBox.Text.ParseToNullableInt()
                );

            SenderPage.SelectConceptProperties(
                _concepts
                .First(c => c.Value == ConceptsComboBox.SelectedIndex)
                .Key);
        }

        private void PropertyValueTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");

            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
