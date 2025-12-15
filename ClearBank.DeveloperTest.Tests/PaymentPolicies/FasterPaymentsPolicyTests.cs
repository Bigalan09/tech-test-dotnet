using AutoFixture;
using ClearBank.DeveloperTest.PaymentPolicies;
using ClearBank.DeveloperTest.Tests.Common;
using ClearBank.DeveloperTest.Types;
using Shouldly;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.PaymentPolicies;

public class FasterPaymentsPolicyTests
{
    [Theory]
    [CustomAutoData]
    internal void Scheme_ReturnsFasterPayments(
        FasterPaymentsPolicy sut)
    {
        // Act
        var actual = sut.Scheme;

        // Assert
        actual.ShouldBe(PaymentScheme.FasterPayments);
    }

    [Theory]
    [CustomAutoData]
    internal void Evaluate_ReturnsRejected_WhenSchemeIsNotAllowed(
        FasterPaymentsPolicy sut,
        IFixture fixture)
    {
        // Arrange
        MakePaymentRequest request = fixture
            .Build<MakePaymentRequest>()
            .With(r => r.PaymentScheme, PaymentScheme.FasterPayments)
            .Create();

        Account account = fixture
            .Build<Account>()
            .With(a => a.AllowedPaymentSchemes, AllowedPaymentSchemes.None)
            .With(a => a.Balance, request.Amount + 1)
            .Create();

        // Act
        var actual = sut.Evaluate(account, request);

        // Assert
        actual.IsApproved.ShouldBeFalse();
        actual.PaymentRejectionReason.ShouldBe(PaymentRejectionReason.SchemeNotAllowed);
    }

    [Theory]
    [CustomAutoData]
    internal void Evaluate_ReturnsRejected_WhenInsufficientFunds(
        FasterPaymentsPolicy sut,
        IFixture fixture)
    {
        // Arrange
        MakePaymentRequest request = fixture
            .Build<MakePaymentRequest>()
            .With(r => r.PaymentScheme, PaymentScheme.FasterPayments)
            .Create();

        Account account = fixture
            .Build<Account>()
            .With(a => a.AllowedPaymentSchemes, AllowedPaymentSchemes.FasterPayments)
            .With(a => a.Balance, request.Amount - 0.01m)
            .Create();

        // Act
        var actual = sut.Evaluate(account, request);

        // Assert
        actual.IsApproved.ShouldBeFalse();
        actual.PaymentRejectionReason.ShouldBe(PaymentRejectionReason.InsufficientFunds);
    }

    [Theory]
    [CustomAutoData]
    internal void Evaluate_ReturnsApproved_WhenSchemeAllowedAndSufficientFunds(
        FasterPaymentsPolicy sut,
        IFixture fixture)
    {
        // Arrange
        MakePaymentRequest request = fixture
            .Build<MakePaymentRequest>()
            .With(r => r.PaymentScheme, PaymentScheme.FasterPayments)
            .Create();

        Account account = fixture
            .Build<Account>()
            .With(a => a.AllowedPaymentSchemes, AllowedPaymentSchemes.FasterPayments)
            .With(a => a.Balance, request.Amount)
            .Create();

        // Act
        var actual = sut.Evaluate(account, request);

        // Assert
        actual.IsApproved.ShouldBeTrue();
        actual.PaymentRejectionReason.ShouldBeNull();
    }

    [Theory]
    [CustomAutoData]
    internal void Evaluate_ReturnsApproved_WhenBalanceEqualsAmount(
        decimal amount,
        FasterPaymentsPolicy sut,
        IFixture fixture)
    {
        // Arrange
        MakePaymentRequest request = fixture
            .Build<MakePaymentRequest>()
            .With(r => r.Amount, amount)
            .With(r => r.PaymentScheme, PaymentScheme.FasterPayments)
            .Create();

        Account account = fixture
            .Build<Account>()
            .With(a => a.AllowedPaymentSchemes, AllowedPaymentSchemes.FasterPayments)
            .With(a => a.Balance, amount)
            .Create();

        // Act
        var actual = sut.Evaluate(account, request);

        // Assert
        actual.IsApproved.ShouldBeTrue();
        actual.PaymentRejectionReason.ShouldBeNull();
    }

    [Theory]
    [CustomAutoData]
    internal void Evaluate_ReturnsRejected_WhenBalanceIsJustBelowAmount(
        decimal amount,
        FasterPaymentsPolicy sut,
        IFixture fixture)
    {
        // Arrange
        MakePaymentRequest request = fixture
            .Build<MakePaymentRequest>()
            .With(r => r.Amount, amount)
            .With(r => r.PaymentScheme, PaymentScheme.FasterPayments)
            .Create();

        Account account = fixture
            .Build<Account>()
            .With(a => a.AllowedPaymentSchemes, AllowedPaymentSchemes.FasterPayments)
            .With(a => a.Balance, amount - 0.01m)
            .Create();

        // Act
        var actual = sut.Evaluate(account, request);

        // Assert
        actual.IsApproved.ShouldBeFalse();
        actual.PaymentRejectionReason.ShouldBe(PaymentRejectionReason.InsufficientFunds);
    }
}