using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    internal class PaymentService(IDataStore dataStore) : IPaymentService
    {
        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            if (request == null || request.Amount <= 0)
                return MakePaymentResult.Rejected("Invalid amount.");
            if (string.IsNullOrWhiteSpace(request.DebtorAccountNumber))
                return MakePaymentResult.Rejected("Invalid debtor account number.");

            Account account = dataStore.GetAccount(request.DebtorAccountNumber);
            if (account == null)
                return MakePaymentResult.Rejected("Account not found.");
            
            account.Balance -= request.Amount;
            dataStore.UpdateAccount(account);

            return MakePaymentResult.Success();
        }
    }
}
