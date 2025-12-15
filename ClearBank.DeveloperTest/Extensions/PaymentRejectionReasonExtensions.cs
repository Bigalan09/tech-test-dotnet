using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Extensions;

public static class PaymentRejectionReasonExtensions
{
    public static string ToReasonString(this PaymentRejectionReason value)
    {
        return value switch
        {
            PaymentRejectionReason.AccountNotLive => "Account not live.",
            PaymentRejectionReason.InsufficientFunds => "Insufficient funds.",
            PaymentRejectionReason.SchemeNotAllowed => "Invalid scheme.",
            
            _ => "Unknown reason."
        };
    }

}