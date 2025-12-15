using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.PaymentPolicies;

public interface IPaymentPolicy
{
    PaymentScheme Scheme { get; }
    PaymentDecision Evaluate(Account account, MakePaymentRequest request);
}