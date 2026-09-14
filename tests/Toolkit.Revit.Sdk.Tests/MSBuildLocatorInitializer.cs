using System.Runtime.CompilerServices;
using Microsoft.Build.Locator;

namespace Toolkit.Revit.Sdk.Tests;

/// <summary>
/// Registers the real, installed MSBuild assembly before <c>Microsoft.Build.*</c> types
/// are first accessed in tests. Without this, the evaluation/execution API cannot
/// resolve the <c>Sdk="Microsoft.NET.Sdk"</c> import and fails with an SDK resolution error.
/// </summary>
internal static class MSBuildLocatorInitializer
{
    /// <summary>
    /// Runs when the runtime loads the module - before any test method.
    /// </summary>
    [ModuleInitializer]
    public static void Initialize()
    {
        if (!MSBuildLocator.IsRegistered)
            MSBuildLocator.RegisterDefaults();
    }
}
