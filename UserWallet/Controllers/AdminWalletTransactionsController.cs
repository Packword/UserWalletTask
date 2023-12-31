﻿namespace UserWallet.Controllers
{
    [Route("api/admin/wallet/tx")]
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
            var (Result, Message) = _transactionService.DecideTransactionStatus(txId, DepositStatus.Approved);
            return Result switch
            {
                ServiceResult.Success => Ok(),
                ServiceResult.NotFound => NotFound(Message),
                _ => BadRequest(Message)
            };
        }

        [HttpPost("decline/{txId:int}")]
        public IActionResult DeclineTransaction(int txId)
        {
            var (Result, Message) = _transactionService.DecideTransactionStatus(txId, DepositStatus.Declined);
            return Result switch
            {
                ServiceResult.Success => Ok(),
                ServiceResult.NotFound => NotFound(),
                _ => BadRequest(Message)
            };
        }
    }
}
