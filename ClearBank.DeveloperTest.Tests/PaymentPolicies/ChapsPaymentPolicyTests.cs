using AutoFixture;
using ClearBank.DeveloperTest.PaymentPolicies;
using ClearBank.DeveloperTest.Tests.Common;
using ClearBank.DeveloperTest.Types;
using Shouldly;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.PaymentPolicies;

public class ChapsPaymentPolicyTests
{
    [Theory]
    [CustomAutoData]
    internal void Scheme_ReturnsChaps(
        ChapsPaymentPolicy sut)
    {
        // Act
        var actual = sut.Scheme;

        // Assert
        actual.ShouldBe(PaymentScheme.Chaps);
    }

    [Theory]
    [CustomAutoData]
    internal void Evaluate_ReturnsRejected_WhenSchemeIsNotAllowed(
        ChapsPaymentPolicy sut,
        IFixture fixture)
    {
        // Arrange
        MakePaymentRequest request = fixture
            .Build<MakePaymentRequest>()
            .With(r => r.PaymentScheme, PaymentScheme.Chaps)
            .Create();

        Account account = fixture
            .Build<Account>()
            .With(a => a.AllowedPaymentSchemes, AllowedPaymentSchemes.None)
            .With(a => a.Status, AccountStatus.Live)
            .Create();

        // Act
        var actual = sut.Evaluate(account, request);

        // Assert
        actual.IsApproved.ShouldBeFalse();
        actual.PaymentRejectionReason.ShouldBe(PaymentRejectionReason.SchemeNotAllowed);
    }

    [Theory]
    [CustomAutoData]
    internal void Evaluate_ReturnsRejected_WhenAccountIsNotLive(
        ChapsPaymentPolicy sut,
        IFixture fixture)
    {
        // Arrange
        MakePaymentRequest request = fixture
            .Build<MakePaymentRequest>()
            .With(r => r.PaymentScheme, PaymentScheme.Chaps)
            .Create();

        Account account = fixture
            .Build<Account>()
            .With(a => a.AllowedPaymentSchemes, AllowedPaymentSchemes.Chaps)
            .With(a => a.Status, AccountStatus.Disabled)
            .Create();

        // Act
        var actual = sut.Evaluate(account, request);

        // Assert
        actual.IsApproved.ShouldBeFalse();
        actual.PaymentRejectionReason.ShouldBe(PaymentRejectionReason.AccountNotLive);
    }

    [Theory]
    [CustomAutoData]
    internal void Evaluate_ReturnsApproved_WhenSchemeAllowedAndAccountIsLive(
        ChapsPaymentPolicy sut,
        IFixture fixture)
    {
        // Arrange
        MakePaymentRequest request = fixture
            .Build<MakePaymentRequest>()
            .With(r => r.PaymentScheme, PaymentScheme.Chaps)
            .Create();

        Account account = fixture
            .Build<Account>()
            .With(a => a.AllowedPaymentSchemes, AllowedPaymentSchemes.Chaps)
            .With(a => a.Status, AccountStatus.Live)
            .Create();

        // Act
        var actual = sut.Evaluate(account, request);

        // Assert
        actual.IsApproved.ShouldBeTrue();
        actual.PaymentRejectionReason.ShouldBeNull();
    }

    [Theory]
    [CustomAutoData]
    internal void Evaluate_ReturnsRejected_WhenAccountStatusIsInboundPaymentsOnly(
        ChapsPaymentPolicy sut,
        IFixture fixture)
    {
        // Arrange
        MakePaymentRequest request = fixture
            .Build<MakePaymentRequest>()
            .With(r => r.PaymentScheme, PaymentScheme.Chaps)
            .Create();

        Account account = fixture
            .Build<Account>()
            .With(a => a.AllowedPaymentSchemes, AllowedPaymentSchemes.Chaps)
            .With(a => a.Status, AccountStatus.InboundPaymentsOnly)
            .Create();

        // Act
        var actual = sut.Evaluate(account, request);

        // Assert
        actual.IsApproved.ShouldBeFalse();
        actual.PaymentRejectionReason.ShouldBe(PaymentRejectionReason.AccountNotLive);
    }
}