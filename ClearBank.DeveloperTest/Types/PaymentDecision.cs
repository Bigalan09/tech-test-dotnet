using System;

namespace ClearBank.DeveloperTest.Types;

public sealed class PaymentDecision
{
    private PaymentDecision(bool isApproved, PaymentRejectionReason? paymentRejectionReason)
    {
        IsApproved = isApproved;
        PaymentRejectionReason = paymentRejectionReason;
    }

    public bool IsApproved { get; }
    public bool IsRejected => !IsApproved;

    public PaymentRejectionReason? PaymentRejectionReason { get; }

    public static PaymentDecision Approve()
        => new PaymentDecision(true, null);

    public static PaymentDecision Reject(PaymentRejectionReason reason)
    {
        return new PaymentDecision(false, reason);
    }
}