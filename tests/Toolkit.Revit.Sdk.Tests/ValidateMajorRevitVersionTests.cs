using Xunit;

namespace Toolkit.Revit.Sdk.Tests;

/// <summary>
/// Tests for the <c>ValidateMajorRevitVersion</c> target, which stops the build
/// if the Revit version could not be determined from the configuration name.
/// </summary>
public sealed class ValidateMajorRevitVersionTests
{
    [Theory]
    [InlineData("-1")]
    [InlineData("")]
    [InlineData("2020")]
    [InlineData("2028")]
    [InlineData("R2026")]
    public void Build_UnsupportedRevitVersion_FailsWithError(string majorRevitVersion)
    {
        var (success, errors) = RunValidateTarget(majorRevitVersion);

        Assert.False(success);
        Assert.Contains(errors, e => e.Contains("is not supported", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("2021")]
    [InlineData("2022")]
    [InlineData("2023")]
    [InlineData("2024")]
    [InlineData("2025")]
    [InlineData("2026")]
    [InlineData("2027")]
    public void Build_SupportedRevitVersion_Succeeds(string majorRevitVersion)
    {
        var (success, errors) = RunValidateTarget(majorRevitVersion);

        Assert.True(success);
        Assert.Empty(errors);
    }

    private static (bool Success, List<string> Errors) RunValidateTarget(string majorRevitVersion)
    {
        var xml = $"""
            <Project>
              <PropertyGroup>
                <MajorRevitVersion>{majorRevitVersion}</MajorRevitVersion>
              </PropertyGroup>
              <Import Project="{SdkPaths.File("ValidateMajorRevitVersion.targets")}" />
            </Project>
            """;

        var projectInstance = TestProjectFactory.CreateProjectInstance(xml);
        var logger = new InMemoryLogger();

        var success = projectInstance.Build(["ValidateMajorRevitVersion"], [logger]);

        return (success, logger.Errors);
    }
}
