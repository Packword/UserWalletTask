﻿namespace UserWallet.Tests.Helpers
{
    public static class TransactionServiceHelper
    {
        public static DepositDTO CreateDepositDTO(decimal amount, string? address, string? cardholderName, string? cardNumber)
            => new DepositDTO
            {
                Amount = amount,
                Address = address,
                CardholderName = cardholderName,
                CardNumber = cardNumber
            };
    }
}
