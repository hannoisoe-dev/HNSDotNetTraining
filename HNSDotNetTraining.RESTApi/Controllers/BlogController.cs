using HNSDotNetTraining.Database.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HNSDotNetTraining.RESTApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly AppDbContext _db = new AppDbContext();

        [HttpGet]
        public IActionResult GellBlogs()
        {
            var lst = _db.TblBlogs.AsNoTracking().ToList();
            return Ok(lst);
        }

        [HttpGet("{id}")]
        public IActionResult GellBlog(int id)
        {
            var lst = _db.TblBlogs.AsNoTracking().Where(x => x.DeleteFlag == false).FirstOrDefault(x => x.BlogId == id);

            if (lst is null)
            {
                return NotFound();
            }

            return Ok(lst);
        }

        [HttpPost]
        public IActionResult CreateBlog(TblBlog blog)
        {
            _db.TblBlogs.Add(blog);
            int result = _db.SaveChanges();
            if (result == 0)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBlog(int id, TblBlog blog)
        {
            var lst = _db.TblBlogs.AsNoTracking().FirstOrDefault(x => x.BlogId == id);
            if (lst is null)
            {
                return NotFound();
            }

            lst.BlogTitle = blog.BlogTitle;
            lst.BlogAuthor = blog.BlogAuthor;
            lst.BlogContent = blog.BlogContent;
            lst.DeleteFlag = false;

            _db.Entry(lst).State = EntityState.Modified;
            _db.SaveChanges();
            return Ok();
        }

        [HttpPatch("{id}")]
        public IActionResult PatchBlog(int id, TblBlog blog)
        {
            var lst = _db.TblBlogs.AsNoTracking().Where(x => x.DeleteFlag == false).FirstOrDefault(x => x.BlogId == id);
            if (lst is null)
            {
                return NotFound();
            }
            if (!blog.BlogTitle.IsNullOrEmpty())
            {
                lst.BlogTitle = blog.BlogTitle;
            }
            if (!blog.BlogAuthor.IsNullOrEmpty())
            {
                lst.BlogAuthor = blog.BlogAuthor;
            }
            if (!blog.BlogContent.IsNullOrEmpty())
            {
                lst.BlogContent = blog.BlogContent;
            }

            _db.Entry(lst).State = EntityState.Modified;
            _db.SaveChanges();
            return Ok(lst);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBlog(int id)
        {
           var lst= _db.TblBlogs.AsNoTracking().Where(x => x.DeleteFlag == false).FirstOrDefault(x => x.BlogId == id);
            if(lst is null)
            {
                return NotFound();
            }
            lst.DeleteFlag = true;
            _db.Entry(lst).State = EntityState.Deleted;
            _db.SaveChanges();
            return Ok("Deleted!");
        }
    }
}
