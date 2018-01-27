using System;
using System.Collections;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Classification.Utility.SQL;
using System.Data.SqlClient;
using System.Data;

namespace Classification.Frames
{
    /// <summary>
    /// Логика взаимодействия для Sources.xaml
    /// </summary>
    public partial class Concepts : Page
    {
        private readonly SQLClient _SQLClient;

        private int? _selectedConceptId = null;
        private int? _selectedClassificationId = null;
        private int _selectedConceptsType = 0;

        public static Concepts Instance;

        public Concepts()
        {
            InitializeComponent();

            Instance = this;
        }

        public Concepts(SQLClient client) : this()
        {
            _SQLClient = client;

            DataTables.ConceptsDataTable = new DataTable();
            SelectConcepts();

            DataTables.ConceptChildsDataTable = new DataTable();
            ConceptChildsDataGrid.ItemsSource = DataTables.ConceptChildsDataTable?.DefaultView;

            SelectClassifications();
        }

        public void SelectConcepts()
        {
            DataTables.ConceptsDataTable.Clear();
            DataTables.ConceptsDataTable = _SQLClient.SelectConcepts();

            ConceptsDataGrid.ItemsSource = null;
            ConceptsDataGrid.ItemsSource = DataTables.ConceptsDataTable.DefaultView;
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
        private void SelectClassificationConcepts(int conceptId, int classificationId)
        {
            DataTables.ConceptChildsDataTable = _selectedConceptsType == 0 ?
                _SQLClient.SelectConceptChilds(conceptId, classificationId) :
                _SQLClient.SelectConceptParents(conceptId, classificationId);

            ConceptChildsDataGrid.ItemsSource = DataTables.ConceptChildsDataTable?.DefaultView;
        }

        private void ClassificationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedClassificationId = int.Parse(ClassificationsComboBox.SelectedItem.ToString().Split('.')[0]);
            DisplayConceptChilds();
        }

        private void ConceptsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ConceptsDataGrid.SelectedItems.Count == 1)
            {
                _selectedConceptId = (int)((DataRowView)ConceptsDataGrid.SelectedItem)["Id"];
                DisplayConceptChilds();
            }
            else
            {
                _selectedConceptId = null;
            }
        }

        private void DisplayConceptChilds()
        {
            if (_selectedClassificationId != null && _selectedConceptId != null)
            {
                ClearConceptChildsDataGrid();

                SelectClassificationConcepts((int)_selectedConceptId, (int)_selectedClassificationId);
            }
        }

        public void ClearConceptChildsDataGrid()
        {
            DataTables.ConceptChildsDataTable.Clear();
            ConceptChildsDataGrid.Items.Refresh();
        }

        private void ConceptsTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedConceptsType = ConceptsTypeComboBox.SelectedIndex;
            DisplayConceptChilds();
        }

        private void AddConceptButton_Click(object sender, RoutedEventArgs e)
        {
            var addConceptWindow = new Windows.AddConceptWindow(_SQLClient);

            addConceptWindow.Show();
        }
    }
}
