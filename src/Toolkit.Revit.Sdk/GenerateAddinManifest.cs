using Microsoft.Build.Framework;
using System.Xml.Linq;
using Task = Microsoft.Build.Utilities.Task;

namespace Toolkit.Revit.Sdk;

/// <summary>
/// Создаёт манифест расширения Revit на основе элементов MSBuild.
/// </summary>
public sealed class GenerateAddinManifest : Task
{
    /// <summary>
    /// Возвращает или задаёт путь к сборке расширения.
    /// </summary>
    public string Assembly { get; set; } = string.Empty;

    /// <summary>
    /// Возвращает или задаёт идентификатор разработчика расширения.
    /// </summary>
    public string VendorId { get; set; } = string.Empty;

    /// <summary>
    /// Возвращает или задаёт описание разработчика расширения.
    /// </summary>
    public string VendorDescription { get; set; } = string.Empty;

    /// <summary>
    /// Возвращает или задаёт целевую версию Revit.
    /// </summary>
    public string RevitVersion { get; set; } = string.Empty;

    /// <summary>
    /// Возвращает или задаёт имя проекта, используемое по умолчанию для элементов манифеста,
    /// у которых не задано метаданное <c>Name</c>.
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Возвращает или задаёт внешние приложения, добавляемые в манифест.
    /// </summary>
    public ITaskItem[] ExternalApplications { get; set; } = [];

    /// <summary>
    /// Возвращает или задаёт внешние команды, добавляемые в манифест.
    /// </summary>
    public ITaskItem[] ExternalCommands { get; set; } = [];

    /// <summary>
    /// Возвращает или задаёт выходной каталог.
    /// </summary>
    public string Output { get; set; } = string.Empty;

    /// <summary>
    /// Возвращает или задаёт путь к сгенерированному манифесту.
    /// </summary>
    [Output]
    public string? ManifestPath { get; set; }

    /// <inheritdoc/>
    public override bool Execute()
    {
        List<RevitApplicationData> apps = [];
        foreach (var app in ExternalApplications)
        {
            var name = GetNameOrDefault(app);
            RevitApplicationData data = new()
            {
                Name = name,
                Assembly = Assembly,
                FullClassName = app.GetMetadata("FullClassName"),
                VendorId = VendorId,
                VendorDescription = VendorDescription,
            };
            apps.Add(data);
        }

        List<RevitCommandData> commands = [];
        foreach (var cmd in ExternalCommands)
        {
            var name = GetNameOrDefault(cmd);
            var text = cmd.GetMetadata("Text");
            RevitCommandData data = new()
            {
                Name = name,
                Assembly = Assembly,
                FullClassName = cmd.GetMetadata("FullClassName"),
                VendorId = VendorId,
                VendorDescription = VendorDescription,
                VisibilityMode = cmd.GetMetadata("VisibilityMode"),
                Text = string.IsNullOrEmpty(text) ? name : text,
            };
            commands.Add(data);
        }

        var directoryOutputPath = Path.Combine(Output, "addins", RevitVersion);
        if (!Directory.Exists(directoryOutputPath))
            Directory.CreateDirectory(directoryOutputPath);
        var addinFileName = Path.GetFileNameWithoutExtension(Assembly);
        var outputPath = Path.Combine(directoryOutputPath, $"{addinFileName}.addin");

        XDocument doc = GenerateAddInFile(apps, commands);
        doc.Save(outputPath);

        ManifestPath = outputPath;

        return true;
    }

    /// <summary>
    /// Возвращает имя элемента манифеста из метаданных <c>Name</c>, либо, если оно не задано, имя проекта.
    /// </summary>
    /// <param name="item">Элемент MSBuild, представляющий приложение или команду Revit.</param>
    /// <returns>Итоговое имя элемента манифеста.</returns>
    private string GetNameOrDefault(ITaskItem item)
    {
        var name = item.GetMetadata("Name");
        return string.IsNullOrEmpty(name) ? ProjectName : name;
    }

    /// <summary>
    /// Создаёт XML-документ манифеста для указанных приложений и команд.
    /// </summary>
    /// <param name="apps">Внешние приложения Revit.</param>
    /// <param name="commands">Внешние команды Revit.</param>
    /// <returns>Сформированный XML-документ манифеста.</returns>
    private static XDocument GenerateAddInFile(List<RevitApplicationData> apps, List<RevitCommandData> commands)
    {
        XElement root = new("RevitAddIns");

        foreach (RevitApplicationData app in apps) {
            var appElement = new XElement("AddIn");

            appElement.SetAttributeValue("Type", "Application");
            appElement.Add(new XElement("Name", app.Name));
            appElement.Add(new XElement("Assembly", app.Assembly));
            appElement.Add(new XElement("AddInId", app.AddInId));
            appElement.Add(new XElement("FullClassName", app.FullClassName));
            appElement.Add(new XElement("VendorId", app.VendorId));
            appElement.Add(new XElement("VendorDescription", app.VendorDescription));

            root.Add(appElement);
        }

        foreach (RevitCommandData command in commands) {
            var commandElement = new XElement("AddIn");

            commandElement.SetAttributeValue("Type", "Command");
            commandElement.Add(new XElement("Name", command.Name));
            commandElement.Add(new XElement("Assembly", command.Assembly));
            commandElement.Add(new XElement("AddInId", command.AddInId));
            commandElement.Add(new XElement("FullClassName", command.FullClassName));
            commandElement.Add(new XElement("VendorId", command.VendorId));
            commandElement.Add(new XElement("VendorDescription", command.VendorDescription));
            commandElement.Add(new XElement("Text", command.Text));
            commandElement.Add(new XElement("VisibilityMode", command.VisibilityMode));

            root.Add(commandElement);
        }

        var declaration = new XDeclaration("1.0", "utf-8", null);

        var doc = new XDocument(declaration, root);

        return doc;
    }
}
