namespace Toolkit.Revit.Sdk;

/// <summary>
/// Содержит общие данные элемента манифеста Revit.
/// </summary>
internal class RevitManifestData
{
    /// <summary>
    /// Возвращает уникальный идентификатор расширения.
    /// </summary>
    public Guid AddInId { get; } = Guid.NewGuid();

    /// <summary>
    /// Возвращает имя расширения.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Возвращает путь к сборке расширения.
    /// </summary>
    public required string Assembly { get; init; }

    /// <summary>
    /// Возвращает полное имя класса расширения.
    /// </summary>
    public required string FullClassName { get; init; }

    /// <summary>
    /// Возвращает идентификатор разработчика расширения.
    /// </summary>
    public required string VendorId { get; init; }

    /// <summary>
    /// Возвращает описание разработчика расширения.
    /// </summary>
    public required string VendorDescription { get; init; }
}
