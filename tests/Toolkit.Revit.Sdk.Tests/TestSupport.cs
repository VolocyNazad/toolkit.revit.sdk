using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace Toolkit.Revit.Sdk.Tests;

/// <summary>
/// Locates the directory with the SDK files (<c>src/Toolkit.Revit.Sdk/Sdk</c>) relative to the repository root,
/// so tests can import the real <c>.props</c>/<c>.targets</c> files instead of copies of them.
/// </summary>
internal static class SdkPaths
{
    private static readonly Lazy<string> _directory = new(FindSdkDirectory);

    /// <summary>
    /// Returns the full path to the SDK file with the given name (e.g. <c>Sdk.props</c>).
    /// </summary>
    /// <param name="fileName">The file name inside the <c>Sdk</c> directory.</param>
    public static string File(string fileName) => Path.Combine(_directory.Value, fileName);

    private static string FindSdkDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !System.IO.File.Exists(Path.Combine(directory.FullName, "Toolkit.Revit.Sdk.slnx")))
            directory = directory.Parent;

        if (directory is null)
            throw new InvalidOperationException(
                $"Could not find the repository root (Toolkit.Revit.Sdk.slnx) above '{AppContext.BaseDirectory}'.");

        return Path.Combine(directory.FullName, "src", "Toolkit.Revit.Sdk", "Sdk");
    }
}

/// <summary>
/// Creates temporary MSBuild projects for testing individual SDK <c>.props</c>/<c>.targets</c> files
/// in isolation - without a real C# code build or launching Revit.
/// </summary>
internal static class TestProjectFactory
{
    /// <summary>
    /// Evaluates the project (without running any targets) and returns the result for reading properties.
    /// </summary>
    /// <param name="projectXml">The full contents of the temporary .csproj file.</param>
    public static Project CreateEvaluatedProject(string projectXml)
    {
        var path = WriteTempProjectFile(projectXml);
        try
        {
            return new Project(path);
        }
        finally
        {
            System.IO.File.Delete(path);
        }
    }

    /// <summary>
    /// Creates an executable project instance for running a specific target.
    /// </summary>
    /// <param name="projectXml">The full contents of the temporary .csproj file.</param>
    public static ProjectInstance CreateProjectInstance(string projectXml)
    {
        var path = WriteTempProjectFile(projectXml);
        try
        {
            return new ProjectInstance(path);
        }
        finally
        {
            System.IO.File.Delete(path);
        }
    }

    private static string WriteTempProjectFile(string projectXml)
    {
        var path = Path.Combine(Path.GetTempPath(), $"toolkit-revit-sdk-test-{Guid.NewGuid():N}.csproj");
        System.IO.File.WriteAllText(path, projectXml);
        return path;
    }
}

/// <summary>
/// A simple MSBuild logger that accumulates error messages for verification in tests.
/// </summary>
internal sealed class InMemoryLogger : ILogger
{
    /// <summary>
    /// Returns the text of all errors logged during the build.
    /// </summary>
    public List<string> Errors { get; } = [];

    /// <inheritdoc/>
    public LoggerVerbosity Verbosity { get; set; } = LoggerVerbosity.Normal;

    /// <inheritdoc/>
    public string? Parameters { get; set; }

    /// <inheritdoc/>
    public void Initialize(IEventSource eventSource) =>
        eventSource.ErrorRaised += (_, e) => Errors.Add(e.Message ?? string.Empty);

    /// <inheritdoc/>
    public void Shutdown()
    {
    }
}
