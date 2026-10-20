using HNSDotNetTraining.RESTApi.DataModels;
using HNSDotNetTraining.RESTApi.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace HNSDotNetTraining.RESTApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogAdoDotNetController : ControllerBase
    {
        private readonly string connectionString = "Data Source=.;Initial Catalog=HNSDotNetTraining;User ID=sa;Password=sasa@123;TrustServerCertificate=True";

        [HttpGet]
        public IActionResult GetBlogs()
        {
            List<BlogViewModel> blogs = new List<BlogViewModel>();

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = "select * from Tbl_Blog where DeleteFlag=0";
            SqlCommand cmd = new SqlCommand(query, connection);
            //SqlDataReader reader = cmd.ExecuteReader();
            //connection.Close();
            //while (reader.Read())
            //{
            //    blogs.Add(new BlogViewModel
            //    {
            //        Id = Convert.ToInt32(reader["BlogId"]),
            //        Title = Convert.ToString(reader["BlogTitle"]),
            //        Author = Convert.ToString(reader["BlogAuthor"]),
            //        Content = Convert.ToString(reader["BlogContent"])
            //    });
            //}

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            connection.Close();
            if (dt.Rows.Count == 0)
            {
                return NotFound();
            }
            foreach (DataRow item in dt.Rows)
            {
                blogs.Add(new BlogViewModel
                {
                    Id = Convert.ToInt32(item["BlogId"]),
                    Title = Convert.ToString(item["BlogTitle"]),
                    Author = Convert.ToString(item["BlogAuthor"]),
                    Content = Convert.ToString(item["BlogContent"]),
                    DeleteFlag = Convert.ToBoolean(item["DeleteFlag"])
                });
            }

            return Ok(blogs);
        }

        [HttpGet("{id}")]

        public IActionResult GetBlog(int id)
        {
            List<BlogViewModel> blog = new List<BlogViewModel>();

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = "select * from Tbl_Blog where DeleteFlag=0 and BlogId=@BlogId";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BlogId", id);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            connection.Close();

            if(dt.Rows.Count == 0)
            {
                return NotFound();
            }
            blog.Add(new BlogViewModel
            {
                Id = Convert.ToInt32(dt.Rows[0]["BlogId"]),
                Title = Convert.ToString(dt.Rows[0]["BlogTitle"]),
                Author = Convert.ToString(dt.Rows[0]["BlogAuthor"]),
                Content = Convert.ToString(dt.Rows[0]["BlogContent"]),
                DeleteFlag = Convert.ToBoolean(dt.Rows[0]["DeleteFlag"])
            });

            return Ok(blog);
        }

        [HttpPost]
        public IActionResult CrateBlog(BlogDataModel blog)
        {
            
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = @"INSERT INTO [dbo].[Tbl_Blog]
           ([BlogTitle]
           ,[BlogAuthor]
           ,[BlogContent]
           ,[DeleteFlag])
     VALUES
           (@BlogTitle
           ,@BlogAuthor
           ,@BlogContent
           ,@DeleteFlag)
";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BlogTitle", blog.BlogTitle);
            cmd.Parameters.AddWithValue("@BlogAuthor", blog.BlogAuthor);
            cmd.Parameters.AddWithValue("@BlogContent", blog.BlogContent);
            cmd.Parameters.AddWithValue("@DeleteFlag",0);

            int result =cmd.ExecuteNonQuery();
            connection.Close();
            if (result == 0)
            {
                return BadRequest("Failed!");
            }
            return Ok("Creating Successful");
        }

        [HttpPut("{id}")]

        public IActionResult UpdateBlog(int id,BlogViewModel blog)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = "select * from Tbl_Blog where DeleteFlag=0 and BlogId=@BlogId";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BlogId", id);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            connection.Close();

            if (dt.Rows.Count == 0)
            {
                return NotFound();
            }
            connection.Open();
            string UpdateQuery = @"UPDATE [dbo].[Tbl_Blog]
                           SET [BlogTitle] = @BlogTitle
                              ,[BlogAuthor] = @BlogAuthor
                              ,[BlogContent] = @BlogContent
                              ,[DeleteFlag] = @DeleteFlag
                         WHERE BlogId=@BlogId";
            SqlCommand command = new SqlCommand(UpdateQuery, connection);
            command.Parameters.AddWithValue("@BlogId", id);
            command.Parameters.AddWithValue("@BlogTitle", blog.Title);
            command.Parameters.AddWithValue("@BlogAuthor", blog.Author);
            command.Parameters.AddWithValue("@BlogContent", blog.Content);
            command.Parameters.AddWithValue("@DeleteFlag", 0);
            int result =cmd.ExecuteNonQuery();
            connection.Close();

            if(result == 0)
            {
                return NotFound("Failed!");
            }

            
            return Ok("Updated!");
        }

        [HttpPatch("{id}")]

        public IActionResult PatchBlog(int id, BlogViewModel blog)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = "select * from Tbl_Blog where DeleteFlag=0 and BlogId=@BlogId";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BlogId", id);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            connection.Close();

            if (dt.Rows.Count == 0)
            {
                return NotFound();
            }
            string condition = "";
            if (!blog.Title.IsNullOrEmpty())
            {
                condition += " [BlogTitle]=@BlogTitle, ";
            }
            if (!blog.Author.IsNullOrEmpty())
            {
                condition += " [BlogAuthor]=@BlogAuthor, ";
            }
            if (!blog.Content.IsNullOrEmpty())
            {
                condition += " [BlogContent]=@BlogContent, ";
            }
            
            if(condition.Length == 0)
            {
                return BadRequest("Failed");
            }

            condition = condition.Substring(0, condition.Length - 2);

           
            connection.Open();
            string PatchQuery = $@"UPDATE [dbo].[Tbl_Blog]
                           SET {condition}
                         WHERE BlogId=@BlogId";
            SqlCommand command =new SqlCommand(PatchQuery, connection);
            command.Parameters.AddWithValue("BlogId", id);
            if (!blog.Title.IsNullOrEmpty())
            {
                command.Parameters.AddWithValue("@BlogTitle",blog.Title);
            }
            if (!blog.Author.IsNullOrEmpty())
            {
                command.Parameters.AddWithValue("@BlogAuthor", blog.Author);
            }
            if (!blog.Content.IsNullOrEmpty())
            {
                command.Parameters.AddWithValue("@BlogContent", blog.Content);
            }
            command.ExecuteNonQuery();
            connection.Close();

            return Ok("Updating Successful!");
        }

        [HttpDelete("{id}")]

        public IActionResult DeleteBlog(int id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = "select * from Tbl_Blog where DeleteFlag=0 and BlogId=@BlogId";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BlogId", id);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            connection.Close();

            if (dt.Rows.Count == 0)
            {
                return NotFound();
            }
           
            connection.Open();
            string DeleteQuery = $@"UPDATE [dbo].[Tbl_Blog]
                               SET 
                                  [DeleteFlag] = {1}
                             WHERE BlogId=@BlogId";
            SqlCommand command = new SqlCommand(DeleteQuery, connection);
            command.Parameters.AddWithValue("@BlogId", id);
            int result=cmd.ExecuteNonQuery();
            connection.Close();
            if (result == 0)
            {
                return BadRequest("Failed!");
            }

            return Ok("Deleted!");
        }
        
    }
}
