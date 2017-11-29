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
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable tempDT = new DataTable();
            _SQLClient.Select(tempDT, ref adapter, "SELECT IdConcept, Name FROM Concept;");
            List<string> concepts = new List<string>();

            foreach(DataRow row in tempDT.Rows)
            {
                concepts.Add(row["IdConcept"].ToString() + ". " + row["Name"].ToString().Trim());
            }

            ConceptsRootComboBox.ItemsSource = concepts;
        }

        private void AddPropertyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _SQLClient.Insert("INSERT INTO Property(Name, IdConceptRoot, Measure)",
                new string[] { "@name", "@concept", "@measure" },
                new string[] { PropertyNameTextBox.Text,
                               ConceptsRootComboBox.Text.Split('.')[0],
                               PropertyMeasureTextBox.Text });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

            MessageBox.Show("New recording successfully added to property table.", "Success!");

            Frames.Properties.Instance.SaveChangesToDB();
            Frames.Properties.Instance.RefreshDataGrid();
        }
    }
}
