using HNSDotNetTraining.Database.Models;

namespace HNSDotNetTraining.RestApiWithDI.Domain
{
    public interface IBlogService
    {
        int Create(TblBlog blog);
        int Delete(int id);
        TblBlog Detail(int id);
        List<TblBlog> Read();
        int Update(int id, TblBlog blog);
    }
}