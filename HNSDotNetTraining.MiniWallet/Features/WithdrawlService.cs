using HNSDotNetTraining.MiniWallet.Models;

namespace HNSDotNetTraining.MiniWallet.Features
{
    public class WithdrawlService
    {
        private readonly AppDbContext _dp;

        public WithdrawlService()
        {
            _dp = new AppDbContext();
        }

        public string Withdrawl(string mobNo,decimal balance)
        {
            var lst=_dp.TblAccounts.FirstOrDefault(x => x.MobileNo == mobNo);
            if(lst is null)
            {
                return "Account doesn't exist";
            }
            if(balance < 0)
            {
                return "Balance must be greater than 0.";
            }
            lst.Balance -= balance;
            if(lst.Balance >= 10000)
            {
                int result = _dp.SaveChanges();
                return result > 0 ? $"Withdrawl Successful.Current Balance: {lst.Balance}" : "Withdrawl Failed";
            }
            return "Remain Balance must be greater than 10000.";
            
        }




    }
}
