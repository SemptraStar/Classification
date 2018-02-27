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
using Classification.Graphs;

namespace Classification.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddClassificatonWindow.xaml
    /// </summary>
    public partial class AddClassificationToConceptWindow : Window
    {
        private readonly SQLClient _SQLClient;

        private int _SelectedClassificationId = -1;

        private Dictionary<int, string> _conceptsOutsideClassification;
        private Dictionary<int, string> _conceptsInClassification;

        public object Sender;

        public AddClassificationToConceptWindow()
        {
            InitializeComponent();
        }
        public AddClassificationToConceptWindow(SQLClient client) : this()
        {
            _SQLClient = client;

            SelectClassifications();
        }
        public AddClassificationToConceptWindow(SQLClient client, int selectedClassificationId) : this(client)
        {
            _SelectedClassificationId = selectedClassificationId;

            ClassificationsComboBox.SelectedIndex = ClassificationsComboBox
                .Items.OfType<string>().ToList()
                .FindIndex(item =>
                int.Parse(item.Split('.').First()) == _SelectedClassificationId);
        }
        public AddClassificationToConceptWindow(SQLClient client, int selectedClassificationId, int selectedParentConceptId) : this(client, selectedClassificationId)
        {
            ParentConceptComboBox.SelectedIndex = ParentConceptComboBox
                .Items.OfType<string>().ToList()
                .FindIndex(item =>
                item == _conceptsInClassification
                    .First(c => c.Key == selectedParentConceptId)
                    .Value);
            
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
        private void SelectConceptsOutsideClassification()
        {
            DataTable dataTable = _SQLClient
                .SelectConceptsOutsideClassification(_SelectedClassificationId);

            List <string> concepts = new List<string>();
            _conceptsOutsideClassification = new Dictionary<int, string>();

            foreach (DataRow row in dataTable.Rows)
            {
                concepts.Add(
                    row["Name"].ToString().Trim());

                _conceptsOutsideClassification.Add(int.Parse(row["Id"].ToString()), row["Name"].ToString().Trim());
            }

            ConceptsComboBox.ItemsSource = null;
            ConceptsComboBox.ItemsSource = concepts;
        }
        private void SelectConceptsFromClassification()
        {
            DataTable dataTable = _SQLClient
                .SelectClassificationConceptsRaw(_SelectedClassificationId);

            List<string> parents = new List<string>();
            _conceptsInClassification = new Dictionary<int, string>();

            foreach (DataRow row in dataTable.Rows)
            {
                parents.Add(
                    row["Name"].ToString().Trim()
                    );

                _conceptsInClassification.Add(int.Parse(row["Id"].ToString()), row["Name"].ToString().Trim());
            }

            ParentConceptComboBox.ItemsSource = null;
            ParentConceptComboBox.ItemsSource = parents;
        }

        private void ClassificationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _SelectedClassificationId = int.Parse(ClassificationsComboBox.SelectedItem.ToString().Split('.')[0]);
            SelectConceptsOutsideClassification();
            SelectConceptsFromClassification();
        }

        private void AddClassificationToPropertyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_SelectedClassificationId < 0)
                    return;

                int selectedConceptId;
                int selectedParentId;

                if (ConceptsComboBox.SelectedValue != null)
                {
                    selectedConceptId = _conceptsOutsideClassification
                        .First(c => c.Value == ConceptsComboBox.SelectedValue.ToString())
                        .Key;
                }
                else
                {
                    if (ConceptsComboBox.Text.Length == 0)
                        throw new Exception("Поле с понятием не должно быть пустым");
                    if (_conceptsInClassification.Values.Contains(ConceptsComboBox.Text))
                        throw new Exception("Понятие уже находится в классификации");

                    selectedConceptId = _SQLClient.InsertConceptIdentity(ConceptsComboBox.Text);
                }

                if (ParentConceptComboBox.SelectedValue != null)
                {
                    selectedParentId = _conceptsInClassification
                        .First(c => c.Value == ParentConceptComboBox.SelectedValue.ToString())
                        .Key;
                }
                else
                {
                    if (ParentConceptComboBox.Text.Length == 0)
                        throw new Exception("Поле с понятием не должно быть пустым");
                    if (!_conceptsInClassification.Values.Contains(ConceptsComboBox.Text))
                        throw new Exception("Родительское понятие должно находиться в классификации");

                    selectedParentId = _conceptsInClassification
                        .First(c => c.Value == ParentConceptComboBox.Text)
                        .Key;
                }

                _SQLClient.InsertConceptToClassification(
                    _SelectedClassificationId,
                    selectedConceptId,
                    selectedParentId,
                    SpeciesDifferenceTextBox.Text
                    );

                SelectConceptsOutsideClassification();
                SelectConceptsFromClassification();

                if (Sender is TreeVisualizationPage)
                {
                    TreeVisualizationPage.Instance.GenerateGraph();
                }
                else if (Sender is Frames.Classifications)
                {
                    Frames.Classifications.Instance.SelectClassificationConcepts();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        } 
    }
}
