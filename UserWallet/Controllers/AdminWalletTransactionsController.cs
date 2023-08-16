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
            var (Result, Message) = _transactionService.ApproveTransaction(txId);
            return Result switch
            {
                ServiceResult.Success => Ok(),
                ServiceResult.NotFound => NotFound(),
                _ => BadRequest(Message)
            };
        }

        [HttpPost("decline/{txId:int}")]
        public IActionResult DeclineTransaction(int txId)
        {
            var (Result, Message) = _transactionService.DeclineTransaction(txId);
            return Result switch
            {
                ServiceResult.Success => Ok(),
                ServiceResult.NotFound => NotFound(),
                _ => BadRequest(Message)
            };
        }
    }
}
