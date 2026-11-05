using HNSDotNetTraining.RestApiWithDI.Data;
using HNSDotNetTraining.RestApiWithDI.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HNSDotNetTraining.RestApiWithDI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogAdoDotNetController : ControllerBase
    {
        private readonly IBlogAdoDotNet _service;

        public BlogAdoDotNetController(IBlogAdoDotNet service)
        {
            _service = service;
        }

        [HttpGet]

        public IActionResult GetBlog()
        {
            var lst=_service.GetBlogs();
            if(lst is null)
            {
                return BadRequest("Not Found!");
            }
            return Ok(lst);
        }

        [HttpGet("{id}")]

        public IActionResult GetBlog(int blogId)
        {
            var lst=_service.GetBlog(blogId);
            if(lst is null)
            {
                return BadRequest("Not found!");
            }
            return Ok(lst);
        }

        [HttpPost]

        public IActionResult Create(BlogDataModel blog)
        {
            var lst=_service.Create(blog);
            if(lst == 0 )
            {
                return BadRequest("Not Found!");
            }
            return Ok("Created!");
        }

        [HttpPatch("{id}")]

        public IActionResult Update(int id,BlogDataModel blog)
        {
            var lst=_service.Update(id, blog);
            if(lst == 0)
            {
                return BadRequest("Not Found!");
            }
            return Ok("Updated!");
        }

        [HttpDelete("{id}")]

        public IActionResult Delete(int id)
        {
            var lst= _service.Delete(id);
            if(lst == 0)
            {
                return BadRequest("Not Found!");
            }
            return Ok("Deleted!");
        }

    }
}
