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
using GraphSharp.Algorithms.Layout.Simple.Hierarchical;
using GraphSharp.Algorithms.Layout.Simple.Tree;
using Classification.Models;
using Classification.Models.GraphSharp;
using QuickGraph;
using System.ComponentModel;
using System.Data;
using Classification.Utility.SQL;
using GraphSharp.Controls;

namespace Classification.Graphs
{
    /// <summary>
    /// Interaction logic for TreeVisualizationPage.xaml
    /// </summary>
    public partial class TreeVisualizationPage : Page
    {
        private ConceptGraphViewModel _conceptGraphViewModel;

        private SQLClient _SQLClient;

        private int _selectedClassificationId;


        private Brush VertexDefaultBackgroundColor;
        private Brush VertexMouseOverBackgroundColor;

        public TreeVisualizationPage()
        {
            _conceptGraphViewModel = new ConceptGraphViewModel();
            DataContext = _conceptGraphViewModel;
            InitializeComponent();

            VertexDefaultBackgroundColor = Brushes.Red;
            VertexMouseOverBackgroundColor = Brushes.Beige;
        }
        public TreeVisualizationPage(SQLClient sqlClient) : this()
        {
            _SQLClient = sqlClient;         
            SelectClassifications();
        }
        
        private void GenerateVertexEvents()
        {
            foreach (var vertex in graphLayout.Children)
            {
                if (vertex is VertexControl)
                {
                    var vertexControl = (vertex as VertexControl);
                    vertexControl.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Vertex_MouseClick);
                    vertexControl.ContextMenu = CreateVertexContextMenu();
                }              
            }
        }

        private void Vertex_MouseClick(object sender, MouseButtonEventArgs e)
        {
            var concept = ((sender as VertexControl).Vertex as ConceptVertex).Concept;

            ConceptLabel.Text = concept.Name;
            ConceptDefinitionLabel.Text = concept.Definition;
            ConceptSpecDifferenceLabel.Text = concept.SpeciesDifference;
            SourceLabel.Text = concept.Source;
        }
        private ContextMenu CreateVertexContextMenu()
        {
            var contextMenu = new ContextMenu();
            var addConceptItem = new MenuItem();
            addConceptItem.Header = "Добавить понятие";
            addConceptItem.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(AddConcept_MouseClick);
            contextMenu.Items.Add(addConceptItem);

            return contextMenu;
        }

        private void AddConcept_MouseClick(object sender, MouseButtonEventArgs e)
        {
            var concept = ((((
                sender as MenuItem)
                .Parent as ContextMenu)
                .PlacementTarget as VertexControl)
                .Vertex as ConceptVertex)
                .Concept;

            MessageBox.Show(concept.Name);
        }

        private void SelectClassifications()
        {
            DataTable dataTable = _SQLClient.SelectClassifications();
            List<string> classifications = new List<string>();

            foreach (DataRow row in dataTable.Rows)
            {
                classifications.Add(row["Id"].ToString().Trim() +
                    ". Основание: " + row["Base"].ToString().Trim() +
                    "; Тип: " + row["Type"].ToString().Trim());
            }

            ClassificationsComboBox.ItemsSource = classifications;          
        }

        private void ClassificationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedClassificationId = int.Parse(ClassificationsComboBox.SelectedItem.ToString().Split('.')[0]);

            var classificationConcepts = _SQLClient.SelectClassificationConcepts(_selectedClassificationId);

            _conceptGraphViewModel.GenerateGraph(Concept.CreateConcepts(classificationConcepts));

            GenerateVertexEvents();
        }
    }
}
