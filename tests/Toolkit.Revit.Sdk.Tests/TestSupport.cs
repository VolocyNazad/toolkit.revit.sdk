using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace Toolkit.Revit.Sdk.Tests;

/// <summary>
/// Находит каталог с файлами SDK (<c>src/Toolkit.Revit.Sdk/Sdk</c>) относительно корня репозитория,
/// чтобы тесты могли импортировать реальные <c>.props</c>/<c>.targets</c>, а не их копии.
/// </summary>
internal static class SdkPaths
{
    private static readonly Lazy<string> _directory = new(FindSdkDirectory);

    /// <summary>
    /// Возвращает полный путь к файлу SDK с указанным именем (например, <c>Sdk.props</c>).
    /// </summary>
    /// <param name="fileName">Имя файла внутри каталога <c>Sdk</c>.</param>
    public static string File(string fileName) => Path.Combine(_directory.Value, fileName);

    private static string FindSdkDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !System.IO.File.Exists(Path.Combine(directory.FullName, "Toolkit.Revit.Sdk.slnx")))
            directory = directory.Parent;

        if (directory is null)
            throw new InvalidOperationException(
                $"Не удалось найти корень репозитория (Toolkit.Revit.Sdk.slnx) выше '{AppContext.BaseDirectory}'.");

        return Path.Combine(directory.FullName, "src", "Toolkit.Revit.Sdk", "Sdk");
    }
}

/// <summary>
/// Создаёт временные MSBuild-проекты для проверки отдельных <c>.props</c>/<c>.targets</c> файлов SDK
/// в изоляции - без реальной сборки C#-кода или запуска Revit.
/// </summary>
internal static class TestProjectFactory
{
    /// <summary>
    /// Оценивает проект (без выполнения таргетов) и возвращает результат для чтения свойств.
    /// </summary>
    /// <param name="projectXml">Полное содержимое временного .csproj-файла.</param>
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
    /// Создаёт исполняемый экземпляр проекта для запуска конкретного таргета.
    /// </summary>
    /// <param name="projectXml">Полное содержимое временного .csproj-файла.</param>
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
/// Простой логгер MSBuild, накапливающий сообщения об ошибках для проверки в тестах.
/// </summary>
internal sealed class InMemoryLogger : ILogger
{
    /// <summary>
    /// Возвращает тексты всех ошибок, залогированных во время сборки.
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
