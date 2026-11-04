using HNSDotNetTraining.DIExamples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNSDotNetTraining.DIExamples
{
    public class EfCoreService
    {
        private readonly AppDbContext _appDbContext;

        public EfCoreService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public List<TblBlog> Query()
        {
            var result=_appDbContext.TblBlogs.ToList();
            return result;
        }

        public TblBlog QueryFirstOrDefault(int id)
        {
            var result=_appDbContext.TblBlogs.FirstOrDefault(x => x.BlogId == id);
            return result;
        }

        public void Update(int id,TblBlog tblBlog)
        {
           var result=_appDbContext.TblBlogs.FirstOrDefault(x => x.BlogId == id);
            if (result is null)
            {
                Console.WriteLine("Fail updated!");
                return;
            }
            if(result is not null)
            {
                if (!string.IsNullOrWhiteSpace(tblBlog.BlogTitle))
                {
                    result.BlogTitle = tblBlog.BlogTitle;
                }
                if (!string.IsNullOrWhiteSpace(tblBlog.BlogAuthor))
                {
                    result.BlogAuthor = tblBlog.BlogAuthor;
                }
                if (!string.IsNullOrWhiteSpace(tblBlog.BlogContent))
                {
                    result.BlogContent = tblBlog.BlogContent;
                }

                result.DeleteFlag = false;
                int ans=_appDbContext.SaveChanges();
                string message = ans > 0 ? "Updated!" : "Failed";
                Console.WriteLine(message);
            }
            
        }

        public void Create(TblBlog blog)
        {
            var result=_appDbContext.TblBlogs.Add(blog);
            int ans=_appDbContext.SaveChanges();
            string message = ans > 0 ? "Created!" : "Failed";
            Console.WriteLine(message);
        }

        public void Delete(int id)
        {
            var result=_appDbContext.TblBlogs.FirstOrDefault(x => x.BlogId == id);
            if(result is null)
            {
                Console.WriteLine("Not Found!");
            }
            result.DeleteFlag = true;
            var ans=_appDbContext.SaveChanges();
            string message = ans > 0 ? "Deleted" : "Delete fail";
            Console.WriteLine(message);
        }
    }
}
