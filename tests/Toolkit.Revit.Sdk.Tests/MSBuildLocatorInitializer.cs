using System.Runtime.CompilerServices;
using Microsoft.Build.Locator;

namespace Toolkit.Revit.Sdk.Tests;

/// <summary>
/// Регистрирует реальную установленную сборку MSBuild до первого обращения к типам
/// <c>Microsoft.Build.*</c> в тестах. Без этого evaluation/execution API не могут
/// разрешить импорт <c>Sdk="Microsoft.NET.Sdk"</c> и падают с ошибкой поиска SDK.
/// </summary>
internal static class MSBuildLocatorInitializer
{
    /// <summary>
    /// Выполняется средой выполнения при загрузке модуля - раньше, чем любой тестовый метод.
    /// </summary>
    [ModuleInitializer]
    public static void Initialize()
    {
        if (!MSBuildLocator.IsRegistered)
            MSBuildLocator.RegisterDefaults();
    }
}
