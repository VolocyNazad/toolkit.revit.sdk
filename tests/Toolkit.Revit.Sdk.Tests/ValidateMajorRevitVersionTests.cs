using Xunit;

namespace Toolkit.Revit.Sdk.Tests;

/// <summary>
/// Tests for the <c>ValidateMajorRevitVersion</c> target, which stops the build
/// if the Revit version could not be determined from the configuration name.
/// </summary>
public sealed class ValidateMajorRevitVersionTests
{
    [Fact]
    public void Build_UnresolvedRevitVersion_FailsWithError()
    {
        var (success, errors) = RunValidateTarget(revitVersion: "-1");

        Assert.False(success);
        Assert.Contains(errors, e => e.Contains("Target Revit version cannot be resolved", StringComparison.Ordinal));
    }

    [Fact]
    public void Build_ResolvedRevitVersion_Succeeds()
    {
        var (success, errors) = RunValidateTarget(revitVersion: "2025");

        Assert.True(success);
        Assert.Empty(errors);
    }

    private static (bool Success, List<string> Errors) RunValidateTarget(string revitVersion)
    {
        var xml = $"""
            <Project>
              <PropertyGroup>
                <RevitVersion>{revitVersion}</RevitVersion>
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
