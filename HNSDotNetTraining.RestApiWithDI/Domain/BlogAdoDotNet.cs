using HNSDotNetTraining.RestApiWithDI.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HNSDotNetTraining.RestApiWithDI.Domain
{
    public class BlogAdoDotNet : IBlogAdoDotNet
    {
        private readonly string _connection;

        public BlogAdoDotNet(IConfiguration conncection)
        {
            _connection = conncection.GetConnectionString("DBConnection")!;
        }

        public DataTable GetBlogs()
        {
            string query = "select * from Tbl_Blog where DeleteFlag=0";
            SqlConnection connection = new SqlConnection(_connection);
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            connection.Close();
            return dt;

        }

        public DataTable GetBlog(int id)
        {
            string query = "select * from Tbl_Blog where DeleteFlag=0 and BlogId=@BlogId";
            SqlConnection connection = new SqlConnection(_connection);
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            connection.Close();
            return dt;
        }

        public int Create(BlogDataModel blog)
        {
            string query = @"INSERT INTO [dbo].[Tbl_Blog]
                               ([BlogTitle]
                               ,[BlogAuthor]
                               ,[BlogContent]
                               ,[DeleteFlag])
                         VALUES
                               (@BlogTitle
                               ,@BlogAuthor
                               ,@BlogContent
                               ,@DeleteFlag)";

            SqlConnection connection = new SqlConnection(_connection);
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            int result = adapter.Fill(dt);
            return result;
        }

        public int Update(int id, BlogDataModel blog)
        {
            string selectQuery = "select * from Tbl_Blog where DeleteFlag=0 and BlogId=@BlogId";
            SqlConnection connection = new SqlConnection(_connection);
            connection.Open();
            SqlCommand cmd = new SqlCommand(selectQuery, connection);
            cmd.Parameters.AddWithValue("@BlogId", id);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            int result = adapter.Fill(dt);
            if (result == 0)
            {
                return 0;
            }

            string condition = string.Empty;

            if (!string.IsNullOrWhiteSpace(blog.BlogTitle))
            {
                condition += "[BlogTitle]= @BlogTitle,";
            }

            if (!string.IsNullOrWhiteSpace(blog.BlogAuthor))
            {
                condition += "[BlogAuthor]=@BlogAuthor,";
            }

            if (!string.IsNullOrWhiteSpace(blog.BlogContent))
            {
                condition += "[BlogContent]=@BlogContent,";
            }

            if (condition.Length == 0)
            {
                return 0;
            }

            condition = condition.Substring(0, condition.Length - 1);

            string query = $@"UPDATE [dbo].[Tbl_Blog]
                           SET {condition}
                              ,[DeleteFlag] = @DeleteFlag
                         WHERE BlogId=@BlogId";

            SqlCommand command = new SqlCommand(query, connection);

            if (!string.IsNullOrWhiteSpace(blog.BlogTitle))
            {
                command.Parameters.AddWithValue("@BlogTitle", blog.BlogTitle);
            }

            if (!string.IsNullOrWhiteSpace(blog.BlogAuthor))
            {
                command.Parameters.AddWithValue("@BlogAuthor", blog.BlogAuthor);
            }

            if (!string.IsNullOrWhiteSpace(blog.BlogContent))
            {
                command.Parameters.AddWithValue("@BlogContent", blog.BlogContent);
            }

            command.Parameters.AddWithValue("@BlogId", id);

            int ans = command.ExecuteNonQuery();
            connection.Close();
            return ans;
        }

        public int Delete(int id)
        {
            string query = "update Tbl_Blog set DeleteFlag=1 where BlogId=@BlogId";
            SqlConnection connection = new SqlConnection(_connection);
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BlogId", id);
            int result = cmd.ExecuteNonQuery();
            connection.Close();
            return result;
        }
    }
}
