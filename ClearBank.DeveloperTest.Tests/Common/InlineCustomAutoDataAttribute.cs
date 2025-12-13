using System;
using AutoFixture.Xunit2;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Common;

public sealed class InlineCustomAutoDataAttribute(params object[] arguments)
    : InlineAutoDataAttribute(FixtureHelper.CreateFixture, arguments);
