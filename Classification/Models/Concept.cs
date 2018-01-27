using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Models
{
    public class Concept
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SpeciesDifference { get; set; }
        public string Definition { get; set; }
        public string Source { get; set; }
        public int? ParentConceptId { get; set; }
        public int Level { get; set; }

        public static Concept CreateConcept(DataRow conceptDataRow)
        {
            return new Concept
            {
                Id = conceptDataRow.Field<int>("Id"),
                Name = conceptDataRow.Field<string>("Name")?.Trim(),
                SpeciesDifference = conceptDataRow.Field<string>("SpecDifference")?.Trim(),
                Definition = conceptDataRow.Field<string>("Definition")?.Trim(),
                Source = conceptDataRow.Field<string>("Source")?.Trim(),
                ParentConceptId = conceptDataRow.Field<int?>("ParentId"),
                Level = conceptDataRow.Field<int>("Level")
            };
        }
        public static List<Concept> CreateConcepts(DataTable conceptsDataTable)
        {
            List<Concept> concepts = new List<Concept>();

            foreach(var conceptDataRow in conceptsDataTable.Rows)
            {
                concepts.Add(CreateConcept((DataRow)conceptDataRow));
            }

            return concepts;
        }

        public Concept FindParent(List<Concept> concepts)
        {
            return concepts.FirstOrDefault(c => c.Id == ParentConceptId);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
