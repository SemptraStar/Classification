using Classification.Utility.SQL;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace Classification.Windows
{
    /// <summary>
    /// Interaction logic for ChangeClassificationWindow.xaml
    /// </summary>
    public partial class ChangeClassificationWindow : Window
    {
        private SQLClient _sqlClient;

        private Models.Classification _classification;

        public ChangeClassificationWindow(SQLClient sqlClient, int classificationId)
        {
            InitializeComponent();

            _sqlClient = sqlClient;

            SelectClassification(classificationId);
            SelectClassificationTypes();
        }

        private void SelectClassification(int classificationId)
        {
            _classification = Models.Classification
                .CreateClassification(_sqlClient.FindClassification(classificationId));

            ClassificationTypeComboBox.Text = _classification.Type;
            ConceptsRootTextBox.Text = GetConceptName(_classification.ConceptRootId);
            ClassificationBaseTextBox.Text = _classification.Base;
        }

        private string GetConceptName(int conceptId)
        {
            return _sqlClient.FindConcept(conceptId).Field<string>("Name").Trim();
        }

        private void SelectClassificationTypes()
        {
            var types = new List<string>();

            foreach (DataRow row in _sqlClient.SelectClassificationsTypes().Rows)
            {
                types.Add(row["Type"].ToString().Trim());
            }

            ClassificationTypeComboBox.ItemsSource = types;
        }

        private void SaveClassificationButton_Click(object sender, RoutedEventArgs e)
        {
            _classification.Type = ClassificationTypeComboBox.Text;
            _classification.Base = ClassificationBaseTextBox.Text;

            _sqlClient.UpdateClassification(_classification);

            Frames.Classifications.Instance.SelectClassifications();
            Frames.Concepts.Instance.SelectClassifications();
            Graphs.TreeVisualizationPage.Instance.SelectClassifications();
        }
    }
}
