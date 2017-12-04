using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classification.Models.GraphSharp;
using System.ComponentModel;

namespace Classification.Models
{
    public class ConceptGraphViewModel : INotifyPropertyChanged
    {
        private ConceptGraph _graph;
        public ConceptGraph Graph
        {
            get { return _graph; }
            set
            {
                _graph = value;
                NotifyPropertyChanged("Graph");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void GenerateGraph(List<Concept> concepts)
        {
            var conceptGraph = new ConceptGraph();

            var vertecies = concepts.Select(c => new ConceptVertex(c)).ToList();
            conceptGraph.AddVertexRange(vertecies);

            foreach (var vertex in vertecies)
            {
                var parent = vertex.FindConceptParent(vertecies);

                if (parent != null)
                    conceptGraph.AddEdge(new ConceptEdge(parent, vertex));
            }

            Graph = conceptGraph;
        }
    }
}
