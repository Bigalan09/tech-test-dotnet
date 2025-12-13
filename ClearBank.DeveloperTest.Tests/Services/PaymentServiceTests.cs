using System;
using System.Collections.Generic;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Tests.Common;
using ClearBank.DeveloperTest.Types;
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
}