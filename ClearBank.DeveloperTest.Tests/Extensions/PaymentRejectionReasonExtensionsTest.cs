using ClearBank.DeveloperTest.Extensions;
using ClearBank.DeveloperTest.PaymentPolicies;
using ClearBank.DeveloperTest.Tests.Common;
using ClearBank.DeveloperTest.Types;
using Shouldly;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Extensions;

public class PaymentRejectionReasonExtensionsTest
{
    [Theory]
    [InlineCustomAutoData(new object[] { PaymentRejectionReason.AccountNotLive, "Account not live." })]
    [InlineCustomAutoData(new object[] { PaymentRejectionReason.InsufficientFunds, "Insufficient funds." })]
    [InlineCustomAutoData(new object[] { PaymentRejectionReason.SchemeNotAllowed, "Invalid scheme." })]
    [InlineCustomAutoData(new object[] { PaymentRejectionReason.Unknown, "Unknown reason." })]
    internal void Scheme_A(
        PaymentRejectionReason reason,
        string expectedReason)
    {
        // Act
        var actual = reason.ToReasonString();

        // Assert
        actual.ShouldBe(expectedReason);
    }
}