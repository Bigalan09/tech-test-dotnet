using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using ClearBank.DeveloperTest.Data;
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
        PaymentService sut)
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "",
            CreditorAccountNumber = "",
            Amount = paymentAmount,
            PaymentScheme = PaymentScheme.FasterPayments,
            PaymentDate = DateTime.Now,
        };
        
        // Act
        var actual = sut.MakePayment(request);

        // Assert
        actual.IsSuccess.ShouldBeFalse();
        actual.Reason.ShouldNotBeNull();
        actual.Reason.ShouldBe("Invalid amount.");
    }
    
    [Theory]
    [CustomAutoData]
    internal void MakePayment_ReturnsPaymentRejected_WhenDebtorAccountNumberIsInvalid(
        PaymentService sut)
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            DebtorAccountNumber = "",
            CreditorAccountNumber = "",
            Amount = 1,
            PaymentScheme = PaymentScheme.FasterPayments,
            PaymentDate = DateTime.Now,
        };
        
        // Act
        var actual = sut.MakePayment(request);

        // Assert
        actual.IsSuccess.ShouldBeFalse();
        actual.Reason.ShouldNotBeNull();
        actual.Reason.ShouldBe("Invalid debtor account number.");
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
    }
}