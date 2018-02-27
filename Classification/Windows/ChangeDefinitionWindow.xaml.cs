using System;
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
using System.Windows.Shapes;

using Classification.Utility.SQL;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Classification.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddClassificatonWindow.xaml
    /// </summary>
    public partial class ChangeDefinitionWindow : Window
    {
        private readonly SQLClient _SQLClient;
        private int? _SelectedClassificationId;
        private int? _SelectedConceptId;
        private string _SelectedSourceName;
        private int _SelectedSourceId;
        private int _SelectedClassConceptId;
        private string _Definition;

        public ChangeDefinitionWindow()
        {
            InitializeComponent();

        }

        public ChangeDefinitionWindow(SQLClient client) : this()
        {
            _SQLClient = client;
                
            //SelectSources();
        }

        public ChangeDefinitionWindow(SQLClient client, int selectedClassificationId, 
            int selectedConceptId, string definition) : this(client)
        {
            _SelectedClassificationId = selectedClassificationId;
            _SelectedConceptId = selectedConceptId;
            _Definition = definition;

            /*
            FindClassConcept();
            FindClassification();
            FindConcept();
            SelectSources();
            FindDefinition();
            */
        }

        /*
        private void FindClassConcept()
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable tempDT = new DataTable();

            _SQLClient.ExecuteProcdureScalar(
                "FindClassConcept",
                new string[] { "@ClassificationId", "@ConceptId" },
                new object[] { _SelectedClassificationId, _SelectedConceptId },
                tempDT,
                ref adapter);

            _SelectedClassConceptId = tempDT.Rows[0].Field<int>("Id");
        }
        private void FindClassification()
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable tempDT = new DataTable();
            _SQLClient.ExecuteProcdureScalar(
                "FindClassification",
                new string[] { "@ClassificationId" },
                new object[] { _SelectedClassificationId },
                tempDT,
                ref adapter);

            var row = tempDT.Rows[0];

            ClassificationText.Text = 
                  $"{row.Field<int>("IdClassification").ToString().Trim()}. " +
                  $"Тип: {row.Field<string>("Type").Trim()}" +
                  $"Основание: {row.Field<string>("Base").Trim()}"; 
        }
        private void FindConcept()
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable tempDT = new DataTable();
            _SQLClient.ExecuteProcdureScalar(
                "FindConcept",
                new string[] { "@ConceptId" },
                new object[] { _SelectedConceptId },
                tempDT,
                ref adapter);

            var row = tempDT.Rows[0];

            ConceptText.Text =
                  $"{row.Field<int>("IdConcept").ToString().Trim()}. " +
                  $"{row.Field<string>("Name").Trim()}";
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

            SourceComboBox.SelectedIndex = SourceComboBox
                .Items.OfType<string>().ToList()
                .FindIndex(item =>
                int.Parse(item.Split('.').First()) == _SelectedSourceId);
        }
        private void FindDefinition()
        {
            DefinitionTextBox.Text = _Definition;
        }
        */

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
