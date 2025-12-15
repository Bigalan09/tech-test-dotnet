using System;
using System.Collections.Generic;
using System.Linq;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.PaymentPolicies;

public class PaymentSchemeRules : IPaymentSchemeRules
{
    private readonly Dictionary<PaymentScheme, IPaymentPolicy> _policies;

    public PaymentSchemeRules(IEnumerable<IPaymentPolicy> policies)
    {
        _policies = policies.ToDictionary(p => p.Scheme);
    }
    
    public IPaymentPolicy For(PaymentScheme scheme)
    {
        if (!Enum.IsDefined(typeof(PaymentScheme), scheme))
            throw new ArgumentException("Invalid scheme provided.");
        
        if (!_policies.TryGetValue(scheme, out var policy))
            throw new InvalidOperationException($"No policy for {scheme}");

        return policy;
    }
}