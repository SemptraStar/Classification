using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;
using GraphSharp.Controls;
using System.Diagnostics;

namespace Classification.Models.GraphSharp
{
    public class ConceptVertex
    {
        public Concept Concept { get; set; }

        public ConceptVertex(Concept concept)
        {
            Concept = concept;
        }

        public ConceptVertex FindConceptParent(List<ConceptVertex> conceptVertecies)
        {
            return conceptVertecies.FirstOrDefault(cv => cv.Concept.Id == Concept.ParentConceptId);
        }

        public override string ToString()
        {
            return Concept.Name;
        }
    }
}
