namespace UserWallet.Controllers
{
    [Route("admin/wallet/tx")]
    [Authorize(Roles = UsersRole.ADMIN)]
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
            => _transactionService.GetAllDeposits();

        [HttpPost("approve/{txId:int}")]
        public IActionResult ApproveTransaction(int txId)
        {
            bool result = _transactionService.ApproveTransaction(txId);
            if (!result)
                return NotFound();

            return Ok();
        }

        [HttpPost("decline/{txId:int}")]
        public IActionResult DeclineTransaction(int txId)
        {
            bool result = _transactionService.DeclineTransaction(txId);
            if (!result)
                return NotFound();

            return Ok();
        }
    }
}
