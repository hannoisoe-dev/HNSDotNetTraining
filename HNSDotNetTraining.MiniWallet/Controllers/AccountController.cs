using HNSDotNetTraining.MiniWallet.Features;
using HNSDotNetTraining.MiniWallet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HNSDotNetTraining.MiniWallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpPost]

        public IActionResult CreateAccount(TblAccount account)
        {
            AccountService service = new AccountService();
            var result = service.CreateAccount(account);
            return Ok(result);

        }

        [HttpGet("{id}")]

        public IActionResult GetAccountById(int id)
        {
            AccountService service = new AccountService();
            var result=service.GetAccountById(id);
            return Ok(result);
        }

        



    }
}
