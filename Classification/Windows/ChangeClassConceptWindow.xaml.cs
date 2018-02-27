using Classification.Frames;
using Classification.Models;
using Classification.Utility.SQL;
using System.Data;
using System.Windows;

namespace Classification.Windows
{
    /// <summary>
    /// Interaction logic for ChangeConceptWindow.xaml
    /// </summary>
    public partial class ChangeClassConceptWindow : Window
    {
        private Concept _concept;

        private SQLClient _sqlClient;

        private int _classificationId;

        private int _conceptId;

        public ChangeClassConceptWindow(SQLClient sqlClient, int classificationId, int conceptId)
        {
            InitializeComponent();

            _sqlClient = sqlClient;

            _conceptId = conceptId;

            _classificationId = classificationId;

            SelectConcept(conceptId);

            ConceptNameTextBox.Text = _concept.Name.Trim();
            SpecDifferenceNameTextBox.Text =
                _sqlClient.FindClassConcept(classificationId, conceptId)
                    .Field<string>("SpecDifference")
                    .Trim();
        }

        private void SelectConcept(int conceptId)
        {
            _concept = Concept.CreateConcept(_sqlClient.FindConcept(conceptId));
        }

        private void SaveConceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConceptNameTextBox.Text != string.Empty)
            {
                _concept.Name = ConceptNameTextBox.Text;

                int classConceptId = 
                    _sqlClient.FindClassConcept(_classificationId, _conceptId).Field<int>("Id");

                _sqlClient.UpdateClassificationConcept(
                    classConceptId,
                    ConceptNameTextBox.Text,
                    SpecDifferenceNameTextBox.Text
                    );

                Classifications.Instance.SelectClassificationConcepts();
                Concepts.Instance.SelectConcepts();
            }         
        }
    }
}
