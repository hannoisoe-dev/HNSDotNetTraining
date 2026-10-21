// See https://aka.ms/new-console-template for more information
using HNSDotNetTraining.ConsoleApp;
using HNSDotNetTraining.ConsoleApp2;
using HNSDotNetTraining.Database.Models;
using HNSDotNetTraining.Shared;
using System.Numerics;

//Console.WriteLine("Hello, World!");

//AppDbContext db = new AppDbContext();
//var result=db.TblBlogs.ToList();

//foreach(var item in result)
//{
//    Console.WriteLine(item.BlogAuthor);
//}

//AdoDotNetExample2 adoDotNetExample2 = new AdoDotNetExample2();
//adoDotNetExample2.Read();
//Console.Write("Enter Blog Id:");
//bool isInt = int.TryParse(Console.ReadLine(), out int result);
//if (!isInt)
//{
//    Console.WriteLine("Invalid Id!");
//    return;
//}
//adoDotNetExample2.ReadById(result);

//adoDotNetExample2.Create();
//adoDotNetExample2.Update();
//adoDotNetExample2.DeleteBlog();

DapperExample2 dp = new DapperExample2();
dp.Read();
//dp.ReadById();
//dp.UpdateBlog();
//dp.Create();
//dp.DeleteBlog();