namespace Toolkit.Revit.Sdk;

/// <summary>
/// Holds the common data for a Revit manifest item.
/// </summary>
internal class RevitManifestData
{
    /// <summary>
    /// Gets the unique identifier of the extension.
    /// </summary>
    public Guid AddInId { get; } = Guid.NewGuid();

    /// <summary>
    /// Gets the extension name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the path to the extension assembly.
    /// </summary>
    public required string Assembly { get; init; }

    /// <summary>
    /// Gets the extension's full class name.
    /// </summary>
    public required string FullClassName { get; init; }

    /// <summary>
    /// Gets the extension vendor's identifier.
    /// </summary>
    public required string VendorId { get; init; }

    /// <summary>
    /// Gets the extension vendor's description.
    /// </summary>
    public required string VendorDescription { get; init; }
}
