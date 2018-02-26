using Classification.Utility.SQL;
using System.Windows;

namespace Classification.Windows
{
    /// <summary>
    /// Interaction logic for AddConceptWindow.xaml
    /// </summary>
    public partial class AddConceptWindow : Window
    {
        private SQLClient _SQLClient;

        public AddConceptWindow()
        {
            InitializeComponent();
        }
        public AddConceptWindow(SQLClient sqlClient) : this()
        {
            _SQLClient = sqlClient;
        }

        private void AddConceptButton_Click(object sender, RoutedEventArgs e)
        {
            _SQLClient.InsertConcept(ConceptNameTextBox.Text);

            Frames.Concepts.Instance.SelectConcepts();
        }
    }
}
