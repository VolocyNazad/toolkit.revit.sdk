namespace Toolkit.Revit.Sdk;

/// <summary>
/// Содержит данные внешней команды Revit.
/// </summary>
internal sealed class RevitCommandData : RevitManifestData
{
    /// <summary>
    /// Возвращает текст команды, отображаемый в интерфейсе Revit.
    /// </summary>
    public string? Text { get; init; }

    /// <summary>
    /// Возвращает режим видимости команды.
    /// </summary>
    public string? VisibilityMode { get; init; }
}
