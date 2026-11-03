using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens.Experimental;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HNSDotNetTraining.DIExamples
{
    public class AdoDotNet
    {
        //public DataTable Read()
        //{
        //    SqlConnection connection = new SqlConnection(AppSetting.connectionString);
        //    connection.Open();
        //    string query = "select * from Tbl_Blog";
        //    SqlCommand cmd = new SqlCommand(query, connection);
        //    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //    DataTable dt = new DataTable();
        //    adapter.Fill(dt);
        //    connection.Close();
        //    return dt;
        //}

        //public DataTable Edit(int id)
        //{
        //    SqlConnection connection =new SqlConnection(AppSetting.connectionString);
        //    connection.Open();
        //    string query = "select * from Tbl_Blog where BlogId=@BlogId";
        //    SqlCommand cmd = new SqlCommand(query, connection);
        //    cmd.Parameters.AddWithValue("@BlogId", id);
        //    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //    DataTable dt = new DataTable();
        //    adapter.Fill(dt);
        //    connection.Close();
        //    return dt;
        //}

        //public int Create(string blogTitle,string blogAuthor,string blogContent)
        //{
        //    SqlConnection connection = new SqlConnection(AppSetting.connectionString);
        //    connection.Open();
        //    String query = @"INSERT INTO [dbo].[Tbl_Blog]
        //                   ([BlogTitle]
        //                   ,[BlogAuthor]
        //                   ,[BlogContent]
        //                   ,[DeleteFlag])
        //             VALUES
        //                   (@BlogTitle
        //                   ,@BlogAuthor
        //                   ,@BlogContent
        //                   ,@DeleteFlag)";
        //    SqlCommand cmd = new SqlCommand(query, connection);
        //    cmd.Parameters.AddWithValue("@BlogTitle", blogTitle);
        //    cmd.Parameters.AddWithValue("@BlogAuthor", blogAuthor);
        //    cmd.Parameters.AddWithValue("@BlogContent", blogContent);
        //    cmd.Parameters.AddWithValue("@DeleteFlag", false);
        //    int result=cmd.ExecuteNonQuery();
        //    connection.Close();
        //    return result;
        //}

        //public int Update(int id,string? blogTitle=null,string? blogAuthor=null,string? blogContent=null)
        //{
        //    string condition = string.Empty;
        //    if (!string.IsNullOrEmpty(blogTitle))
        //    {
        //        condition += $"[BlogTitle]=@BlogTitle,";
        //    }
        //    if (!string.IsNullOrEmpty(blogAuthor))
        //    {
        //        condition += $"[BlogAuthor]=@BlogAuthor,";
        //    }
        //    if (!string.IsNullOrEmpty(blogContent))
        //    {
        //        condition += $"[BlogContent]=@BlogContent,";
        //    }

        //    if(condition.Length == 0)
        //    {
        //        Console.WriteLine("Invalid");
        //        return 0;
        //    }

        //    condition = condition.Substring(0, condition.Length - 1); 
            
        //    SqlConnection connection =new SqlConnection(AppSetting.connectionString);
        //    connection.Open();
        //    string query = $@"UPDATE [dbo].[Tbl_Blog]
        //                   SET {condition}
        //                 WHERE BlogId=@BlogId";
        //    SqlCommand cmd = new SqlCommand(query,connection);
        //    if (!string.IsNullOrEmpty(blogTitle))
        //    {
        //        cmd.Parameters.AddWithValue("@BlogTitle",blogTitle);
        //    }
        //    if (!string.IsNullOrEmpty(blogAuthor))
        //    {
        //        cmd.Parameters.AddWithValue("@BlogAuthor", blogAuthor);
        //    }
        //    if (!string.IsNullOrEmpty(blogContent))
        //    {
        //        cmd.Parameters.AddWithValue("@BlogContent", blogContent);
        //    }
        //    cmd.Parameters.AddWithValue("@BlogId", id);
        //    int result=cmd.ExecuteNonQuery();
        //    connection.Close();
        //    return result;
        //}

        //public int Delete(int id)
        //{
        //    SqlConnection connection = new SqlConnection(AppSetting.connectionString);
        //    connection.Open();
        //    string query=$@"UPDATE [dbo].[Tbl_Blog]
        //                   SET DeleteFlag=@DeleteFlag
        //                 WHERE BlogId=@BlogId";
        //    SqlCommand cmd = new SqlCommand(query,connection);
        //    cmd.Parameters.AddWithValue("@DeleteFlag", true);
        //    cmd.Parameters.AddWithValue("@BlogId", id);
        //    int result=cmd.ExecuteNonQuery();
        //    connection.Close();
        //    return result;
            
        //}

        public DataTable Query(string query,params SqlParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(AppSetting.connectionString);
            connection.Open();
            SqlCommand cmd=new SqlCommand(query,connection);
            if(parameters is not null)
            {
                foreach(var item in parameters)
                {
                    cmd.Parameters.AddWithValue(item.Name,item.Value);
                }
            }
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            connection.Close();
            return dt;
        }

        public int Execute(string query,params SqlParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(AppSetting.connectionString);
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            if( parameters is not null)
            {
                foreach(var item in parameters)
                {
                    cmd.Parameters.AddWithValue(item.Name, item.Value);
                }
            }
            int result=cmd.ExecuteNonQuery();
            connection.Close();
            return result;
        }
        
    }

    public class SqlParameter
    {
        public string Name;

        public object Value;

        public SqlParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }

}
