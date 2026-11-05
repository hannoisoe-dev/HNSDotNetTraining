using HNSDotNetTraining.RestApiWithDI.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HNSDotNetTraining.RestApiWithDI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogDapperController : ControllerBase
    {

        private readonly IBlogDapper _service;

        public BlogDapperController(IBlogDapper service)
        {
            _service = service;
        }

        [HttpGet]

        public IActionResult GetBlogs()
        {
            var lst=_service.GetBlogs();
            if(lst is null)
            {
                return NotFound("Not found!");
            }
            return Ok(lst);
        }

        [HttpGet("{id}")]

        public IActionResult GetBlog(int id)
        {
            var lst=_service.GetBlog(id);
            if(lst is null)
            {
                return BadRequest("Not found!");
            }
            return Ok(lst);
        }

        [HttpPost]

        public IActionResult CreateBlog(BlogDataModel blog)
        {
            int result=_service.CreateBlog(blog);
            return result > 0 ? Ok("Created!") : BadRequest("Create Fail");
        }


        [HttpPut("{id}")]

        public IActionResult Update(int id, BlogDataModel blog)
        {
            int result = _service.UpdateBlog(id, blog);
            return result > 0 ? Ok("Updated!") : BadRequest("Update Fial");
        }

        [HttpDelete("{id}")]

        public IActionResult Delete(int id)
        {
            int result = _service.Delete(id);
            return result > 0 ? Ok("Deleted!") : BadRequest("Delete Fail");
        }

    }
}
