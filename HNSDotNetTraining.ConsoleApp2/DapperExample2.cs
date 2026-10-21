using HNSDotNetTraining.Shared;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HNSDotNetTraining.ConsoleApp2
{
    public class DapperExample2
    {
        private readonly string connectionString = "Data Source=.;Initial Catalog=HNSDotNetTraining;User ID=sa;Password=sasa@123;TrustServerCertificate=True";
        private readonly DapperService _dapperService;

        public DapperExample2()
        {
            _dapperService = new DapperService(connectionString);
        }

        public class Blog
        {
            public int BlogId { get; set; }

            public string BlogTitle { get; set; }

            public string BlogAuthor { get; set; }

            public string BlogContent { get; set; }

            public bool DeleteFlag { get; set; }
        }

        public void Read()
        {
            string query = "select * from Tbl_Blog where  DeleteFlag=0";
            List<Blog> lst=_dapperService.Query<Blog>(query);
            if (lst is null)
            {
                Console.WriteLine("Not found!");
                return;
            }
            foreach(var item in lst)
            {
                Console.WriteLine(item.BlogId);
                Console.WriteLine(item.BlogTitle);
                Console.WriteLine(item.BlogAuthor);
                Console.WriteLine(item.BlogContent);
            }

        }

        public void ReadById()
        {
            Console.Write("Enter Blog Id:");
            int id = Convert.ToInt32(Console.ReadLine())!;
            string query = "select * from Tbl_Blog where BlogId=@BlogId and DeleteFlag=0";
            var lst=_dapperService.Query<Blog>(query, new Blog
            {
                BlogId = id
            });
            if(lst.Count == 0)
            {
                Console.WriteLine("Not Found!");
                return;
            }
            foreach(var item in lst)
            {
                Console.WriteLine(item.BlogId);
                Console.WriteLine(item.BlogTitle);
                Console.WriteLine(item.BlogAuthor);
                Console.WriteLine(item.BlogContent);
            }

        }

        public void CreateBlog()
        {
            Console.Write("Enter Blog Title: ");
            string title = Console.ReadLine()!;

            Console.Write("Enter Blog Author: ");
            string author = Console.ReadLine()!;

            Console.Write("Enter Blog Content: ");
            string content = Console.ReadLine()!;

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
            int result=_dapperService.Execute(query, new Blog
            {
                BlogTitle = title,
                BlogAuthor = author,
                BlogContent = content,
                DeleteFlag = false

            });
            string message = result > 0 ? "Creating Successful!" : "Creating Fail";
            Console.WriteLine(message);
        }

        public void UpdateBlog()
        {
            string condition = "";

            Console.Write("Enter Blog Id:");
            int id = Convert.ToInt32(Console.ReadLine());

            string query = "select * from Tbl_Blog where BlogId=@BlogId and DeleteFlag=0";

           int result= _dapperService.Execute(query,new Blog
            {
                BlogId = id
            });

            if(result ==0)
            {
                Console.WriteLine("Not Found!");
                return;
            }


            Console.Write("Enter Blog Title:");
            string title = Console.ReadLine()!;

            Console.Write("Enter Blog Author:");
            string author = Console.ReadLine()!;

            Console.Write("Enter Blog Content:");
            string content = Console.ReadLine()!;

            if (!title.IsNullOrEmpty())
            {
                condition += "[BlogId]= @BlogId";
            }
            if (!author.IsNullOrEmpty())
            {
                condition += "[BlogAuthor]= @BlogAuthor";
            }
            if (!content.IsNullOrEmpty())
            {
                condition += "[BlogContent]= @BlogContent";
            }
            if(condition.Length == 0)
            {
                Console.WriteLine("Invalid input!");
            }


            string updateQuery = $@"UPDATE [dbo].[Tbl_Blog]
                           SET {condition}
                              ,[DeleteFlag] = 0
                         WHERE BlogId=@BlogId and DeleteFlag=0";

           int res= _dapperService.Execute(updateQuery, new Blog
            {
                BlogId = id,
                BlogAuthor = author,
                BlogContent = content

            });
            string message = res == 0 ? "Updating Fail." : "Updating Successful";
            Console.WriteLine(message);
        }

        public void DeleteBlog()
        {
            string updateQuery = @"UPDATE [dbo].[Tbl_Blog]
                           SET [DeleteFlag] = 1
                         WHERE BlogId=@BlogId and DeleteFlag=0";

            Console.Write("Enter Blog Id:");
            int id = Convert.ToInt32(Console.ReadLine());

            string query = "select * from Tbl_Blog where BlogId=@BlogId and DeleteFlag=0";
            var lst = _dapperService.Query<Blog>(query, new Blog
            {
                BlogId = id
            });
            if(lst.Count == 0)
            {
                Console.WriteLine("Not Found!");
                return;
            }

            int res=_dapperService.Execute(updateQuery, new Blog
            {
                BlogId = id
            });

            string message = res > 0 ? "Deleting Successful" : "Deleting Fail.";
            Console.WriteLine(message);

        }

    }
}
