using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Models
{
    public class Classification
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public string Base { get; set; } 

        public int ConceptRootId { get; set; }

        public static Classification CreateClassification(DataRow classificationRow)
        {
            return new Classification
            {
                Id = classificationRow.Field<int>("IdClassification"),
                Type = classificationRow.Field<string>("Type").Trim(),
                Base = classificationRow.Field<string>("Base").Trim(),
                ConceptRootId = classificationRow.Field<int>("IdConceptRoot")
            };
        }
        public static List<Classification> CreateClassifications(DataTable classificationTable)
        {
            var classificationList = new List<Classification>();

            foreach(var classificationRow in classificationTable.Rows)
            {
                classificationList.Add(CreateClassification((DataRow)classificationRow));
            }

            return classificationList;
        }
    }
}
