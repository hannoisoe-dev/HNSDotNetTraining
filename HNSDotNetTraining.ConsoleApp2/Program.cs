// See https://aka.ms/new-console-template for more information
using HNSDotNetTraining.Database.Models;

Console.WriteLine("Hello, World!");

AppDbContext db = new AppDbContext();
var result=db.TblBlogs.ToList();

foreach(var item in result)
{
    Console.WriteLine(item.BlogAuthor);
}