using HNSDotNetTraining.MiniWallet.Models;

namespace HNSDotNetTraining.MiniWallet.Features
{
    public class BalanceService
    {
        private readonly AppDbContext _db;

        public BalanceService()
        {
            _db = new AppDbContext();
        }

        public string GetBalance(int id, int password)
        {
           var result= _db.TblAccounts.FirstOrDefault(x => x.Id==id & x.Pin==password);
            if(result is null)
            {
                return "Incorrect Id or Pin";
            }
            return result.Balance.ToString();
        }

    }
}
