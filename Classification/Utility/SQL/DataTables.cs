using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Classification.Utility.SQL
{
    public static class DataTables
    {
        public static DataTable ClassificationsDataTable;
        public static DataTable ClassificationConceptsDataTable;

        public static DataTable ConceptsDataTable;
        public static DataTable ConceptsQueryDataTable;

        public static DataTable ClassificationsToConceptsDataTable;
        public static DataTable ClassificationsToConceptsQueryDataTable;

        public static DataTable PropertiesDataTable;
        public static DataTable PropertiesQueryDataTable;

        public static DataTable DefinitiosDataTable;
        public static DataTable DefinitionsQueryDataTable;

        public static DataTable SourcesDataTable;
        public static DataTable SourcesQueryDataTable;
    }
}
