using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Models.GraphSharp
{
    public class ConceptGraph : BidirectionalGraph<ConceptVertex, ConceptEdge>
    {
        public ConceptGraph()
        {

        }

        public ConceptGraph(bool allowParallelEdges) 
            : base(allowParallelEdges)
        {

        }

        public ConceptGraph(bool allowParallelEdges, int vertexCapacity) 
            : base(allowParallelEdges, vertexCapacity)
        {

        }
    }
}
