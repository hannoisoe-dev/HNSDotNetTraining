using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNSDotNetTraining.DIExamples
{
    public class DapperService
    {
        public List<T> Query<T>(string query)
        {
            using(IDbConnection connection=new SqlConnection(AppSetting.connectionString))
            {
               var result= connection.Query<T>(query).ToList();
                return result;
            }
        }

        public T QueryFirstOrDefault<T>(string query,object? param)
        {
            using (IDbConnection connection = new SqlConnection(AppSetting.connectionString))
            {
                var result = connection.QueryFirstOrDefault<T>(query, param);
                return result;
            }
        }

        public int Execute(string query,object? param)
        {
            using (IDbConnection connection=new SqlConnection(AppSetting.connectionString))
            {
                int result=connection.Execute(query, param);
                return result;
            }
        }
    }

  
}
