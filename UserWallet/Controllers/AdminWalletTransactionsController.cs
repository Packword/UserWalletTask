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
            var transaction = _transactionService.GetTransactionById(txId);
            if (transaction is null)
                return NotFound();

            if(transaction.Status != DepositStatus.Undecided)
                return BadRequest();

            _transactionService.ApproveTransaction(txId);
            return Ok();
        }

        [HttpPost("decline/{txId:int}")]
        public IActionResult DeclineTransaction(int txId)
        {
            var transaction = _transactionService.GetTransactionById(txId);
            if (transaction is null)
                return NotFound();

            if (transaction.Status != DepositStatus.Undecided)
                return BadRequest();

            _transactionService.DeclineTransaction(txId);
            return Ok();
        }
    }
}
