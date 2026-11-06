using HNSDotNetTraining.MiniWallet.Models;

namespace HNSDotNetTraining.MiniWallet.Features
{
    public class TransferService
    {
        private readonly AppDbContext _db;

        public TransferService()
        {
            _db = new AppDbContext();
        }

        public string Transfer(string FrmMobile,string ToMobile,decimal Amount,int Pin)
        {
            if(FrmMobile == ToMobile)
            {
                return "Invalid Mobile Number!";
            }

            var FrmMob=_db.TblAccounts.FirstOrDefault(x => x.MobileNo == FrmMobile);
            if(FrmMob is null)
            {
                return "From Mobile Number doesn't exist.";
            }
            var ToMob = _db.TblAccounts.FirstOrDefault(x => x.MobileNo == ToMobile);
            if(ToMob is null)
            {
                return "To Mobile Number doesn't exist.";
            }
            if (FrmMob.Pin != Pin)
            {
                return "Incorrect Pin!";
            }
            if(Amount < 0)
            {
                return "Amount must be greater than 0.";
            }

            if(FrmMob.Balance < Amount)
            {
                return "Insufficient balance.";
            }

            decimal checkBalance = FrmMob.Balance - Amount;

            if(checkBalance < 10000)
            {
                return "Remain Balance must be not less than 10000.";
            }

            FrmMob.Balance = checkBalance;
            ToMob.Balance += Amount;
            int result=_db.SaveChanges();

            
            
            if(result > 0)
            {
                _db.TblTransactions.Add(new TblTransaction
                {
                    FromMb = FrmMobile,
                    ToMb = ToMobile,
                    Amount = Amount,
                    Date = DateTime.Now
                });
                return "Transfer Successful.";
            }

            return "Transfer Fail.";
            

        }
    }
}
