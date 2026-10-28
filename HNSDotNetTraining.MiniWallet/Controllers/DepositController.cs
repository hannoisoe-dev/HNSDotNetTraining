using HNSDotNetTraining.MiniWallet.Features;
using HNSDotNetTraining.MiniWallet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HNSDotNetTraining.MiniWallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositController : ControllerBase
    {
        DepositService service = new DepositService();

        [HttpPost]
        public IActionResult Deposite(string MobileNumber,decimal balance)
        {
            string lst=service.Deposit(MobileNumber, balance);
            return Ok(lst);
        }

       

    }
}
