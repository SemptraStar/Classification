using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Utility.SQL
{
    public class SqlQuerySyntaxException : Exception
    {
        public SqlQuerySyntaxException(string message) : base(message)
        {

        }
    }
}
