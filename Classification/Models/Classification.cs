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
        public string Type { get; set; }
        public string Base { get; set; } 
        public Concept RootConcept { get; set; }

        public static Classification CreateClassification(DataRow classificationRow)
        {
            return new Classification
            {
                Type = (string)classificationRow["Type"],
                Base = (string)classificationRow["Base"],
                RootConcept = new Concept
                {
                    Name = (string)classificationRow["ConceptRoot"]
                }
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
