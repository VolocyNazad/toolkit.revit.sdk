using Xunit;

namespace Toolkit.Revit.Sdk.Tests;

/// <summary>
/// Tests for generating condition constants in <c>SetupRevitDefineConstants.props</c>.
/// </summary>
public sealed class DefineConstantsTests
{
    [Fact]
    public void Evaluate_MajorRevitVersion_DefinesVersionAndIsConstants()
    {
        var constants = GetDefineConstants(majorRevitVersion: "2025", configuration: "Release");

        Assert.Contains("VERSION_2025", constants);
        Assert.Contains("IS2025", constants);
    }

    [Theory]
    // MajorRevitVersion=2025: BEFORE constants are defined inclusively for 2025 and later.
    [InlineData("2025", "BEFORE2025", true)]
    [InlineData("2025", "BEFORE2026", true)]
    [InlineData("2025", "BEFORE2024", false)]
    [InlineData("2025", "AFTER2025", true)]
    [InlineData("2025", "AFTER2024", true)]
    [InlineData("2025", "AFTER2026", false)]
    public void Evaluate_MajorRevitVersion_DefinesBeforeAfterConstantsInclusively(
        string majorRevitVersion, string expectedConstant, bool shouldBeDefined)
    {
        var constants = GetDefineConstants(majorRevitVersion, configuration: "Release");

        if (shouldBeDefined)
            Assert.Contains(expectedConstant, constants);
        else
            Assert.DoesNotContain(expectedConstant, constants);
    }

    [Theory]
    [InlineData("Release", "RELEASE", true)]
    [InlineData("Release", "DEBUG", false)]
    [InlineData("Debug", "DEBUG", true)]
    [InlineData("Debug", "RELEASE", false)]
    [InlineData("Debug_2025.0.0", "DEBUG", true)]
    [InlineData("Release_2025.0.0", "RELEASE", true)]
    public void Evaluate_Configuration_DefinesDebugOrReleaseConstant(
        string configuration, string expectedConstant, bool shouldBeDefined)
    {
        var constants = GetDefineConstants(majorRevitVersion: "2025", configuration);

        if (shouldBeDefined)
            Assert.Contains(expectedConstant, constants);
        else
            Assert.DoesNotContain(expectedConstant, constants);
    }

    private static string[] GetDefineConstants(string majorRevitVersion, string configuration)
    {
        var xml = $"""
            <Project>
              <PropertyGroup>
                <Configuration>{configuration}</Configuration>
                <MajorRevitVersion>{majorRevitVersion}</MajorRevitVersion>
              </PropertyGroup>
              <Import Project="{SdkPaths.File("SetupRevitDefineConstants.props")}" />
            </Project>
            """;

        var project = TestProjectFactory.CreateEvaluatedProject(xml);
        return project.GetPropertyValue("DefineConstants")
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
