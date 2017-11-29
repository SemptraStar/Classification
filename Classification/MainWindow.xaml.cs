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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;

using Classification.Utility.SQL;
using System.Data;

namespace Classification
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SQLClient _SQLClient;

        private Frames.Classifications _ClassificationPage;
        private Frames.Concepts _ConceptPage;
        private Frames.ClassificationsToConcept _ClassificationsToConceptsPage;
        private Frames.Properties _PropertiesPage;
        private Frames.Definitions _DefinitionsPage;
        private Frames.Sources _SourcePage;
        

        public MainWindow()
        {
            InitializeComponent();

            _SQLClient = new SQLClient();
            InitPages();

            MainFrame.NavigationService.Navigate(_ClassificationPage);
        }

        private void InitPages()
        {
            _ClassificationPage = new Frames.Classifications(_SQLClient);
            _ConceptPage = new Frames.Concepts(_SQLClient);
            _ClassificationsToConceptsPage = new Frames.ClassificationsToConcept(_SQLClient);
            _PropertiesPage = new Frames.Properties(_SQLClient);
            _DefinitionsPage = new Frames.Definitions(_SQLClient);
            _SourcePage = new Frames.Sources(_SQLClient);
        }


        private void ClassificationsOpen_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(_ClassificationPage);
        }

        private void ConceptsOpen_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(_ConceptPage);
        }

        private void PropertiesOpen_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(_PropertiesPage);
        }

        private void SourcesOpen_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(_SourcePage);
        }

        private void ClassificationsToConceptsOpen_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(_ClassificationsToConceptsPage);
        }

        private void DefinitionsOpen_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(_DefinitionsPage);
        }
    }
}
