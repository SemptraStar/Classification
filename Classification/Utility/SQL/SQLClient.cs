using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace Classification.Utility.SQL
{
    public class SQLClient
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

        public void UpdateProperty(int propertyId, int conceptId, string name, string measure)
        {
            ExecuteProcedure(
                    "UpdateProperty",
                    new string[] { "@PropertyId", "@PropertyConceptRootId", "@Name", "@Measure" },
                    new object[]
                    { propertyId, conceptId, name, measure }
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

        public void DeleteProperty(int propertyId)
        {
            ExecuteProcedure(
                "DeleteProperty",
                new string[] { "@PropertyId" },
                new object[] { propertyId }
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
    }
}
