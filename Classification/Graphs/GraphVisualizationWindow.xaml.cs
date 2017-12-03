using Classification.Models;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using GraphSharp.Algorithms.Layout.Simple.Hierarchical;
using GraphSharp.Algorithms.Layout.Simple.Tree;

namespace Classification.Graphs
{
    /// <summary>
    /// Interaction logic for GraphVisualizationWindow.xaml
    /// </summary>
    public partial class GraphVisualizationWindow : Window, INotifyPropertyChanged
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

        public EfficientSugiyamaLayoutParameters ClassificationTreeLayoutParameters { get; set; }

        public List<Concept> TreeConcepts;

        public event PropertyChangedEventHandler PropertyChanged;

        public GraphVisualizationWindow()
        {
            InitializeComponent();
        }

        public void RaisePropChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

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
            
            foreach (var concept in TreeConcepts)
            {
                var parent = concept.FindParent(TreeConcepts);

                if (parent != null)
                    classificationTree.AddVerticesAndEdge(new Edge<object>(parent, concept));
            }

            ParametrizeLayout();

            ClassificationTree = classificationTree;
        }
    }
}
