using HNSDotNetTraining.Database.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNSDotNetTraining.Domain.Features.Blog
{
    
    public class BlogService
    {
        private readonly AppDbContext _db;

        public BlogService()
        {
            _db = new AppDbContext();
        }

        public List<TblBlog> GetBlogs()
        {
            var lst =_db.TblBlogs.Where(x=>x.DeleteFlag == false).ToList();
            return lst;
        }

        public TblBlog GetBlogById(int id)
        {
            var lst=_db.TblBlogs.FirstOrDefault(x => x.BlogId == id & x.DeleteFlag==false);
            if(lst is null)
            {
                return null;
            }
            return lst;
        }

        public bool CreateBlog(TblBlog blog)
        {
            _db.TblBlogs.Add(blog);
            int result= _db.SaveChanges();
            return result > 0 ? true : false;
        }

        public bool? UpdateBlog(int id,TblBlog blog)
        {
            var lst=_db.TblBlogs.FirstOrDefault(x => x.BlogId == id & x.DeleteFlag == false);
            if(lst is null)
            {
                return null;
            }
            lst.BlogTitle = blog.BlogTitle;
            lst.BlogAuthor = blog.BlogAuthor;
            lst.BlogContent = blog.BlogContent;
            int result=_db.SaveChanges();
            return result > 0 ? true : false;
        }

        public bool? PatchBlog(int id,TblBlog blog)
        {
            var lst=_db.TblBlogs.FirstOrDefault(x => x.BlogId == id& x.DeleteFlag == false);
            if(lst is null)
            {
                return null;
            }
            if (!blog.BlogTitle.IsNullOrEmpty())
            {
                lst.BlogTitle=blog.BlogTitle;
            }
            if (!blog.BlogAuthor.IsNullOrEmpty())
            {
                lst.BlogAuthor=blog.BlogAuthor;
            }
            if (!blog.BlogContent.IsNullOrEmpty())
            {
                lst.BlogContent = blog.BlogContent;
            }
            int result=_db.SaveChanges();
            return result > 0 ? true : false;
        }

        public bool? DeleteBlog(int id)
        {
            var lst=_db.TblBlogs.FirstOrDefault(x => x.BlogId == id & x.DeleteFlag == false);
            if(lst is null)
            {
                return null;
            }

            lst.DeleteFlag = true;

            int result=_db.SaveChanges();

            return result > 0 ? true : false;


        }
    }
}
