using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Models
{
    public class ClassificationConcept
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SpeciesDifference { get; set; }
        public int? ParentConceptId { get; set; }
        public int Level { get; set; }

        public static ClassificationConcept CreateClassificationConcept(DataRow conceptDataRow)
        {
            return new ClassificationConcept
            {
                Id = conceptDataRow.Field<int>("Id"),
                Name = conceptDataRow.Field<string>("Name")?.Trim(),
                SpeciesDifference = conceptDataRow.Field<string>("SpecDifference")?.Trim(),
                ParentConceptId = conceptDataRow.Field<int?>("ParentId"),
                Level = conceptDataRow.Field<int>("Level")
            };
        }
        public static List<ClassificationConcept> CreateClassificationConcepts(DataTable conceptsDataTable)
        {
            List<ClassificationConcept> concepts = new List<ClassificationConcept>();

            foreach(var conceptDataRow in conceptsDataTable.Rows)
            {
                concepts.Add(CreateClassificationConcept((DataRow)conceptDataRow));
            }

            return concepts;
        }

        public ClassificationConcept FindParent(List<ClassificationConcept> concepts)
        {
            return concepts.FirstOrDefault(c => c.Id == ParentConceptId);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
