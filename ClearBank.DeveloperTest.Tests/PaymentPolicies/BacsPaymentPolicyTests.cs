using AutoFixture;
using ClearBank.DeveloperTest.PaymentPolicies;
using ClearBank.DeveloperTest.Tests.Common;
using ClearBank.DeveloperTest.Types;
using Shouldly;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.PaymentPolicies;

public class BacsPaymentPolicyTests
{
    [Theory]
    [CustomAutoData]
    internal void Scheme_ReturnsBacs(
        BacsPaymentPolicy sut)
    {
        // Act
        var actual = sut.Scheme;

        // Assert
        actual.ShouldBe(PaymentScheme.Bacs);
    }

    [Theory]
    [CustomAutoData]
    internal void Evaluate_ReturnsRejected_WhenSchemeIsNotAllowed(
        BacsPaymentPolicy sut,
        IFixture fixture)
    {
        // Arrange
        MakePaymentRequest request = fixture
            .Build<MakePaymentRequest>()
            .With(r => r.PaymentScheme, PaymentScheme.Bacs)
            .Create();

        Account account = fixture
            .Build<Account>()
            .With(a => a.AllowedPaymentSchemes, AllowedPaymentSchemes.None)
            .Create();

        // Act
        var actual = sut.Evaluate(account, request);

        // Assert
        actual.IsApproved.ShouldBeFalse();
        actual.PaymentRejectionReason.ShouldBe(PaymentRejectionReason.SchemeNotAllowed);
    }

    [Theory]
    [CustomAutoData]
    internal void Evaluate_ReturnsApproved_WhenSchemeIsAllowed(
        BacsPaymentPolicy sut,
        IFixture fixture)
    {
        // Arrange
        MakePaymentRequest request = fixture
            .Build<MakePaymentRequest>()
            .With(r => r.PaymentScheme, PaymentScheme.Bacs)
            .Create();

        Account account = fixture
            .Build<Account>()
            .With(a => a.AllowedPaymentSchemes, AllowedPaymentSchemes.Bacs)
            .Create();

        // Act
        var actual = sut.Evaluate(account, request);

        // Assert
        actual.IsApproved.ShouldBeTrue();
        actual.PaymentRejectionReason.ShouldBeNull();
    }

    [Theory]
    [CustomAutoData]
    internal void Evaluate_ReturnsApproved_WhenMultipleSchemesIncludeBacs(
        BacsPaymentPolicy sut,
        IFixture fixture)
    {
        // Arrange
        MakePaymentRequest request = fixture
            .Build<MakePaymentRequest>()
            .With(r => r.PaymentScheme, PaymentScheme.Bacs)
            .Create();

        Account account = fixture
            .Build<Account>()
            .With(a => a.AllowedPaymentSchemes, AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments)
            .Create();

        // Act
        var actual = sut.Evaluate(account, request);

        // Assert
        actual.IsApproved.ShouldBeTrue();
        actual.PaymentRejectionReason.ShouldBeNull();
    }
}