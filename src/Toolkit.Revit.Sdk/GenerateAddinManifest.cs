using Microsoft.Build.Framework;
using System.Xml.Linq;
using Task = Microsoft.Build.Utilities.Task;

namespace Toolkit.Revit.Sdk;

public sealed class GenerateAddinManifest : Task
{
    // Входные параметры из MSBuild
    public string Assembly { get; set; } = string.Empty;
    public string VendorId { get; set; } = string.Empty;
    public string VendorDescription { get; set; } = string.Empty;
    public string RevitVersion { get; set; } = string.Empty;
    public ITaskItem[] ExternalApplications { get; set; } = [];
    public ITaskItem[] ExternalCommands { get; set; } = [];
    public string Output { get; set; } = string.Empty;

    /// <summary>
    /// Путь к сгенерированному манифест (Выходной параметр)
    /// </summary>
    [Output]
    public string? ManifestPath { get; set; }

    /// <inheritdoc/>
    public override bool Execute()
    {
        List<RevitApplicationData> apps = [];
        foreach (var app in ExternalApplications)
        {
            RevitApplicationData data = new()
            {
                Name = app.GetMetadata("Name"),
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
            RevitCommandData data = new()
            {
                Name = cmd.GetMetadata("Name"),
                Assembly = Assembly,
                FullClassName = cmd.GetMetadata("FullClassName"),
                VendorId = VendorId,
                VendorDescription = VendorDescription,
                VisibilityMode = cmd.GetMetadata("VisibilityMode"),
                Text = cmd.GetMetadata("Text"),
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

    XDocument GenerateAddInFile(List<RevitApplicationData> apps, List<RevitCommandData> commands)
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