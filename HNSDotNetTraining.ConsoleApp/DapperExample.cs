using Dapper;
using HNSDotNetTraining.ConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNSDotNetTraining.ConsoleApp
{
    internal class DapperExample
    {
        private readonly string _sqlConnection = "Data Source=.;initial Catalog=HNSDotNetTraining;User ID=sa;Password=sasa@123;";

        public void Read()
        {
            //using (IDbConnection db = new SqlConnection(_sqlConnection))
            //{
            //    string query = "select * from Tbl_Blog where DeleteFlag = 0";
            //    var result = db.Query(query).ToList();
            //    foreach (var item in result)
            //    {
            //        Console.WriteLine(item.BlogId);
            //        Console.WriteLine(item.BlogTitle);
            //        Console.WriteLine(item.BlogContent);
            //    }
            //}

            using (IDbConnection db = new SqlConnection(_sqlConnection))
            {
                string query = "select * from Tbl_Blog where DeleteFlag =0";
                var result=db.Query<BlogDapperDataModel>(query).ToList();
                foreach(var item in result)
                {
                    Console.WriteLine(item.BlogId);
                    Console.WriteLine(item.BlogTitle);
                    Console.WriteLine(item.BlogContent);
                }
            }

        }

        public void Create(string blogTitle, string blogAuthor, string blogContent)
        {
            using (IDbConnection db = new SqlConnection(_sqlConnection))
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

                int result = db.Execute(query, new BlogDapperDataModel
                {
                    BlogTitle = blogTitle,
                    BlogAuthor = blogAuthor,
                    BlogContent = blogContent,
                    
                });

                string message = result > 0 ? "Saving Successful" : "Saving Failed";
                Console.WriteLine(message);
            }
        }

        public void Edit(int blogId)
        {
            using (IDbConnection db = new SqlConnection(_sqlConnection))
            {
                string query = "select * from Tbl_Blog where BlogId=@BlogId and DeleteFlag=0";
               var result= db.Query<BlogDapperDataModel>(query, new BlogDapperDataModel
                {
                    BlogId = blogId,
                    
                }).FirstOrDefault();

                if(result is null)
                {
                    Console.WriteLine("No data found!");
                    return;
                }

                   Console.WriteLine(result.BlogId);
                   Console.WriteLine(result.BlogTitle);
                   Console.WriteLine(result.BlogContent);
               
            }
        }

        public void Update(int blogId, string blogTitle, string blogAuthor, string blogContent)
        {
            using (IDbConnection db = new SqlConnection(_sqlConnection))
            {
                string query = "select * from Tbl_Blog where BlogId=@BlogId and DeleteFlag=0";
                var result = db.Query<BlogDapperDataModel>(query, new BlogDapperDataModel
                {
                    BlogId = blogId
                }).FirstOrDefault();

                if (result is null)
                {
                    Console.WriteLine("No data found!");
                    return;
                }

               
                string updateQuery = @"UPDATE [dbo].[Tbl_Blog]
                               SET [BlogTitle] = @BlogTitle
                                   ,[BlogAuthor] = @BlogAuthor
                                  ,[BlogContent] = @BlogContent
     
                             WHERE [BlogId] = @BlogId and [DeleteFlag]= 0";

               int result1= db.Execute(updateQuery, new BlogDapperDataModel
                {
                    BlogId = blogId,
                    BlogTitle = blogTitle,
                    BlogAuthor = blogAuthor,
                    BlogContent = blogContent
                });

                string message = result1 > 0 ? "Update Successful" : "Update Failed";
                Console.WriteLine(message);
            }
        }

        public void Delete(int blogId)
        {
            using (IDbConnection db = new SqlConnection(_sqlConnection))
            {
                string query = "select * from Tbl_Blog where BlogId=@BlogId and DeleteFlag=0";
                var result = db.Query<BlogDapperDataModel>(query, new BlogDapperDataModel
                {
                    BlogId = blogId
                }).FirstOrDefault();

                if (result is null)
                {
                    Console.WriteLine("No data found!");
                    return;
                }

                string deleteQuery = @"UPDATE [dbo].[Tbl_Blog]
                               SET [DeleteFlag] = 1
                             WHERE [BlogId] = @BlogId and [DeleteFlag]=0";

                int result1 =db.Execute(deleteQuery, new BlogDapperDataModel
                {
                    BlogId = blogId,
                });
                string message = result1 > 0 ? "Delete Successful!" : "Delete Failed!";
                Console.WriteLine(message);
            }
        }
    }
}
