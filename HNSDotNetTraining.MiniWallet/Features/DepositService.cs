using HNSDotNetTraining.MiniWallet.Models;
using Microsoft.IdentityModel.Tokens;

namespace HNSDotNetTraining.MiniWallet.Features
{
    public class DepositService
    {
        private readonly AppDbContext _db;

        public DepositService()
        {
            _db = new AppDbContext();

        }

        public string Deposit(string mobNo, decimal amount)
        {
            var lst=_db.TblAccounts.FirstOrDefault(x => x.MobileNo == mobNo);
            if(lst is null)
            {
                return "Account doesn't exit.";
            }
            if(amount < 0)
            {
                return "amount must be greater than 0.";
            }
            lst.Balance += amount;
            int result=_db.SaveChanges();

            return result > 0 ? $"Deposit successful!Current Balance: {lst.Balance}" : "Deposit fail.";
        }





        
        
    }
}
