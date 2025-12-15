using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.PaymentPolicies;

public class BacsPaymentPolicy : IPaymentPolicy
{
    public PaymentScheme Scheme => PaymentScheme.Bacs;
    
    public PaymentDecision Evaluate(Account account, MakePaymentRequest request)
    {
        if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
            return PaymentDecision.Reject(PaymentRejectionReason.SchemeNotAllowed);
        
        return PaymentDecision.Approve();
    }
}