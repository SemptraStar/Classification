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

        private int classificationId = 0;

        public AddClassificationToConceptWindow()
        {
            InitializeComponent();
        }

        public AddClassificationToConceptWindow(SQLClient client) : this()
        {
            _SQLClient = client;

            SelectClassifications();
        }

        private void SelectClassifications()
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable tempDT = new DataTable();
            _SQLClient.Select(tempDT, ref adapter, "SELECT " +
                "Classification.IdClassification as Id, Classification.Base as Base, Classification.Type as Type " +
                "FROM Classification;");
                
            List<string> classifications = new List<string>();

            foreach (DataRow row in tempDT.Rows)
            {
                classifications.Add(row["Id"].ToString().Trim() + ". Base: " + row["Base"].ToString().Trim() + "; Type: " + row["Type"].ToString().Trim());
            }

            ClassificationsComboBox.ItemsSource = null;
            ClassificationsComboBox.ItemsSource = classifications;
        }

        private void SelectConcepts()
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable tempDT = new DataTable();

            _SQLClient.Select(tempDT, ref adapter, 
                String.Format(
                "SELECT Concept.IdConcept as Id, Concept.Name as Name " +
                "FROM Concept " +
                "WHERE Concept.IdConcept NOT IN " +
                "(SELECT ClassificationToConcept.IdConcept " +
                "FROM ClassificationToConcept " +
                "WHERE ClassificationToConcept.IdClassification = {0});", classificationId));

            List <string> concepts = new List<string>();

            foreach (DataRow row in tempDT.Rows)
            {
                concepts.Add(row["Id"].ToString().Trim() + ". " + row["Name"].ToString().Trim());
            }

            ConceptsComboBox.ItemsSource = null;
            ConceptsComboBox.ItemsSource = concepts;
        }

        private void SelectParentConcepts()
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable tempDT = new DataTable();

            _SQLClient.Select(tempDT, ref adapter,
                String.Format(
                "SELECT Concept.IdConcept as Id, Concept.Name as Name " +
                "FROM Concept " +
                "WHERE Concept.IdConcept IN " +
                "(SELECT ClassificationToConcept.IdConcept " +
                "FROM ClassificationToConcept " +
                "WHERE ClassificationToConcept.IdClassification = {0});", classificationId));

            List<string> parents = new List<string>();

            foreach (DataRow row in tempDT.Rows)
            {
                parents.Add(row["Id"].ToString().Trim() + ". " + row["Name"].ToString().Trim());
            }

            ParentConceptComboBox.ItemsSource = null;
            ParentConceptComboBox.ItemsSource = parents;
        }

        private void ClassificationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            classificationId = int.Parse(ClassificationsComboBox.SelectedItem.ToString().Split('.')[0]);
            SelectConcepts();
        }

        private void ConceptsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectParentConcepts();
        }
    }
}
