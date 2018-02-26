using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Models
{
    public class Definition
    {
        public int ClassConceptId { get; set; }

        public int SourceId { get; set; }

        public string Name { get; set; }

        public int Page { get; set; }

        public string SourceName { get; set; }

        public static Definition CreateDefinition(DataRow definitionDataRow)
        {
            return new Definition
            {
                ClassConceptId = definitionDataRow.Field<int>("IdClassConcept"),
                SourceId = definitionDataRow.Field<int>("IdSource"),
                Name = definitionDataRow.Field<string>("Definition").Trim(),
                Page = definitionDataRow.Field<int>("Page"),
                SourceName = definitionDataRow.Field<string>("Name").Trim()
            };
        }

        public static IEnumerable<Definition> CreateDefinitions(DataTable definitionDataTable)
        {
            foreach(DataRow row in definitionDataTable.Rows)
            {
                yield return CreateDefinition(row);
            }
        }
    }
}
