using Classification.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Classification.Utility.SQL
{
    public class SQLClient : IDisposable
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
           
        public SqlConnection Connection;

        public SQLClient()
        {
            Connection = new SqlConnection(ConnectionString);
        }
       
        public DataTable ExecuteSelectProcedure(string procedure, string[] parameters, object[] values)
        {
            using (SqlCommand command = new SqlCommand(procedure, Connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                for (int i = 0; i < parameters?.Length; i++)
                {
                    command.Parameters.AddWithValue(parameters[i], values[i]);
                }

                try
                {
                    Connection.Open();

                    var adapter = new SqlDataAdapter
                    {
                        SelectCommand = command
                    };
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    return dataTable;
                }
                finally
                {
                    Connection.Close();
                }
            }
        }
        public DataTable SelectQuery(string query)
        {
            SqlCommand sqlCommand = new SqlCommand(query, Connection);
            var adapter = new SqlDataAdapter(sqlCommand);
            var dataTable = new DataTable();

            try
            {
                Connection.Open();
                adapter.Fill(dataTable);

                return dataTable;
            }
            finally
            {
                Connection?.Close();
            }
        }

        public DataTable SelectClassifications()
        {
            return SelectQuery(
                "SELECT " +
                "Classification.IdClassification as Id, Classification.Base as Base, Classification.Type as Type " +
                "FROM Classification;"
                );
        }
        public DataTable SelectClassificationsTypes()
        {
            return SelectQuery(
                "SELECT DISTINCT Classification.Type FROM Classification;"
                );
        }

        public DataTable SelectConcepts()
        {
            return SelectQuery(
                "SELECT Concept.IdConcept AS Id, Concept.Name " +
                "FROM Concept;"
                );
        }
        public DataTable SelectConceptsOutsideClassification(int classificationId)
        {
            return ExecuteSelectProcedure(
                "SelectConceptsOutsideClassification",
                new string[] { "@ClassificationId" },
                new object[] { classificationId }
                );
        }
        public DataTable SelectConceptChilds(int conceptId, int classificationId)
        {
            return ExecuteSelectProcedure(
                "SelectConceptChilds",
                new string[] { "@ConceptId", "@ClassificationId" },
                new object[] { conceptId, classificationId }
                );
        }
        public DataTable SelectConceptParents(int conceptId, int classificationId)
        {
            return ExecuteSelectProcedure(
                "SelectConceptParents",
                new string[] { "@ConceptId", "@ClassificationId" },
                new object[] { conceptId, classificationId }
                );
        }
        public DataTable SelectConceptISAClassifcations(int conceptId)
        {
            return ExecuteSelectProcedure(
                "SelectConceptISAClassifcations",
                new string[] { "@ConceptId"},
                new object[] { conceptId }
                );
        }

        public DataTable SelectClassConceptDefinitions(int classConceptId)
        {
            return ExecuteSelectProcedure(
                "SelectClassConceptDefinitions",
                new string[] { "@ClassConceptId" },
                new object[] { classConceptId }
                );
        }

        public DataTable SelectClassificationConcepts(int classificationId)
        {
            return ExecuteSelectProcedure(
                "SelectClassificationConcepts",
                new string[] { "@Classificationid" },
                new object[] { classificationId }
                );
        }
        public DataTable SelectClassificationConceptsRaw(int classificationId)
        {
            return ExecuteSelectProcedure(
               "SelectClassificationConceptsRaw",
               new string[] { "@Classificationid" },
               new object[] { classificationId }
               );
        }
        public DataTable SelectClassificationDefinitions(int classificationId)
        {
            return ExecuteSelectProcedure(
               "SelectDefinitions",
               new string[] { "@Classificationid" },
               new object[] { classificationId }
               );
        }
        public DataTable SelectClassificationsWithRootConcepts()
        {
            return ExecuteSelectProcedure(
               "SelectClassificationsWithRootConcepts",
               null,
               null
               );
        }

        public DataTable SelectProperties()
        {
            return SelectQuery(
                "SELECT Property.IdProperty AS Id, Property.Name, Concept.Name as ConceptRoot, Property.Measure " +
                "FROM Property  " +
                "JOIN Concept " +
                "ON Property.IdConceptRoot = Concept.IdConcept;"
                );
        }
        public DataTable SelectConceptProperties(int conceptId)
        {
            return ExecuteSelectProcedure(
              "SelectConceptProperties",
              new string[] { "@ConceptId" },
              new object[] { conceptId }
              );
        }
        public DataTable SelectPropertyArousedConcepts(int propertyId)
        {
            return ExecuteSelectProcedure(
              "SelectPropertyArousedConcepts",
              new string[] { "@PropertyId" },
              new object[] { propertyId }
              );
        }
        public DataTable SelectPropertiesNotOfConcept(int conceptId)
        {
            return ExecuteSelectProcedure(
              "SelectPropertiesNotOfConcept",
              new string[] { "@ConceptId" },
              new object[] { conceptId }
              );
        }

        public DataTable SelectSources()
        {
            return SelectQuery(
               "SELECT Source.IdSource AS Id, Name, Author, Year" +
               " FROM Source;"
               );
        }

        public DataTable SelectSourceDefinitions(int sourceId)
        {
            return ExecuteSelectProcedure(
               "SelectSourceDefinitions",
               new string[] { "@SourceId" },
               new object[] { sourceId }
               );
        }

        public DataRow FindClassification(int classificationId)
        {
            var dataTable = ExecuteSelectProcedure(
               "FindClassification",
               new string[] { "@ClassificationId" },
               new object[] { classificationId }
               );

            if (dataTable.Rows.Count == 1)
                return dataTable.Rows[0];
            else
                return null;
        }
        public DataTable FindConceptClassifications(int conceptId)
        {
            return ExecuteSelectProcedure(
                    "FindConceptClassifications",
                    new string[] { "@ConceptId" },
                    new object[] { conceptId }
                    );
        }
        public DataRow FindClassConcept(int classificationId, int conceptId)
        {
            var dataTable = ExecuteSelectProcedure(
                    "FindClassConcept",
                    new string[] { "@ClassificationId", "@ConceptId" },
                    new object[] { classificationId, conceptId }
                    );

            if (dataTable.Rows.Count == 1)
                return dataTable.Rows[0];
            else
                return null;
        }
        public DataRow FindConcept(int conceptId)
        {
            var dataTable = ExecuteSelectProcedure(
                "FindConcept",
                new string[] { "@ConceptId" },
                new object[] { conceptId }
                );

            if (dataTable.Rows.Count == 1)
                return dataTable.Rows[0];
            else
                return null;
        }
        public DataRow FindProperty(int propertyId)
        {
            var dataTable = ExecuteSelectProcedure(
                "FindProperty",
                new string[] { "@PropertyId" },
                new object[] { propertyId }
                );

            if (dataTable.Rows.Count == 1)
                return dataTable.Rows[0];
            else
                return null;
        }
        public DataRow FindSource(int sourceId)
        {
            var dataTable = ExecuteSelectProcedure(
               "FindSource",
               new string[] { "@SourceId" },
               new object[] { sourceId }
               );

            if (dataTable.Rows.Count == 1)
                return dataTable.Rows[0];
            else
                return null;
        }

        public bool IsClassConceptHasChilds(int classificationId, int conceptId)
        {
            DataTable childs = SelectConceptChilds(conceptId, classificationId);

            return childs.Rows.Count != 0;
        }

        public void ExecuteProcedure(string procedure, string[] parameters, object[] values)
        {
            using (SqlCommand command = new SqlCommand(procedure, Connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                for (int i = 0; i < parameters?.Length; i++)
                {
                    command.Parameters.AddWithValue(parameters[i], values[i]);
                }

                try
                {
                    Connection.Open();

                    command.ExecuteNonQuery();
                }
                finally
                {
                    Connection.Close();
                }
            }
        }
        public int ExecuteProcedureIdentity(string procedure, string[] parameters, object[] values)
        {
            using (SqlCommand command = new SqlCommand(procedure, Connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                for (int i = 0; i < parameters?.Length; i++)
                {
                    command.Parameters.AddWithValue(parameters[i], values[i]);
                }

                try
                {
                    Connection.Open();

                    return (int)command.ExecuteScalar();
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

        public void InsertClassification(string type, int conceptRootId, string classificationBase)
        {
            ExecuteProcedure("InsertClassification",
                    new string[] { "@Type", "@ConceptRootId", "@Base" },
                    new object[]
                    {
                        type,
                        conceptRootId,
                        classificationBase
                    });
        }
        public void InsertConcept(string name)
        {
            ExecuteProcedure(
                "InsertConcept",
                 new string[] { "@ConceptName" },
                 new object[] { name }
                );
        }
        public void InsertConceptToClassification(int classificationId, int conceptId, int parentId, string specDifference)
        {
            ExecuteProcedure(
                    "InsertConceptToClassification",
                    new string[] { "@ClassificationId", "@ConceptId", "@ParentId", "@SpecDifference" },
                    new object[]
                    {
                        classificationId,
                        conceptId,
                        parentId,
                        specDifference
                    });
        }
        public void InsertProperty(string name, int conceptId, string measure)
        {
            ExecuteProcedure(
                "InsertProperty",
                new string[] { "@Name", "@ConceptId", "@Measure" },
                new object[] {name, conceptId, measure }
                );
        }
        public void InsertPropertyToConcept(int conceptId, int propertyId, int? value)
        {
            var isaClassifications = SelectConceptISAClassifcations(conceptId);

            foreach(DataRow row in isaClassifications.Rows)
            {
                ExecuteProcedure(
                    "InsertPropertyToConcept",
                    new string[] { "@ClassificationId", "@ConceptId", "@PropertyId", "@Value" },
                    new object[] { (int)row["IdClassification"], conceptId, propertyId, value ?? (object)DBNull.Value }
                );
            }          
        }
        public void InsertDefinition(int classConceptId, int sourceId, string definitionText, int page)
        {
            ExecuteProcedure(
                    "InsertDefinition",
                    new string[] { "@ClassConceptId", "@SourceId", "@DefinitionText", "@Page" },
                    new object[]
                    {
                        classConceptId,
                        sourceId,
                        definitionText,
                        page
                    });
        }
        public void InsertSource(string name, string author, int year)
        {
            ExecuteProcedure(
                    "InsertSource",
                    new string[] { "@Name", "@Author", "@Year" },
                    new object[] { name, author, year }
                    );
        }

        public int InsertConceptIdentity(string name)
        {
            return ExecuteProcedureIdentity(
               "InsertConcept",
                new string[] { "@ConceptName" },
                new object[] { name }
               );
        }

        public void UpdateClassification(Models.Classification classification)
        {
            ExecuteProcedure(
                   "UpdateClassification",
                   new string[] { "@ClassificationId", "@Type", "@Base" },
                   new object[] { classification.Id, classification.Type, classification.Base }
                   );
        }
        public void UpdateClassificationConcept(int classificationConceptId, string name, string specDifference)
        {
            ExecuteProcedure(
                   "UpdateClassificationConcept",
                   new string[] { "@ClassificationConceptId", "@Name", "@SpecDifference" },
                   new object[] { classificationConceptId, name, specDifference }
                   );
        }
        public void UpdateClassificationConceptParent(int classificationId, int conceptId, int newParentId)
        {
            ExecuteProcedure(
                    "UpdateClassificationConceptParent",
                    new string[] { "@ClassificationId", "@ConceptId", "@NewParentId" },
                    new object[] { classificationId, conceptId, newParentId }
                    );
        }
        public void UpdateProperty(int propertyId, int conceptId, string name, string measure)
        {
            ExecuteProcedure(
                    "UpdateProperty",
                    new string[] { "@PropertyId", "@PropertyConceptRootId", "@Name", "@Measure" },
                    new object[] { propertyId, conceptId, name, measure }
                    );
        }
        public void UpdateConcept(Concept concept)
        {
            ExecuteProcedure(
                   "UpdateConcept",
                   new string[] { "@ConceptId", "@Name" },
                   new object[] { concept.Id, concept.Name }
                   );
        }
        public void UpdateConceptPropertyValue(int conceptId, int propertyId, int value)
        {
            ExecuteProcedure(
                   "UpdateConceptPropertyValue",
                   new string[] { "@ConceptId", "@PropertyId", "@Value" },
                   new object[] { conceptId, propertyId, value }
                   );
        }
        public void UpdateSource(int sourceId, string name, string author, int year)
        {
            ExecuteProcedure(
                    "UpdateSource",
                    new string[] { "@SourceId", "@Name", "@Author", "@Year" },
                    new object[]
                    { sourceId, name, author, year }
                    );
        }

        public void DeleteClassification(int classificationId)
        {
            ExecuteProcedure(
                "DeleteClassification",
                new string[] { "@ClassificationId" },
                new object[] { classificationId }
                );
        }
        public void DeleteConcept(int conceptId)
        {
            ExecuteProcedure(
                "DeleteConcept",
                new string[] { "@ConceptId" },
                new object[] { conceptId }
                );
        }
        public void DeleteClassificationConcept(int classificationId, int conceptId)
        {
            DeleteClassConceptDefinitions(classificationId, conceptId);

            ExecuteProcedure(
                "DeleteClassificationConcept",
                new string[] { "@ClassificationId", "@ConceptId" },
                new object[] { classificationId, conceptId }
                );
        }
        public void DeleteProperty(int propertyId)
        {
            ExecuteProcedure(
                "DeleteProperty",
                new string[] { "@PropertyId" },
                new object[] { propertyId }
                );
        }
        public void DeletePropertyFromConcepts(int conceptId, int propertyID)
        {
            var isaClassifications = SelectConceptISAClassifcations(conceptId);

            foreach (DataRow row in isaClassifications.Rows)
            {
                ExecuteProcedure(
                    "DeletePropertyFromConcepts",
                    new string[] { "@PropertyId", "@ConceptId", "@ClassificationId", },
                    new object[] { propertyID, conceptId, (int)row["IdClassification"] }
                );
            }
        }
        public void DeleteDefinition(int classConceptId, int sourceId)
        {
            ExecuteProcedure(
                "DeleteDefinition",
                new string[] { "@ClassConceptId", @"SourceId" },
                new object[] { classConceptId, sourceId }
                );
        }
        public void DeleteClassConceptDefinitions(int classificationId, int conceptId)
        {
            ExecuteProcedure(
                "DeleteClassConceptDefinitions",
                new string[] { "@ClassificationId", "@ConceptId" },
                new object[] { classificationId, conceptId }
                );
        }
        public void DeleteSource(int sourceId)
        {
            ExecuteProcedure(
                "DeleteSource",
                new string[] { "@SourceId" },
                new object[] { sourceId }
                );
        }

        public void Dispose()
        {
            Connection?.Dispose();
        }
    }
}
