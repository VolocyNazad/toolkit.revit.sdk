namespace Toolkit.Revit.Sdk;

/// <summary>
/// Holds the data for a Revit external command.
/// </summary>
internal sealed class RevitCommandData : RevitManifestData
{
    /// <summary>
    /// Gets the command text displayed in the Revit UI.
    /// </summary>
    public string? Text { get; init; }

    /// <summary>
    /// Gets the command's visibility mode.
    /// </summary>
    public string? VisibilityMode { get; init; }
}
