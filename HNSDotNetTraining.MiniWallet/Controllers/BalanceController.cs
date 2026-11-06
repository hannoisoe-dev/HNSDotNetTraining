using HNSDotNetTraining.MiniWallet.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HNSDotNetTraining.MiniWallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalanceController : ControllerBase
    {
        [HttpGet("{id}")]

        public IActionResult GetBalance(int id, int pin)
        {
            BalanceService service=new BalanceService();
            string result=service.GetBalance(id, pin);
            return Ok(result);

        }
    }
}
