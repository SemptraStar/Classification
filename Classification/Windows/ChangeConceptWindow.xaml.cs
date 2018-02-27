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
using Classification.Models;
using Classification.Frames;

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

                Concepts.Instance.SelectConcepts();
            }         
        }
    }
}
