using HNSDotNetTraining.RestApiWithDI.Data;

namespace HNSDotNetTraining.RestApiWithDI.Domain
{
    public interface IBlogDapper
    {
        int CreateBlog(BlogDataModel blog);
        int Delete(int id);
        BlogDataModel GetBlog(int id);
        List<BlogDataModel> GetBlogs();
        int UpdateBlog(int id, BlogDataModel blog);
    }
}