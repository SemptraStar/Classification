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

namespace Classification.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddClassificatonWindow.xaml
    /// </summary>
    public partial class AddClassificationWindow : Window
    {
        private readonly SQLClient _SQLClient;

        private Dictionary<int, string> _concepts;

        public AddClassificationWindow()
        {
            InitializeComponent();
        }
        public AddClassificationWindow(SQLClient client) : this()
        {
            _SQLClient = client;

            SelectClassificationTypes();
            SelectConcepts();
        }

        private void SelectClassificationTypes()
        {
            var types = new List<string>();

            foreach(DataRow row in _SQLClient.SelectClassificationsTypes().Rows)
            {
                types.Add(row["Type"].ToString().Trim());
            }

            ClassificationTypeComboBox.ItemsSource = types;
        }

        private void SelectConcepts()
        {
            var concepts = new List<string>();
            var dataTable = _SQLClient.SelectConcepts();
            _concepts = new Dictionary<int, string>();

            foreach (DataRow row in dataTable.Rows)
            {
                concepts.Add(
                    row["Name"].ToString().Trim()
                    );

                _concepts.Add(int.Parse(row["Id"].ToString()), row["Name"].ToString().Trim());
            }

            ConceptsRootComboBox.ItemsSource = concepts;
        }

        private void AddClassificationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string conceptName;
                int conceptId;

                if (ConceptsRootComboBox.SelectedValue != null)
                {
                    conceptName = ConceptsRootComboBox.SelectedValue.ToString();
                    conceptId = _concepts.First(c => c.Value == conceptName).Key;
                }                  
                else if (ConceptsRootComboBox.Text.Length != 0)
                {
                    conceptName = ConceptsRootComboBox.Text;
                    conceptId = _SQLClient.InsertConceptIdentity(conceptName);
                }
                else
                {
                    throw new ArgumentException("Поле с понятием должно быть заполнено.");
                }

                _SQLClient.InsertClassification(
                        ClassificationTypeComboBox.Text,
                        conceptId,
                        ClassificationBaseTextBox.Text
                    );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

            Frames.Classifications.Instance.SelectClassifications();
            Frames.Concepts.Instance.SelectClassifications();
            Graphs.TreeVisualizationPage.Instance.SelectClassifications();
        }
    }
}
