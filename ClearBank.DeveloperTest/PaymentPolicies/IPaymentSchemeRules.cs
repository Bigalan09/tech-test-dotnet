using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.PaymentPolicies;

public interface IPaymentSchemeRules
{
    IPaymentPolicy For(PaymentScheme scheme);
}