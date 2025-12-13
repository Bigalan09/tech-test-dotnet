using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace ClearBank.DeveloperTest.Tests.Common;

public static class FixtureHelper
{
    public static IFixture CreateFixture()
    {
        var fixture = new Fixture();

        fixture.Customize(new AutoNSubstituteCustomization
        {
            ConfigureMembers = true,
            GenerateDelegates = true
        });

        return fixture;
    }
}