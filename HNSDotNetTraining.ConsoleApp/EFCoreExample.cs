using HNSDotNetTraining.ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNSDotNetTraining.ConsoleApp
{
    public class EFCoreExample
    {
       public void Read()
        {
            AppDBContext db = new AppDBContext();
            var result=db.Blogs.ToList();
            foreach(var item in result)
            {
                Console.WriteLine(item.BlogId);
                Console.WriteLine(item.BlogTitle);
                Console.WriteLine(item.BlogAuthor);
                Console.WriteLine(item.BlogContent);
            }
           
        }

        public void Create(string blogTitle,string blogAuthor, string blogContent)
        {
            AppDBContext db = new AppDBContext();
            db.Add(new BlogDataModel
            {
                BlogTitle = blogTitle,
                BlogAuthor = blogAuthor,
                BlogContent = blogContent,

            });
            int result=db.SaveChanges();

            string message = result > 0 ? "Saving Successful" : "Saving Failed";
            Console.WriteLine(message);
        }

        public void Edit(int blogId)
        {
            bool deleteFlag = false;
            AppDBContext db = new AppDBContext();
            var result=db.Blogs.FirstOrDefault(x => x.BlogId == blogId && x.DeleteFlag == deleteFlag);

            if(result is null)
            {
                Console.WriteLine("Data not found!");
                return;
            }

            Console.WriteLine(result.BlogId);
            Console.WriteLine(result.BlogTitle);
            Console.WriteLine(result.BlogAuthor);
            Console.WriteLine(result.BlogContent);

        }

        public void Update(int blogId, string blogTitle, string blogAuthor, string blogContent)
        {
            bool deleteFlag = false;
            AppDBContext db = new AppDBContext();
            var result = db.Blogs.AsNoTracking().FirstOrDefault(x => x.BlogId == blogId && x.DeleteFlag == deleteFlag);

            if (result is null)
            {
                Console.WriteLine("Data not found!");
                return;
            }

            result.BlogTitle = blogTitle;
            result.BlogAuthor = blogAuthor;
            result.BlogContent = blogContent;

            db.Entry(result).State = EntityState.Modified;
            int result1 =db.SaveChanges();

            string message = result1 > 0 ? "Update Successful" : "Update Failed!";
            Console.WriteLine(message);


        }

        public void Delete(int blogId)
        {
            bool deleteflag = false;
            AppDBContext db = new AppDBContext();
            var result=db.Blogs.FirstOrDefault(x => x.BlogId == blogId && x.DeleteFlag == deleteflag);
            
            if(result is null)
            {
                Console.WriteLine("No Data Found!");
                return;
            }

           db.Remove(result);
           int result1=db.SaveChanges();

            string message = result1 > 0 ? "Delete Successful" : "Delete Failed";

            Console.WriteLine(message);

        }


        
    }
}
