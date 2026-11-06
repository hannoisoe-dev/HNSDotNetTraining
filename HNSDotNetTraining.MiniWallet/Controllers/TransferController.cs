using HNSDotNetTraining.MiniWallet.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HNSDotNetTraining.MiniWallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        [HttpPost]

        public IActionResult Transfer(string FrmMoblie,string ToMobile,decimal amount,int pin)
        {
            TransferService service = new TransferService();
            string message=service.Transfer(FrmMoblie,ToMobile,amount,pin);
            return Ok(message);
        }
    }
}
