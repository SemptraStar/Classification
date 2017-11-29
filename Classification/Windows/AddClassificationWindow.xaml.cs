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

        public AddClassificationWindow()
        {
            InitializeComponent();
        }

        public AddClassificationWindow(SQLClient client) : this()
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

        private void AddClassificationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _SQLClient.ExecuteProcedureVoid("InsertClassification",
                    new string[] { "@Type", "@ConceptRootId", "@Base"    },
                    new object[]
                    {
                        ClassificationTypeComboBox.Text,
                        ConceptsRootComboBox.Text.Split('.')[0],
                        ClassificationBaseTextBox.Text
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

            Frames.Classifications.Instance.SaveChangesToDB();
            Frames.Classifications.Instance.RefreshClassificationsDataGrid();
        }
    }
}
