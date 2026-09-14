using Microsoft.Build.Utilities;
using System.Xml.Linq;
using Toolkit.Revit.Sdk;
using Xunit;

namespace Toolkit.Revit.Sdk.Tests;

/// <summary>
/// Tests for <see cref="GenerateAddinManifest"/>.
/// </summary>
public sealed class GenerateAddinManifestTests : IDisposable
{
    private readonly string _outputDirectory;

    public GenerateAddinManifestTests()
    {
        _outputDirectory = Path.Combine(Path.GetTempPath(), "Toolkit.Revit.Sdk.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_outputDirectory);
    }

    public void Dispose()
    {
        if (Directory.Exists(_outputDirectory))
            Directory.Delete(_outputDirectory, recursive: true);
    }

    private GenerateAddinManifest CreateTask(string projectName, TaskItem[] applications, TaskItem[] commands) =>
        new()
        {
            Assembly = "MyAddin.dll",
            VendorId = "VolocyNazad",
            VendorDescription = "VolocyNazad",
            RevitVersion = "2025",
            ProjectName = projectName,
            ExternalApplications = applications,
            ExternalCommands = commands,
            Output = _outputDirectory,
        };

    [Fact]
    public void Execute_ApplicationWithoutName_FallsBackToProjectName()
    {
        var app = new TaskItem("HostApp");
        app.SetMetadata("FullClassName", "MyAddin.ExternalApplication");

        var task = CreateTask("MyAddin", [app], []);

        var result = task.Execute();

        Assert.True(result);
        var manifest = LoadManifest(task.ManifestPath);
        var name = manifest.Root!.Element("AddIn")!.Element("Name")!.Value;
        Assert.Equal("MyAddin", name);
    }

    [Fact]
    public void Execute_ApplicationWithExplicitName_KeepsExplicitName()
    {
        var app = new TaskItem("HostApp");
        app.SetMetadata("FullClassName", "MyAddin.ExternalApplication");
        app.SetMetadata("Name", "Explicit_Host_Name");

        var task = CreateTask("MyAddin", [app], []);

        var result = task.Execute();

        Assert.True(result);
        var manifest = LoadManifest(task.ManifestPath);
        var name = manifest.Root!.Element("AddIn")!.Element("Name")!.Value;
        Assert.Equal("Explicit_Host_Name", name);
    }

    [Fact]
    public void Execute_CommandWithoutNameOrText_BothFallBackToProjectName()
    {
        var cmd = new TaskItem("MyCommand");
        cmd.SetMetadata("FullClassName", "MyAddin.ExternalCommand");
        cmd.SetMetadata("VisibilityMode", "AlwaysVisible");

        var task = CreateTask("MyAddin", [], [cmd]);

        var result = task.Execute();

        Assert.True(result);
        var addIn = LoadManifest(task.ManifestPath).Root!.Element("AddIn")!;
        Assert.Equal("MyAddin", addIn.Element("Name")!.Value);
        Assert.Equal("MyAddin", addIn.Element("Text")!.Value);
    }

    [Fact]
    public void Execute_CommandWithNameButNoText_TextFallsBackToName()
    {
        var cmd = new TaskItem("MyCommand");
        cmd.SetMetadata("FullClassName", "MyAddin.ExternalCommand");
        cmd.SetMetadata("Name", "1_My_Command");
        cmd.SetMetadata("VisibilityMode", "AlwaysVisible");

        var task = CreateTask("MyAddin", [], [cmd]);

        var result = task.Execute();

        Assert.True(result);
        var addIn = LoadManifest(task.ManifestPath).Root!.Element("AddIn")!;
        Assert.Equal("1_My_Command", addIn.Element("Name")!.Value);
        Assert.Equal("1_My_Command", addIn.Element("Text")!.Value);
    }

    [Fact]
    public void Execute_CommandWithExplicitNameAndText_KeepsBoth()
    {
        var cmd = new TaskItem("MyCommand");
        cmd.SetMetadata("FullClassName", "MyAddin.ExternalCommand");
        cmd.SetMetadata("Name", "1_My_Command");
        cmd.SetMetadata("Text", "Run My Command");
        cmd.SetMetadata("VisibilityMode", "AlwaysVisible");

        var task = CreateTask("MyAddin", [], [cmd]);

        var result = task.Execute();

        Assert.True(result);
        var addIn = LoadManifest(task.ManifestPath).Root!.Element("AddIn")!;
        Assert.Equal("1_My_Command", addIn.Element("Name")!.Value);
        Assert.Equal("Run My Command", addIn.Element("Text")!.Value);
    }

    [Fact]
    public void Execute_MultipleItems_AssignsDistinctAddInIds()
    {
        var command1 = new TaskItem("Command1");
        command1.SetMetadata("FullClassName", "MyAddin.Command1");
        command1.SetMetadata("Name", "1_Command_One");

        var command2 = new TaskItem("Command2");
        command2.SetMetadata("FullClassName", "MyAddin.Command2");
        command2.SetMetadata("Name", "2_Command_Two");

        var task = CreateTask("MyAddin", [], [command1, command2]);

        var result = task.Execute();

        Assert.True(result);
        var ids = LoadManifest(task.ManifestPath).Root!.Elements("AddIn")
            .Select(e => e.Element("AddInId")!.Value)
            .ToList();
        Assert.Equal(2, ids.Distinct().Count());
    }

    private static XDocument LoadManifest(string? manifestPath)
    {
        Assert.False(string.IsNullOrEmpty(manifestPath));
        Assert.True(File.Exists(manifestPath));
        return XDocument.Load(manifestPath!);
    }
}
