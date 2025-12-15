using System;
using System.Collections.Generic;
using System.Linq;
using ClearBank.DeveloperTest.PaymentPolicies;
using ClearBank.DeveloperTest.Tests.Common;
using ClearBank.DeveloperTest.Types;
using NSubstitute;
using Shouldly;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.PaymentPolicies;

public class PaymentSchemeRulesTests
{
    [Theory]
    [CustomAutoData]
    internal void For_ReturnsPolicy_WhenSchemeIsValidAndPolicyExists(
        PaymentScheme scheme)
    {
        // Arrange
        var policy = Substitute.For<IPaymentPolicy>();
        policy.Scheme.Returns(scheme);

        var sut = new PaymentSchemeRules(new[] { policy });

        // Act
        var actual = sut.For(scheme);

        // Assert
        actual.ShouldBeSameAs(policy);
    }

    [Theory]
    [CustomAutoData]
    internal void For_ThrowsInvalidOperationException_WhenSchemeIsValidButNoPolicyExists(
        PaymentScheme scheme)
    {
        // Arrange
        var sut = new PaymentSchemeRules(Array.Empty<IPaymentPolicy>());

        // Act
        var ex = Should.Throw<InvalidOperationException>(() => sut.For(scheme));

        // Assert
        ex.Message.ShouldBe($"No policy for {scheme}");
    }

    [Theory]
    [CustomAutoData]
    internal void For_ThrowsArgumentException_WhenSchemeIsNotDefined(
        PaymentSchemeRules sut)
    {
        // Arrange
        var maxDefined = Enum.GetValues(typeof(PaymentScheme)).Cast<PaymentScheme>().Max(s => Convert.ToInt32(s));
        var invalid = (PaymentScheme)(maxDefined + 1);

        // Act
        var ex = Should.Throw<ArgumentException>(() => sut.For(invalid));

        // Assert
        ex.Message.ShouldBe("Invalid scheme provided.");
    }

    [Theory]
    [CustomAutoData]
    internal void For_MinDefinedScheme_ReturnsPolicy()
    {
        // Arrange
        var minScheme = Enum.GetValues(typeof(PaymentScheme))
            .Cast<PaymentScheme>()
            .OrderBy(s => Convert.ToInt32(s))
            .First();

        var policy = Substitute.For<IPaymentPolicy>();
        policy.Scheme.Returns(minScheme);

        var sut = new PaymentSchemeRules(new[] { policy });

        // Act
        var actual = sut.For(minScheme);

        // Assert
        actual.ShouldBeSameAs(policy);
    }

    [Theory]
    [CustomAutoData]
    internal void For_MaxDefinedScheme_ReturnsPolicy()
    {
        // Arrange
        var maxScheme = Enum.GetValues(typeof(PaymentScheme))
            .Cast<PaymentScheme>()
            .OrderByDescending(s => Convert.ToInt32(s))
            .First();

        var policy = Substitute.For<IPaymentPolicy>();
        policy.Scheme.Returns(maxScheme);

        var sut = new PaymentSchemeRules(new[] { policy });

        // Act
        var actual = sut.For(maxScheme);

        // Assert
        actual.ShouldBeSameAs(policy);
    }

    [Theory]
    [CustomAutoData]
    internal void Ctor_ThrowsArgumentNullException_WhenPoliciesIsNull()
    {
        // Arrange
        IEnumerable<IPaymentPolicy> policies = null!;

        // Act
        var ex = Should.Throw<ArgumentNullException>(() => new PaymentSchemeRules(policies));

        // Assert
        ex.Message.ShouldContain("Value cannot be null.");
    }

    [Theory]
    [CustomAutoData]
    internal void Ctor_ThrowsArgumentException_WhenDuplicateSchemesProvided(
        PaymentScheme scheme)
    {
        // Arrange
        var policy1 = Substitute.For<IPaymentPolicy>();
        policy1.Scheme.Returns(scheme);

        var policy2 = Substitute.For<IPaymentPolicy>();
        policy2.Scheme.Returns(scheme);

        // Act
        Should.Throw<ArgumentException>(() => new PaymentSchemeRules(new[] { policy1, policy2 }));
    }

}