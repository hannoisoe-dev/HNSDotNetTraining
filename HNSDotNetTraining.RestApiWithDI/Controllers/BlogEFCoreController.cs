using HNSDotNetTraining.Database.Models;
using HNSDotNetTraining.RestApiWithDI.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HNSDotNetTraining.RestApiWithDI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogEFCoreController : ControllerBase
    {
        private readonly IBlogService _service;

        public BlogEFCoreController(IBlogService service)
        {
            _service = service;
        }

        [HttpGet]

        public IActionResult Read()
        {
            var lst=_service.Read();
            if(lst is null)
            {
                return BadRequest("No Data Found!");
            }
            return Ok(lst);
        }

        [HttpGet("{id}")]

        public IActionResult Edit(int id)
        {
            var lst=_service.Detail(id);
            if(lst is null)
            {
                return BadRequest("Not Found!");
            }
            return Ok(lst);
        }

        [HttpPost]

        public IActionResult Create(TblBlog blog)
        {
            int result=_service.Create(blog);
            return result > 0 ? Ok("Created") : BadRequest("Create Failed");
        }

        [HttpPut("{id}")]

        public IActionResult Update(int id,TblBlog blog)
        {
            int result= _service.Update(id, blog);
            return result > 0 ? Ok("Updated") : BadRequest("Update Failed");
        }

        //[HttpPatch("{id}")]

        //public IActionResult patchUpdate(int id,TblBlog blog)
        //{
        //    int result = _service.Update(id, blog);
        //    return result > 0 ? Ok("Updated") : BadRequest("Update Failed");
        //}

        [HttpDelete("{id}")]

        public IActionResult Delete(int id)
        {
            int result=_service.Delete(id);
            return result > 0 ? Ok("Deleted") : BadRequest("Delete Failed");
        }

    }
}
