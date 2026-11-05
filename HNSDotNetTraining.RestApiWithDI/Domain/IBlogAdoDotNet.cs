using HNSDotNetTraining.RestApiWithDI.Data;
using System.Data;

namespace HNSDotNetTraining.RestApiWithDI.Domain
{
    public interface IBlogAdoDotNet
    {
        int Create(BlogDataModel blog);
        int Delete(int id);
        DataTable GetBlog(int id);
        DataTable GetBlogs();
        int Update(int id, BlogDataModel blog);
    }
}