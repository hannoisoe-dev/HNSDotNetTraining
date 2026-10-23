using HNSDotNetTraining.Database.Models;
using HNSDotNetTraining.Domain.Features.Blog;

namespace HNSDotNetTraining.MinimalAPI.EndPoint
{
    public static class BlogServiceEndPoint
    {
        public static void UseBlogService(this IEndpointRouteBuilder app)
        {
            BlogService service = new BlogService();
            app.MapGet("/blogs", () =>
            {
                var lst = service.GetBlogs();
                return lst is null ? null : lst;
            })
            .WithName("GetBlogs")
            .WithOpenApi();


            app.MapGet("/blog/{id}", (int id) =>
            {
                var lst = service.GetBlogById(id);
                return lst is null ? null : lst;
            })
            .WithDisplayName("GetBlogById")
            .WithOpenApi();


            app.MapPost("/blog", (TblBlog blog) =>
            {
                var lst = service.CreateBlog(blog);
                return lst is false ? false : lst;
            })
            .WithDisplayName("Create Blog.")
            .WithOpenApi();

            app.MapPut("/blog", (int id, TblBlog blog) =>
            {
                var lst = service.UpdateBlog(id, blog);
                return lst is false ? false : lst;

            })
            .WithDisplayName("Update Blog")
            .WithOpenApi();

            app.MapPatch("/blog", (int id, TblBlog blog) =>
            {
                bool? lst = service.PatchBlog(id, blog);
                return lst is false ? false : true;
            })
            .WithDisplayName("Patch Blog")
            .WithOpenApi();

            app.MapDelete("/blog", (int id) =>
            {
                var lst = service.DeleteBlog(id);
                return lst is false ? false : true;
            })
            .WithDisplayName("Delete Blog")
            .WithOpenApi();
        }

       
    }
}
