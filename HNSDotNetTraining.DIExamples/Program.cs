// See https://aka.ms/new-console-template for more information
using HNSDotNetTraining.DIExamples;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Net.Http.Headers;
using static HNSDotNetTraining.ConsoleApp2.DapperExample2;

Console.WriteLine("Hello, World!");

//var services=new ServiceCollection().AddSingleton<AdoDotNet>().BuildServiceProvider();
//var adoDotNet=services.GetRequiredService<AdoDotNet>();
//var result = adoDotNet.Read();
//foreach (DataRow item in result.Rows)
//{
//    Console.WriteLine(item["BlogTitle"]);
//}

//DataTable dt=adoDotNet.Edit(2);
//foreach(DataRow dr in dt.Rows)
//{
//    Console.WriteLine(dr["BlogTitle"]);
//}

//int result=adoDotNet.Create("DI", "DI", "DI");
//if(result > 0)
//{
//    Console.WriteLine("Created");
//}

//var result=adoDotNet.Update(22,blogContent:"blog");
//if(result > 0)
//{
//    Console.WriteLine("Updated!");
//}

//int result=adoDotNet.Delete(22);
//if(result > 0)
//{
//    Console.WriteLine("Deleted!");
//}

var services=new ServiceCollection().AddSingleton<AdoDotNet>().BuildServiceProvider();
var adoDotNet=services.GetRequiredService<AdoDotNet>();
//var result=adoDotNet.Query("select * from Tbl_Blog");
//foreach(DataRow dr in result.Rows)
//{
//    Console.WriteLine(dr["BlogTitle"]);
//}

//var result=adoDotNet.Query("select * from Tbl_Blog where BlogId=@BlogId", 
//    new SqlParameter("@BlogId", 3));
//foreach(DataRow dr in result.Rows)
//{
//    Console.WriteLine(dr["BlogTitle"]);
//}

//int result=adoDotNet.Execute(@"INSERT INTO [dbo].[Tbl_Blog]
//                           ([BlogTitle]
//                          ,[BlogAuthor]
//                           ,[BlogContent]
//                           ,[DeleteFlag])
//                    VALUES
//                           (@BlogTitle
//                          ,@BlogAuthor
//                           ,@BlogContent
//                           ,@DeleteFlag)",
//                           new SqlParameter("@BlogTitle", "Tuesday"),
//                           new SqlParameter("@BlogAuthor", "hns"),
//                           new SqlParameter("@BlogContent", "Tuesday"),
//                           new SqlParameter("DeleteFlag", false)

//                           );
//if(result > 0)
//{
//    Console.WriteLine("Created!");
//}

//int result =adoDotNet.Execute("update Tbl_Blog set BlogTitle=@BlogTitle,BlogAuthor=@BlogAuthor where BlogId=@BlogId",
//    new SqlParameter("@BlogTitle","Update Tuesday"),
//    new SqlParameter("@BlogAuthor","HNS"),
//    new SqlParameter("@BlogId",23)
//    );

//if(result > 0)
//{
//    Console.WriteLine("Updated!");
//}


//int result=adoDotNet.Execute("update Tbl_Blog set DeleteFlag=@DeleteFlag where BlogId=@BlogId",
//    new SqlParameter("@DeleteFlag", true),
//    new SqlParameter("@BlogId", 23)
//    );
//if(result > 0)
//{
//    Console.WriteLine("Deleted!");
//}

var service=new ServiceCollection().AddSingleton<DapperService>().BuildServiceProvider();
var dapperService=service.GetRequiredService<DapperService>();
//var result=dapperService.Query<BlogDataModel>("select * from Tbl_Blog");
//foreach(var item in result)
//{
//    Console.WriteLine(item.BlogTitle);
//}

//var result=dapperService.QueryFirstOrDefault<BlogDataModel>("select * from Tbl_Blog where BlogId=@BlogId",
//   new BlogDataModel
//   {
//       BlogId =23
//   });
//Console.WriteLine(result.BlogTitle);

//int result=dapperService.Execute(@"INSERT INTO [dbo].[Tbl_Blog]
//           ([BlogTitle]
//           ,[BlogAuthor]
//           ,[BlogContent]
//           ,[DeleteFlag])
//     VALUES
//           (@BlogTitle
//           ,@BlogAuthor
//           ,@BlogContent
//           ,@DeleteFlag)",
//           new BlogDataModel
//           {
//               BlogTitle = "New Title",
//               BlogAuthor = "HHH",
//               BlogContent = "New Content",
//               DeleteFlag = false
//           }
//           );
//if(result > 0)
//{
//    Console.WriteLine("Created!");
//}

//int result=dapperService.Execute(@"UPDATE [dbo].[Tbl_Blog]
//   SET [BlogTitle] = @BlogTitle
// WHERE BlogId=@BlogId",
// new BlogDataModel
// {
//     BlogId = 24,
//     BlogTitle = "Newer"
// }
// );
//if (result > 0)
//{
//    Console.WriteLine("Updated!");
//}


int result=dapperService.Execute(@"UPDATE [dbo].[Tbl_Blog]
   SET [DeleteFlag] = @DeleteFlag  WHERE BlogId=@BlogId",
   new BlogDataModel
   {
       BlogId = 24,
       DeleteFlag = true
   }
   );

string message = result > 0 ? "Deleted!" : "Delete Failed!";
Console.WriteLine(message);



