using Dapper;
using HNSDotNetTraining.Database.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HNSDotNetTraining.RestApiWithDI.Domain
{
    public class BlogDapper : IBlogDapper
    {
        private readonly IConfiguration _configuration;

        public BlogDapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<BlogDataModel> GetBlogs()
        {
            string query = "select * from Tbl_Blog where DeleteFlag=0";
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DBConnection"));
            var lst = db.Query<BlogDataModel>(query).ToList();
            return lst;
        }

        public BlogDataModel GetBlog(int id)
        {
            string query = "select * from Tbl_Blog where BlogId=@BlogId and DeleteFlag=0";
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DBConnection"));
            var result = db.Query<BlogDataModel>(query, new BlogDataModel
            {
                BlogId = id
            }).FirstOrDefault();
            return result;

        }

        public int CreateBlog(BlogDataModel blog)
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

            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DBConnection"));
            int result = db.Execute(query, blog);
            return result;
        }

        public int UpdateBlog(int id, BlogDataModel blog)
        {
            string selectQuery = "select * from Tbl_Blog where BlogId=@BlogId and DeleteFlag=0";
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DBConnection"));
            var result = db.Query<BlogDataModel>(selectQuery, new BlogDataModel
            {
                BlogId = id
            }).FirstOrDefault();

            if (result == null)
            {
                Console.WriteLine("Not Found!");
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

            condition = condition.Substring(0, condition.Length - 1);

            string query = $@"UPDATE [dbo].[Tbl_Blog]
                           SET {condition}
                              ,[DeleteFlag] = @DeleteFlag
                         WHERE BlogId=@BlogId";


            if (!string.IsNullOrWhiteSpace(blog.BlogTitle))
            {
                result.BlogTitle = blog.BlogTitle;
            }

            if (!string.IsNullOrWhiteSpace(blog.BlogAuthor))
            {
                result.BlogAuthor = blog.BlogAuthor;
            }

            if (!string.IsNullOrWhiteSpace(blog.BlogContent))
            {
                result.BlogContent = blog.BlogContent;
            }

            result.BlogId = id;

            int ans = db.Execute(query, result);
            return ans;

        }

        public int Delete(int id)
        {
            string deleteQuery = "update Tbl_Blog set DeleteFlag=1 where BlogId=@BlogId";
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DBConnection"));
            int result = db.Execute(deleteQuery, new BlogDataModel
            {
                BlogId = id
            });
            return result;
        }




    }


    public class BlogDataModel
    {
        public int BlogId { get; set; }
        public string? BlogTitle { get; set; }
        public string? BlogAuthor { get; set; }
        public string? BlogContent { get; set; }
        public bool DeleteFlag { get; set; }

    }


}
