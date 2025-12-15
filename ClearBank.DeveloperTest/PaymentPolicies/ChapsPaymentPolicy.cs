using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.PaymentPolicies;

public class ChapsPaymentPolicy : IPaymentPolicy
{
    public PaymentScheme Scheme => PaymentScheme.Chaps;
    
    public PaymentDecision Evaluate(Account account, MakePaymentRequest request)
    {
        if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
            return PaymentDecision.Reject(PaymentRejectionReason.SchemeNotAllowed);
        
        if (account.Status != AccountStatus.Live)
            return PaymentDecision.Reject(PaymentRejectionReason.AccountNotLive);
        
        return PaymentDecision.Approve();
    }
}