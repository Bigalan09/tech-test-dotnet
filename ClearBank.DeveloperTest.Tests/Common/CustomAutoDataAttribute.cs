using AutoFixture.Xunit2;

namespace ClearBank.DeveloperTest.Tests.Common;

public sealed class CustomAutoDataAttribute() : AutoDataAttribute(FixtureHelper.CreateFixture);
