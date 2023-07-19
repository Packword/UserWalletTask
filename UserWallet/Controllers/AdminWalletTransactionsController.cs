using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserWallet.Controllers
{
    [Route("admin/wallet/tx")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class AdminWalletTransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public AdminWalletTransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public List<Deposit>? Get()
        {
            return _transactionService.GetAllDeposits();
        }

        [HttpPost("approve/{txId}")]
        public IActionResult ApproveTransaction(string txId)
        {
            if (!int.TryParse(txId, out int id))
                return BadRequest();

            bool result = _transactionService.ApproveTransaction(id);
            if (!result)
                return NotFound();

            return Ok();
        }

        [HttpPost("decline/{txId}")]
        public IActionResult DeclineTransaction(string txId)
        {
            if (!int.TryParse(txId, out int id))
                return BadRequest();

            bool result = _transactionService.DeclineTransaction(id);
            if (!result)
                return NotFound();

            return Ok();
        }
    }
}
