using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Classification.Utility
{
    public static class DataGridHelper
    {
        public static bool IsSingleRowSelected(this DataGrid dataGrid)
        {
            return dataGrid.SelectedItems.Count == 1;
        }
    }
}
