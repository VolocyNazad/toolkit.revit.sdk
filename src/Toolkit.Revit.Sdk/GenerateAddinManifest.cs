using Microsoft.Build.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using Task = Microsoft.Build.Utilities.Task;

namespace Toolkit.Revit.Sdk;

/// <summary>
/// Generates a Revit extension manifest from MSBuild items.
/// </summary>
public sealed class GenerateAddinManifest : Task
{
    /// <summary>
    /// Gets or sets the path to the extension assembly.
    /// </summary>
    public string Assembly { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the extension vendor's identifier.
    /// </summary>
    public string VendorId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the extension vendor's description.
    /// </summary>
    public string VendorDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the target Revit version.
    /// </summary>
    public string RevitVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the project name used as the default for manifest items
    /// that do not have the <c>Name</c> metadata set.
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the external applications added to the manifest.
    /// </summary>
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "MSBuild task item parameters require arrays.")]
    public ITaskItem[] ExternalApplications { get; set; } = [];

    /// <summary>
    /// Gets or sets the external commands added to the manifest.
    /// </summary>
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "MSBuild task item parameters require arrays.")]
    public ITaskItem[] ExternalCommands { get; set; } = [];

    /// <summary>
    /// Gets or sets the output directory.
    /// </summary>
    public string Output { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the generated manifest.
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
    /// Returns the manifest item's name from the <c>Name</c> metadata, or the project name if it is not set.
    /// </summary>
    /// <param name="item">The MSBuild item representing a Revit application or command.</param>
    /// <returns>The resulting manifest item name.</returns>
    private string GetNameOrDefault(ITaskItem item)
    {
        var name = item.GetMetadata("Name");
        return string.IsNullOrEmpty(name) ? ProjectName : name;
    }

    /// <summary>
    /// Builds the manifest XML document for the given applications and commands.
    /// </summary>
    /// <param name="apps">The Revit external applications.</param>
    /// <param name="commands">The Revit external commands.</param>
    /// <returns>The generated manifest XML document.</returns>
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
