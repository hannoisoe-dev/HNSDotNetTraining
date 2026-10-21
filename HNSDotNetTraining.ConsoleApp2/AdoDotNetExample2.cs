using HNSDotNetTraining.Shared;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using SqlParameter = HNSDotNetTraining.Shared.SqlParameter;

namespace HNSDotNetTraining.ConsoleApp
{
    public class AdoDotNetExample2
    {
        private readonly string _connectionString = "Data Source=.;Initial Catalog=HNSDotNetTraining;User ID=sa;Password=sasa@123;TrustServerCertificate=True";
        private readonly AdoDotNetService _adoDotNetService;
        public AdoDotNetExample2()
        {
            _adoDotNetService = new AdoDotNetService(_connectionString);
        }

        public void Read()
        {
            string query = "select * from Tbl_Blog where DeleteFlag=0";
            var lst=_adoDotNetService.Query(query);
            foreach(DataRow item in lst.Rows)
            {
                Console.WriteLine(item["BlogId"]);
                Console.WriteLine(item["BlogTitle"]);
                Console.WriteLine(item["BlogAuthor"]);
                Console.WriteLine(item["BlogContent"]);
            }
        }

        public void ReadById(int id)
        {
            string query = "select * from Tbl_Blog where BlogId=@BlogId and DeleteFlag=0";

            var lst=_adoDotNetService.Query(query, new SqlParameter("@BlogId", id));
            if(lst.Rows.Count ==0)
            {
                Console.WriteLine("Not Found!");
                return;
            }
            foreach(DataRow item in lst.Rows)
            {
                Console.WriteLine(item["BlogId"]);
                Console.WriteLine(item["BlogTitle"]);
                Console.WriteLine(item["BlogAuthor"]);
                Console.WriteLine(item["BlogContent"]);
            }

        }

        public void Create(params SqlParameter[] parameters)
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
                           ,0)";

            int result=_adoDotNetService.Execute(query,
                new SqlParameter("@BlogTitle", title),
                new SqlParameter("@BlogAuthor", author),
                new SqlParameter("@BlogContent", content));
            if(result == 0)
            {
                Console.WriteLine("Creating Fail!");
                return;
            }
            Console.WriteLine("Creating Successful");
                
        }

        public void Update(params SqlParameter[] parameters)
        {
            Console.Write("Enter Blog Id:");
            int Blogid = Convert.ToInt32(Console.ReadLine());

            string query = "select * from Tbl_Blog where BlogId=@BlogId and DeleteFlag=0";

            var lst = _adoDotNetService.Query(query, new SqlParameter("@BlogId", Blogid));

            if(lst.Rows.Count ==0 )
            {
                Console.WriteLine("Not found!");
                return;
            }

            Console.Write("Enter Blog Title:");
            string title = Console.ReadLine()!;

            Console.Write("Enter Blog Author:");
            string author = Console.ReadLine()!;

            Console.Write("Enter Blog Content:");
            string content = Console.ReadLine()!;

            List<SqlParameter> list = new List<SqlParameter>();
            string condition = "";

            if (!title.IsNullOrEmpty())
            {
                condition += " [BlogTitle]=@BlogTitle, ";
                
            }
            if (!author.IsNullOrEmpty())
            {
                condition += " [BlogAuthor]=@BlogAuthor, ";
                
            }
            if (!content.IsNullOrEmpty())
            {
                condition += " [BlogContent]=@BlogContent, ";
                
            }
            if(condition.Length ==0)
            {
                Console.WriteLine("Invalid input!");
                return;
            }
            condition = condition.Substring(0, condition.Length - 2);

            string Updatequery = $@"UPDATE [dbo].[Tbl_Blog]
                           SET {condition}
                              ,[DeleteFlag] = 0
                         WHERE BlogId=@BlogId";

            list.Add(new SqlParameter("BlogId", Blogid));

            if (!title.IsNullOrEmpty())
            {
               list.Add(new SqlParameter("@BlogTitle", title));
            }
            if (!author.IsNullOrEmpty())
            {
                list.Add(new SqlParameter("@BlogAuthor", author));
            }
            if (!content.IsNullOrEmpty())
            {
                list.Add(new SqlParameter("@BlogContent", content));
            }

            int result=_adoDotNetService.Execute(Updatequery,list.ToArray());

            if (result == 0) 
            {
                Console.WriteLine("Updating Fail!");
                return;
            }
            Console.WriteLine("Updating Successful!");

        }

        public void DeleteBlog()
        {
            Console.Write("Enter Blog Id:");
            string id=Console.ReadLine()!;

            string query = "select * from Tbl_blog where BlogId=@BlogId and DeleteFlag=0";
            var lst=_adoDotNetService.Query(query, new SqlParameter("@BlogId", id));
            if(lst.Rows.Count ==0)
            {
                Console.WriteLine("Not Found!");
                return;
            }

            string deleteQuery = @"UPDATE [dbo].[Tbl_Blog]
                                   SET [DeleteFlag] = 1
                                 WHERE BlogId=@BlogId";

            int result=_adoDotNetService.Execute(deleteQuery, new SqlParameter("@BlogId", id));
            if(result == 0)
            {
                Console.WriteLine("Delete Failed");
                return;
            }
            Console.WriteLine("Deleting Successful");
        }

        

    }
}
