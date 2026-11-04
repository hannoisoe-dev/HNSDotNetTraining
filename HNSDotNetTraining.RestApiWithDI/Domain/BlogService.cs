using HNSDotNetTraining.Database.Models;

namespace HNSDotNetTraining.RestApiWithDI.Domain
{
    public class BlogService : IBlogService
    {
        private readonly AppDbContext _appDbContext;

        public BlogService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public List<TblBlog> Read()
        {
            
            var lst = _appDbContext.TblBlogs.Where(x => x.DeleteFlag == false).ToList();
            return lst;
        }

        public TblBlog Detail(int id)
        {
            var lst = _appDbContext.TblBlogs.Where(x=>x.DeleteFlag==false).FirstOrDefault(x => x.BlogId == id);
            if (lst is null)
            {
                return null;
            }
            return lst;

        }

        public int Create(TblBlog blog)
        {
            _appDbContext.TblBlogs.Add(blog);
            var result = _appDbContext.SaveChanges();
            return result;
        }

        public int Update(int id, TblBlog blog)
        {
            var lst = _appDbContext.TblBlogs.FirstOrDefault(x => x.BlogId == id);
            if (lst is null)
            {
                Console.WriteLine("Not Found");
                return 0;
            }
            if (!string.IsNullOrWhiteSpace(blog.BlogTitle))
            {
                lst.BlogTitle = blog.BlogTitle;
            }
            if (!string.IsNullOrWhiteSpace(blog.BlogAuthor))
            {
                lst.BlogAuthor = blog.BlogAuthor;
            }
            if (!string.IsNullOrWhiteSpace(blog.BlogContent))
            {
                lst.BlogContent = blog.BlogContent;
            }
            int result = _appDbContext.SaveChanges();
            return result;

        }

        public int Delete(int id)
        {
            var lst = _appDbContext.TblBlogs.FirstOrDefault(x => x.BlogId == id);
            if (lst is null)
            {
                Console.WriteLine("Not Found!");
                return 0;
            }
            lst.DeleteFlag = true;
            var result = _appDbContext.SaveChanges();
            return result;
        }

    }
}
