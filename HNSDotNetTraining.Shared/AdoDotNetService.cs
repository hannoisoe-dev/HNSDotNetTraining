using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace HNSDotNetTraining.Shared
{
    
    public class AdoDotNetService
    {
        private readonly string _connectionString;

        public AdoDotNetService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable Query(string query,params SqlParameter[] sqlParameters)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(query,connection);
            if (sqlParameters is not null)
            {
                foreach(SqlParameter item in sqlParameters)
                {
                    command.Parameters.AddWithValue(item.Name, item.Value);
                }
            }
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            connection.Close();
            return dt;
        }

        public int Execute(string query,params SqlParameter[] sqlParameters)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(query,connection);
            if(sqlParameters is not null)
            {
                foreach (var item in sqlParameters)
                {
                    command.Parameters.AddWithValue(item.Name, item.Value);
                }
            }
            int result= command.ExecuteNonQuery();
            connection.Close();
            return result;
        }

    }



    public class SqlParameter
    {
        public string Name;

        public object Value;

        public  SqlParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
