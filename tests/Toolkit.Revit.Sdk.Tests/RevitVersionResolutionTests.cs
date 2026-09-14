using Xunit;

namespace Toolkit.Revit.Sdk.Tests;

/// <summary>
/// Tests for parsing the Revit version and selecting the target framework in <c>RevitVersionResolution.props</c>.
/// </summary>
public sealed class RevitVersionResolutionTests
{
    [Theory]
    [InlineData("Release_2025.0.0", "2025")]
    [InlineData("Debug_2026.1.0", "2026")]
    [InlineData("Release_2024.0.0", "2024")]
    [InlineData("Debug_2022.0.0", "2022")]
    public void Evaluate_ConfigurationEncodesVersion_ResolvesMajorRevitVersion(string configuration, string expectedMajorVersion)
    {
        var project = LoadRevitVersionResolutionProps(configuration);

        Assert.Equal(expectedMajorVersion, project.GetPropertyValue("MajorRevitVersion"));
    }

    [Theory]
    [InlineData("Release_2025.0.0", "net8.0-windows")]
    [InlineData("Release_2026.0.0", "net8.0-windows")]
    [InlineData("Release_2024.0.0", "net48")]
    [InlineData("Release_2021.0.0", "net48")]
    [InlineData("Release_2027.0.0", "net48")]
    public void Evaluate_MajorRevitVersion_SelectsTargetFramework(string configuration, string expectedTargetFramework)
    {
        var project = LoadRevitVersionResolutionProps(configuration);

        Assert.Equal(expectedTargetFramework, project.GetPropertyValue("TargetFramework"));
    }

    [Fact]
    public void Evaluate_DebugConfiguration_DisablesOptimizationAndKeepsSymbols()
    {
        var project = LoadRevitVersionResolutionProps("Debug_2025.0.0");

        Assert.Equal("False", project.GetPropertyValue("Optimize"));
        Assert.Equal("portable", project.GetPropertyValue("DebugType"));
    }

    [Fact]
    public void Evaluate_ReleaseConfiguration_EnablesOptimizationAndDropsSymbols()
    {
        var project = LoadRevitVersionResolutionProps("Release_2025.0.0");

        Assert.Equal("True", project.GetPropertyValue("Optimize"));
        Assert.Equal("none", project.GetPropertyValue("DebugType"));
        Assert.Equal("false", project.GetPropertyValue("DebugSymbols"));
    }

    [Fact]
    public void Evaluate_Net8TargetFramework_EnablesCopyLocalLockFileAssemblies()
    {
        var project = LoadRevitVersionResolutionProps("Release_2025.0.0");

        Assert.Equal("true", project.GetPropertyValue("CopyLocalLockFileAssemblies"));
    }

    [Fact]
    public void Evaluate_Net48TargetFramework_DoesNotForceCopyLocalLockFileAssemblies()
    {
        var project = LoadRevitVersionResolutionProps("Release_2024.0.0");

        Assert.NotEqual("true", project.GetPropertyValue("CopyLocalLockFileAssemblies"), StringComparer.OrdinalIgnoreCase);
    }

    private static Microsoft.Build.Evaluation.Project LoadRevitVersionResolutionProps(string configuration)
    {
        var xml = $"""
            <Project>
              <PropertyGroup>
                <Configuration>{configuration}</Configuration>
              </PropertyGroup>
              <Import Project="{SdkPaths.File("RevitVersionResolution.props")}" />
            </Project>
            """;

        return TestProjectFactory.CreateEvaluatedProject(xml);
    }
}
