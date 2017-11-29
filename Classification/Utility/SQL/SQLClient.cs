using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            Connect();
        }

        public void Connect()
        {
            Connection = new SqlConnection(ConnectionString);
        }

        public void Select(DataTable dataTable, ref SqlDataAdapter adapter, string[] tables, string[] attributes)
        {
            string all_tables = string.Join(", ", tables);
            string all_attributes = string.Join(", ", attributes);

            string sql = "SELECT " + all_attributes + " FROM " + all_tables + ";";

            SqlCommand sqlCommand = new SqlCommand(sql, Connection);
            adapter = new SqlDataAdapter(sqlCommand);

            try
            {
                Connection.Open();
                adapter.Fill(dataTable);
            }
            finally
            {
                Connection?.Close();
            }
        }
        public void Select(DataTable dataTable, ref SqlDataAdapter adapter, string sqlQuery)
        {
            SqlCommand sqlCommand = new SqlCommand(sqlQuery, Connection);
            adapter = new SqlDataAdapter(sqlCommand);

            try
            {
                Connection.Open();
                adapter.Fill(dataTable);
            }
            finally
            {
                Connection?.Close();
            }
        }

        public object SelectScalar(string sqlQuery)
        {
            SqlCommand sqlCommand = new SqlCommand(sqlQuery, Connection);

            try
            {
                Connection.Open();
                return sqlCommand.ExecuteScalar();
            }
            finally
            {
                Connection?.Close();
            }
        }

        public void Insert(string query, string[] values, string[] args)
        {
            query += " VALUES (" + String.Join(",", values) + ")";

            using (SqlCommand command = new SqlCommand(query, Connection))
            {
                for (int i = 0; i < values.Length; i++)
                {
                    command.Parameters.AddWithValue(values[i], args[i]);
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
        public void ExecuteProcedureVoid(string procedure, string[] parameters, object[] values)
        {
            using (SqlCommand command = new SqlCommand(procedure, Connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                for(int i = 0; i < parameters.Length; i++)
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
        public object ExecuteProcdureScalar(string procedure, string[] parameters, object[] values)
        {
            if (parameters?.Length == 0)
                throw new IndexOutOfRangeException("parameters is empty");

            if (values?.Length == 0)
                throw new IndexOutOfRangeException("values is empty");

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

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.TableMappings.Add("Classification", "table");
                    adapter.SelectCommand = command;

                    DataSet dataSet = new DataSet("table");
                    adapter.Fill(dataSet);

                    return dataSet.Tables["table"];
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

        public void Update(DataTable table, SqlDataAdapter adapter)
        {
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(adapter);
            adapter.Update(table);
        }
    }
}
