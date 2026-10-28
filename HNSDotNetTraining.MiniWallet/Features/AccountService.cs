using HNSDotNetTraining.MiniWallet.Models;
using Microsoft.IdentityModel.Tokens;

namespace HNSDotNetTraining.MiniWallet.Features
{
    public class AccountService
    {
        private readonly AppDbContext _db;

        public AccountService()
        {
            _db = new AppDbContext();
        }

        public string CreateAccount(TblAccount account)
        {
            if (account.FullName.IsNullOrEmpty() || account.MobileNo.IsNullOrEmpty())
            {
                return "Invalid FullName or mobile number.";
            }
            if (account.Balance < 0)
            {
                return "Balance must be greater than 0.";

            }

            _db.TblAccounts.Add(account);
            var lst = _db.SaveChanges();
            return lst > 0 ? "Creating Successful" : "Creating Failed";
        }

        public TblAccount GetAccountById(int id)
        {
            var lst = _db.TblAccounts.FirstOrDefault(x => x.Id == id);
            if (lst is null)
            {
                return null;
            }
            return lst;
        }
    }
}
