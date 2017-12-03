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
    public partial class TreeVisualizationPage : Page, INotifyPropertyChanged
    {
        public IBidirectionalGraph<object, IEdge<object>> _classificationTree;
        public IBidirectionalGraph<object, IEdge<object>> ClassificationTree
        {
            get { return _classificationTree; }
            set
            {
                if (!Equals(value, _classificationTree))
                {
                    _classificationTree = value;
                    RaisePropChanged("ClassificationTree");
                }
            }
        }

        public List<Concept> TreeConcepts;

        public event PropertyChangedEventHandler PropertyChanged;

        private SQLClient _SQLClient;

        private int _selectedClassificationId;

        public TreeVisualizationPage()
        {
            InitializeComponent();
        }
        public TreeVisualizationPage(SQLClient sqlClient) : this()
        {
            _SQLClient = sqlClient;
            SelectClassifications();
            ParametrizeLayout();
        }

        public void RaisePropChanged(string name) =>
           PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

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

        private void ParametrizeLayout()
        {

            var _classificationTreeLayoutParameters = new EfficientSugiyamaLayoutParameters
            {
                PositionMode = 2,
                WidthPerHeight = 5.0,
                LayerDistance = 20.0,
                EdgeRouting = SugiyamaEdgeRoutings.Traditional
            };

            MainGraphLayout.LayoutParameters = _classificationTreeLayoutParameters;
        }

        public void CreateGraph(List<Concept> concepts)
        {
            TreeConcepts = concepts.OrderBy(c => c.Level).ToList();
            var classificationTree = new BidirectionalGraph<object, IEdge<object>>();
            classificationTree.AddVertexRange(concepts);

            foreach (var concept in TreeConcepts)
            {
                var parent = concept.FindParent(TreeConcepts);

                if (parent != null)
                    classificationTree.AddEdge(new Edge<object>(parent, concept));
            }

            ClassificationTree = classificationTree;
        }
        private void CreateClickHandlers()
        {
            foreach (var vertex in MainGraphLayout.Children)
            {
                if (vertex is VertexControl)
                    (vertex as VertexControl).MouseDown += new MouseButtonEventHandler(v_MouseDoubleClick);
            }
        }

        private void v_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(((sender as VertexControl).Vertex as Concept).Definition);
        }

        private void ClassificationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedClassificationId = int.Parse(ClassificationsComboBox.SelectedItem.ToString().Split('.')[0]);

            var classificationConcepts = _SQLClient.SelectClassificationConcepts(_selectedClassificationId);

            CreateGraph(Concept.CreateConcepts(classificationConcepts));
            CreateClickHandlers();
        }
    }
}
