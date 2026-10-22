using HNSDotNetTraining.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HNSDotNetTraining.MinimalAPI.EndPoint
{
    public static class Blog
    {
        public static void UseBlogEndPoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/blog", () =>
            {
                AppDbContext db = new AppDbContext();
                var lst = db.TblBlogs.AsNoTracking().Where(x => x.DeleteFlag == false).ToList();
                if (lst is null)
                {
                    return Results.BadRequest("Not Found!");
                }
                return Results.Ok(lst);
            })
                .WithName("GetBlog")
                .WithOpenApi();

            app.MapGet("/blog/{id}", (int id) =>
            {
                AppDbContext db = new AppDbContext();
                var lst = db.TblBlogs.AsNoTracking().Where(x => x.BlogId == id && x.DeleteFlag == false).ToList();
                if (lst is null)
                {
                    return Results.BadRequest("Not Found!");
                }
                return Results.Ok(lst);
            })
            .WithName("GetBlogById")
            .WithOpenApi();

            app.MapPost("/blog", (TblBlog blog) =>
            {
                AppDbContext db = new AppDbContext();
                db.Add(blog);
                int result = db.SaveChanges();

                return result > 0 ? Results.Ok("Creating Successful") : Results.BadRequest("Fail");
            })
            .WithName("CrateBlog")
            .WithOpenApi();

            app.MapPut("/blog/{id}", (int id, TblBlog blog) =>
            {
                AppDbContext db = new AppDbContext();
                var lst = db.TblBlogs.FirstOrDefault(x => x.BlogId == id);
                if (lst is null)
                {
                    return Results.BadRequest("not found!");
                }

                lst.BlogTitle = blog.BlogTitle;
                lst.BlogAuthor = blog.BlogAuthor;
                lst.BlogContent = blog.BlogContent;

                db.SaveChanges();

                return Results.Ok("Updated!");
            })
            .WithName("UpdateBlog")
            .WithOpenApi();

            app.MapDelete("/blog", (int id) =>
            {
                AppDbContext db = new AppDbContext();
                var lst = db.TblBlogs.FirstOrDefault(x => x.BlogId == id);
                if (lst is null)
                {
                    return Results.BadRequest("Not Found!");
                }

                lst.DeleteFlag = true;
                db.SaveChanges();

                return Results.Ok("Deleted!");
            })
            .WithName("DeleteBlog")
            .WithOpenApi();


            app.MapPatch("/blog", (int id, TblBlog blog) =>
            {
                AppDbContext db = new AppDbContext();
                var lst = db.TblBlogs.FirstOrDefault(x => x.BlogId == id);
                if (lst is null)
                {
                    return Results.BadRequest("Not Found!");
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

                db.SaveChanges();

                return Results.Ok("Updated!");
            })
            .WithName("PatchBlog")
            .WithOpenApi();

           
        }
    }
}
