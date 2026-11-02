using HNSDotNetTraining.Database.Models;
using HNSDotNetTraining.Domain.Features.Blog;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HNSDotNetTraining.RESTApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogServiceController : ControllerBase
    {
        [HttpGet]

        public IActionResult GetBlogs([FromServices] IBlogService service)
        {
            
            var lst = service.GetBlogs();
            if (lst is null)
            {
                return BadRequest("Not Found!");
            }
            return Ok(lst);
        }

        [HttpGet("{id}")]

        public IActionResult GetBlogByid([FromServices] IBlogService service,int id)
        {
            var lst = service.GetBlogById(id);
            if (lst is null)
            {
                return NotFound();
            }
            return Ok(lst);
        }

        [HttpPost]

        public IActionResult CreateBlog([FromServices] IBlogService service,TblBlog blog)
        {
            
            var lst = service.CreateBlog(blog);
            if (lst is false)
            {
                return NotFound();
            }
            return Ok(lst);
        }

        [HttpPut("{id}")]

        public IActionResult UpdateBlog([FromServices] IBlogService service,int id, TblBlog blog)
        {
            
            var lst = service.UpdateBlog(id, blog);
            if (lst is false)
            {
                return NotFound();
            }
            return Ok(lst);
        }

        [HttpPatch("{id}")]

        public IActionResult PatchBlog([FromServices] IBlogService service,int id, TblBlog blog)
        {
            
            var lst = service.PatchBlog(id, blog);
            {
                if (lst is false)
                {
                    return NotFound();
                }
                return Ok(lst);
            }
        }

        [HttpDelete]

        public IActionResult DeleteBlog([FromServices] IBlogService service,int id)
        {
           
            var lst=service.DeleteBlog(id);
            if(lst is false)
            {
                return NotFound();
            }
            return Ok("Deleted!");
        }
    }
}
