using Xunit;

namespace Toolkit.Revit.Sdk.Tests;

/// <summary>
/// Tests for the <c>RemoveRevitAPICopyLocal</c> target, which excludes <c>RevitAPI.dll</c>/<c>RevitAPIUI.dll</c>
/// from the dependencies copied on build (these assemblies are provided by Revit itself).
/// </summary>
public sealed class RemoveRevitApiCopyLocalTests
{
    [Fact]
    public void Build_RemovesRevitApiAssembliesButKeepsOtherReferences()
    {
        var xml = $"""
            <Project>
              <ItemGroup>
                <ReferenceCopyLocalPaths Include="C:\refs\RevitAPI.dll" />
                <ReferenceCopyLocalPaths Include="C:\refs\RevitAPIUI.dll" />
                <ReferenceCopyLocalPaths Include="C:\refs\Newtonsoft.Json.dll" />
              </ItemGroup>
              <Import Project="{SdkPaths.File("RemoveRevitAPICopyLocal.targets")}" />
            </Project>
            """;

        var projectInstance = TestProjectFactory.CreateProjectInstance(xml);

        var success = projectInstance.Build(["RemoveRevitAPICopyLocal"], []);

        Assert.True(success);
        var remaining = projectInstance.GetItems("ReferenceCopyLocalPaths")
            .Select(item => Path.GetFileName(item.EvaluatedInclude))
            .ToList();

        Assert.DoesNotContain("RevitAPI.dll", remaining);
        Assert.DoesNotContain("RevitAPIUI.dll", remaining);
        Assert.Contains("Newtonsoft.Json.dll", remaining);
    }
}
