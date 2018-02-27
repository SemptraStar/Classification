using QuickGraph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Models.GraphSharp
{
    public class ConceptEdge : Edge<ConceptVertex>, INotifyPropertyChanged
    {
        public ConceptEdge(ConceptVertex source, ConceptVertex target) : base(source, target)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
