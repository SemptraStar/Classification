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
    public partial class AddClassificationToConceptWindow : Window
    {
        private readonly SQLClient _SQLClient;

        private int _SelectedClassificationId = -1;
        private int _SelectedConceptId = -1;
        private int _SelectedParentConceptId = -1;

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

            foreach (DataRow row in dataTable.Rows)
            {
                concepts.Add(
                    row["Id"].ToString().Trim() + ". " + 
                    row["Name"].ToString().Trim());
            }

            ConceptsComboBox.ItemsSource = null;
            ConceptsComboBox.ItemsSource = concepts;
        }
        private void SelectConceptsFromClassification()
        {
            DataTable dataTable = _SQLClient
                .SelectClassificationConceptsRaw(_SelectedClassificationId);

            List<string> parents = new List<string>();

            foreach (DataRow row in dataTable.Rows)
            {
                parents.Add(
                    row["Id"].ToString().Trim() + ". " + 
                    row["Name"].ToString().Trim()
                    );
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
        private void ConceptsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ConceptsComboBox.SelectedIndex != -1)
            {
                _SelectedConceptId = int.Parse(
                    ConceptsComboBox
                        .SelectedItem
                        .ToString()
                        .Split('.')[0]
                    );
            }      
        }
        private void ParentConceptComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ParentConceptComboBox.SelectedIndex != -1)
            {
                _SelectedParentConceptId = int.Parse(
                    ParentConceptComboBox
                        .SelectedItem
                        .ToString()
                        .Split('.')[0]
                    );
            }         
        }

        private void AddClassificationToPropertyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ClassificationsComboBox.SelectedIndex < 0 || 
                    ConceptsComboBox.SelectedIndex < 0 ||
                    ParentConceptComboBox.SelectedIndex < 0)
                    return;

                _SQLClient.InsertConceptToClassification(
                    _SelectedClassificationId,
                    _SelectedConceptId,
                    _SelectedParentConceptId,
                    SpeciesDifferenceTextBox.Text
                    );

                Frames.Classifications.Instance.SelectClassificationConcepts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        } 
    }
}
