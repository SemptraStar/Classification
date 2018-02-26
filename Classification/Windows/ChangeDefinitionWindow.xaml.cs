using Classification.Utility.SQL;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Classification.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddClassificatonWindow.xaml
    /// </summary>
    public partial class ChangeDefinitionWindow : Window
    {
        private readonly SQLClient _sqlClient;
        private int? _selectedClassificationId;
        private int? _selectedConceptId;
        private int _SelectedClassConceptId;
        private string _definition;

        public ChangeDefinitionWindow()
        {
            InitializeComponent();

        }

        public ChangeDefinitionWindow(SQLClient client) : this()
        {
            _sqlClient = client;               
        }

        public ChangeDefinitionWindow(SQLClient client, int selectedClassificationId, 
            int selectedConceptId, string definition) : this(client)
        {
            _selectedClassificationId = selectedClassificationId;
            _selectedConceptId = selectedConceptId;
            _definition = definition;

            FindClassConcept();
            FindClassification();
            FindConcept();
            SelectSources();
            FindDefinition();            
        }

        private void FindClassConcept()
        {
            DataRow classConceptDataRow = _sqlClient.FindClassConcept((int)_selectedClassificationId, (int)_selectedConceptId);

            _SelectedClassConceptId = classConceptDataRow.Field<int>("Id");
        }
        private void FindClassification()
        {
            DataRow classificationDataRow = _sqlClient.FindClassification((int)_selectedClassificationId);

            ClassificationText.Text =
                  classificationDataRow.Field<int>("IdClassification").ToString().Trim() +
                  ". Тип: " +
                  classificationDataRow.Field<string>("Type").Trim() +
                  "Основание: " +
                  classificationDataRow.Field<string>("Base").Trim(); 
        }
        private void FindConcept()
        {
            DataRow conceptDataRow = _sqlClient.FindConcept((int)_selectedConceptId);

            ConceptText.Text =
                  conceptDataRow.Field<int>("IdConcept").ToString().Trim() +
                  ". " +
                  conceptDataRow.Field<string>("Name").Trim();
        }
        private void SelectSources()
        {
            DataTable dataTable = _sqlClient.SelectSources();
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
        private void FindDefinition()
        {
            DefinitionTextBox.Text = _definition;
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SaveDefinitionButton_Click_1(object sender, RoutedEventArgs e)
        {
            /*
            _SQLClient.ExecuteProcedureVoid(
                "UpdateDefinition",
                new string[] {
                    "@ClassConceptId",
                    "@ClassificationId",
                    "@ConceptId",
                    "@SourceId",
                    "@DefinitionText",
                    "@Page"
                },
                new object[] {
                    _SelectedClassConceptId,
                    _SelectedClassificationId,
                    _SelectedConceptId,
                    int.Parse(((string)SourceComboBox.SelectedValue).Split('.')[0]),
                    DefinitionTextBox.Text,
                    int.Parse(PageTextBox.Text)
                });

            Frames.Classifications.Instance.SelectClassificationConcepts();
            */
        }
    }
}
