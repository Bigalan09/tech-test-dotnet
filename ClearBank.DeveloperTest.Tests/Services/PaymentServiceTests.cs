using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Xunit2;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Extensions;
using ClearBank.DeveloperTest.PaymentPolicies;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Tests.Common;
using ClearBank.DeveloperTest.Types;
using NSubstitute;
using Shouldly;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Services;

public class PaymentServiceTests
{
    [Theory]
    [InlineCustomAutoData(0.0)]
    [InlineCustomAutoData(-10.0)]
    internal void MakePayment_ReturnsPaymentRejected_WhenPaymentAmountIsInvalid(
        decimal paymentAmount,
        [Frozen] IDataStore dataStore,
        PaymentService sut,
        IFixture fixture)
    {
        // Arrange
        var request = fixture.Build<MakePaymentRequest>()
            .With(x => x.Amount, paymentAmount)
            .Create();
        
        // Act
        var actual = sut.MakePayment(request);

        // Assert
        actual.IsSuccess.ShouldBeFalse();
        actual.Reason.ShouldNotBeNull();
        actual.Reason.ShouldBe("Invalid amount.");
        
        dataStore.DidNotReceive().UpdateAccount(Arg.Any<Account>());
    }
    
    [Theory]
    [CustomAutoData]
    internal void MakePayment_ReturnsPaymentRejected_WhenDebtorAccountNumberIsInvalid(
        PaymentService sut,
        [Frozen] IDataStore dataStore,
        IFixture fixture)
    {
        // Arrange
        var request = fixture.Build<MakePaymentRequest>()
            .With(x => x.DebtorAccountNumber, string.Empty)
            .Create();
        
        // Act
        var actual = sut.MakePayment(request);

        // Assert
        actual.IsSuccess.ShouldBeFalse();
        actual.Reason.ShouldNotBeNull();
        actual.Reason.ShouldBe("Invalid debtor account number.");

        dataStore.DidNotReceive().UpdateAccount(Arg.Any<Account>());
    }
    
    [Theory]
    [CustomAutoData]
    internal void MakePayment_ReturnsPaymentRejected_WhenDebtorAccountIsNotFound(
        MakePaymentRequest request,
        [Frozen] IDataStore dataStore,
        PaymentService sut)
    {
        // Arrange
        dataStore
            .GetAccount(request.DebtorAccountNumber)
            .Returns((Account)null);
        
        // Act
        var actual = sut.MakePayment(request);

        // Assert
        actual.IsSuccess.ShouldBeFalse();
        actual.Reason.ShouldNotBeNull();
        actual.Reason.ShouldBe("Account not found.");
        
        dataStore.DidNotReceive().UpdateAccount(Arg.Any<Account>());
        
    }

    [Theory]
    [CustomAutoData]
    internal void MakePayment_ReturnsPaymentRejected_WhenPolicyDecisionIsRejected(
        [Frozen] IDataStore dataStore,
        [Frozen] IPaymentSchemeRules paymentSchemeRules,
        [Frozen] IPaymentPolicy paymentPolicy,
        PaymentService sut,
        IFixture fixture)
    {
        // Arrange
        decimal paymentAmount = 10;
        decimal accountBalance = 100;

        var request = fixture
            .Build<MakePaymentRequest>()
            .With(x => x.Amount, paymentAmount)
            .Create();

        var account = fixture
            .Build<Account>()
            .With(x => x.Balance, accountBalance)
            .Create();

        dataStore.GetAccount(request.DebtorAccountNumber).Returns(account);
        paymentSchemeRules.For(request.PaymentScheme).Returns(paymentPolicy);

        paymentPolicy
            .Evaluate(account, request)
            .Returns(PaymentDecision.Reject(PaymentRejectionReason.SchemeNotAllowed));

        // Act
        var actual = sut.MakePayment(request);

        // Assert
        actual.IsSuccess.ShouldBeFalse();
        actual.Reason.ShouldBe(PaymentRejectionReason.SchemeNotAllowed.ToReasonString());

        dataStore.DidNotReceive().UpdateAccount(Arg.Any<Account>());
    }

    [Theory]
    [CustomAutoData]
    internal void MakePayment_ReturnsPaymentSuccess_WhenPolicyDecisionIsApproved(
        [Frozen] IDataStore dataStore,
        [Frozen] IPaymentSchemeRules paymentSchemeRules,
        [Frozen] IPaymentPolicy paymentPolicy,
        PaymentService sut,
        IFixture fixture)
    {
        // Arrange
        decimal paymentAmount = 10;
        decimal accountBalance = 100;

        var request = fixture
            .Build<MakePaymentRequest>()
            .With(x => x.Amount, paymentAmount)
            .Create();
        
        var account = fixture
            .Build<Account>()
            .With(x => x.Balance, accountBalance)
            .Create();
        
        dataStore.GetAccount(request.DebtorAccountNumber).Returns(account);
        paymentSchemeRules.For(request.PaymentScheme).Returns(paymentPolicy);
        
        paymentPolicy
            .Evaluate(account, request)
            .Returns(PaymentDecision.Approve());

        // Act
        var actual = sut.MakePayment(request);

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.Reason.ShouldBeNull();

        account.Balance.ShouldBe(90m);
        dataStore.Received(1).UpdateAccount(account);
    }
    
    [Theory]
    [CustomAutoData]
    internal void MakePayment_ReturnsPaymentSuccess_WhenAccountBalanceEqualsAmount(
        [Frozen] IDataStore dataStore,
        [Frozen] IPaymentSchemeRules paymentSchemeRules,
        [Frozen] IPaymentPolicy paymentPolicy,
        PaymentService sut,
        IFixture fixture)
    {
        // Arrange
        decimal paymentAmount = 10;
        decimal accountBalance = 10;

        var request = fixture
            .Build<MakePaymentRequest>()
            .With(x => x.Amount, paymentAmount)
            .Create();
        
        var account = fixture
            .Build<Account>()
            .With(x => x.Balance, accountBalance)
            .Create();
        
        dataStore.GetAccount(request.DebtorAccountNumber).Returns(account);
        paymentSchemeRules.For(request.PaymentScheme).Returns(paymentPolicy);
        
        paymentPolicy
            .Evaluate(account, request)
            .Returns(PaymentDecision.Approve());

        // Act
        var actual = sut.MakePayment(request);

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.Reason.ShouldBeNull();

        account.Balance.ShouldBe(0m);
        dataStore.Received(1).UpdateAccount(account);
    }
}