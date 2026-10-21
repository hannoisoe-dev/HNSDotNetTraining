using Dapper;
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
    public class BlogDapperController : ControllerBase
    {
        private readonly string connectionString = "Data Source=.;Initial Catalog=HNSDotNetTraining;User ID=sa;Password=sasa@123;TrustServerCertificate=true";

        [HttpGet]
        public IActionResult GetBlogs()
        {
            List<BlogViewModel> blogs = new List<BlogViewModel>();

            using (IDbConnection db= new SqlConnection(connectionString))
            {
                string query = "select * from Tbl_Blog where DeleteFlag=0";
                var lst = db.Query<BlogDataModel>(query).ToList();
                if(lst is null)
                {
                    return NotFound();
                }
                foreach(var item in lst)
                {
                    blogs.Add(new BlogViewModel
                    {
                        Id = item.BlogId,
                        Title = item.BlogTitle,
                        Author = item.BlogAuthor,
                        Content = item.BlogContent,
                    });
                }

                return Ok(blogs);
            }
           
        }

        [HttpGet("{id}")]

        public IActionResult GetBlog(int id)
        {
            List<BlogViewModel> blog = new List<BlogViewModel>();
             using (IDbConnection db =new SqlConnection(connectionString))
            {
                string query = "select * from Tbl_Blog where DeleteFlag=0 and BlogId=@BlogId and DeleteFlag=0";
                var lst = db.Query<BlogDataModel>(query,
                    new BlogDataModel
                    {
                        BlogId = id
                    }).FirstOrDefault();
                if(lst is null)
                {
                    return NotFound();
                }
                blog.Add(new BlogViewModel
                {
                    Id = lst.BlogId,
                    Title = lst.BlogTitle,
                    Author = lst.BlogAuthor,
                    Content = lst.BlogContent,
                });
                return Ok(blog);
            }
            
        }

        [HttpPost]

        public IActionResult CreateBlog(BlogViewModel blog)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
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
               int result= db.Execute(query, new BlogDataModel
                {
                    BlogTitle = blog.Title,
                    BlogAuthor = blog.Author,
                    BlogContent = blog.Content
                });

                if(result == 0)
                {
                    return NotFound();
                }
                return Ok("Updated!");
            }
                
        }
        [HttpPut("{id}")]

        public IActionResult UpdateBlog(int id,BlogViewModel blog)
        {
            string query = @"UPDATE [dbo].[Tbl_Blog]
                           SET [BlogTitle] = @BlogTitle
                              ,[BlogAuthor] = @BlogAuthor
                              ,[BlogContent] = @BlogContent
                              ,[DeleteFlag] = @DeleteFlag
                         WHERE BlogId=@BlogId and DeleteFlag=0";
           using(IDbConnection db=new SqlConnection(connectionString))
            {
                int result=db.Execute(query, new BlogDataModel
                {
                    BlogTitle = blog.Title,
                    BlogAuthor =  blog.Author,
                    BlogContent = blog.Content,
                    BlogId = id
                });
                if(result == 0)
                {
                    return BadRequest();
                }
                return Ok("Updated!");
            }
        }

        [HttpPatch("{id}")]

        public IActionResult PatchBlog(int id,BlogViewModel blog)
        {
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
            if (condition.Length == 0)
            {
                return BadRequest();
            }
            condition = condition.Substring(0, condition.Length - 2);
            string query = $@"UPDATE [dbo].[Tbl_Blog]
                           SET {condition}
                         WHERE BlogId=@BlogId and DeleteFlag=0";
             using(IDbConnection db= new SqlConnection(connectionString))
            {
               BlogDataModel model= new BlogDataModel();
                
                if (!blog.Title.IsNullOrEmpty())
                {
                    model.BlogTitle = blog.Title;
                }
                if(!blog.Author.IsNullOrEmpty())
                {
                    model.BlogAuthor = blog.Author;
                }
                if (!blog.Content.IsNullOrEmpty())
                {
                    model.BlogContent = blog.Content;
                }

                model.BlogId = id;

               
                int result=db.Execute(query, model);
                if(result == 0)
                {
                    return BadRequest();
                }
                return Ok("Updated!");
            }
        }

        [HttpDelete("{id}")]

        public IActionResult DeleteBlog(int id)
        {
            string query = "UPDATE [dbo].[Tbl_Blog] set [DeleteFlag]=1 where [BlogId]=@BlogId and DeleteFlag=0";
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                int result=db.Execute(query, new BlogDataModel
                {
                    BlogId = id
                });
                if(result == 0)
                {
                    return BadRequest();
                }
                return Ok("Deleted!");
            }
                
        }
    }
}
