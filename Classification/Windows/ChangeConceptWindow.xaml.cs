using Classification.Models;
using Classification.Utility.SQL;
using System.Windows;

namespace Classification.Windows
{
    /// <summary>
    /// Interaction logic for ChangeConceptWindow.xaml
    /// </summary>
    public partial class ChangeConceptWindow : Window
    {
        private Concept _concept;

        private SQLClient _sqlClient;

        public ChangeConceptWindow(SQLClient sqlClient, int conceptId)
        {
            InitializeComponent();

            _sqlClient = sqlClient;

            SelectConcept(conceptId);

            ConceptNameTextBox.Text = _concept.Name;
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

                _sqlClient.UpdateConcept(_concept);

                Frames.Concepts.Instance.SelectConcepts();
                Frames.Classifications.Instance.SelectClassificationConcepts();
            }         
        }
    }
}
