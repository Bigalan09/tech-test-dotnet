using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.PaymentPolicies;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    internal class PaymentService(IDataStore dataStore, IPaymentSchemeRules paymentSchemeRules) : IPaymentService
    {
        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            if (request.Amount <= 0)
                return MakePaymentResult.Rejected("Invalid amount.");
            if (string.IsNullOrWhiteSpace(request.DebtorAccountNumber))
                return MakePaymentResult.Rejected("Invalid debtor account number.");

            Account? account = dataStore.GetAccount(request.DebtorAccountNumber);
            if (account == null)
                return MakePaymentResult.Rejected("Account not found.");

            var schemePolicy = paymentSchemeRules.For(request.PaymentScheme);
            var paymentDecision = schemePolicy.Evaluate(account, request);

            if (paymentDecision.IsRejected)
                return MakePaymentResult.Rejected(paymentDecision.PaymentRejectionReason?.ToString() ?? "Unknown reason.");

            account.Balance -= request.Amount;
            dataStore.UpdateAccount(account);

            return MakePaymentResult.Success();
        }
    }
}
