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

        public static Concept CreateConcept(DataRow conceptDataRow)
        {
            return new Concept
            {
                Id = conceptDataRow.Field<int>("IdConcept"),
                Name = conceptDataRow.Field<string>("Name").Trim()
            };
        }
    }
}
