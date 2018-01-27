using Classification.Utility.SQL;
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
