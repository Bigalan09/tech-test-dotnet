using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.PaymentPolicies;

public class FasterPaymentsPolicy : IPaymentPolicy
{
    public PaymentScheme Scheme => PaymentScheme.FasterPayments;

    public PaymentDecision Evaluate(Account account, MakePaymentRequest request)
    {
        if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
            return PaymentDecision.Reject(PaymentRejectionReason.SchemeNotAllowed);

        if (account.Balance < request.Amount)
            return PaymentDecision.Reject(PaymentRejectionReason.InsufficientFunds);

        return PaymentDecision.Approve();
    }
}