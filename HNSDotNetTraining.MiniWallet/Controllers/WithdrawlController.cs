using HNSDotNetTraining.MiniWallet.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HNSDotNetTraining.MiniWallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawlController : ControllerBase
    {
        [HttpPost]

        public IActionResult Withdrawl(string MobileNumber, decimal balance)
        {
            WithdrawlService service = new WithdrawlService();
            string msg=service.Withdrawl(MobileNumber, balance);
            return Ok(msg);
        }
    }
}
