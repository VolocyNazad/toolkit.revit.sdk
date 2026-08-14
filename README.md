# Revit.Sdk

[![Revit 2021-2027](https://img.shields.io/badge/Revit-2021–2027-green.svg)](https://www.autodesk.com/products/revit/overview)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.md)
[![VolocyNazad](https://img.shields.io/badge/VolocyNazad-blue.svg)](https://github.com/VolocyNazad)

MSBuild SDK для разработки расширений Autodesk Revit. Он сокращает настройку проекта плагина, выбирает целевую платформу для указанной версии Revit и автоматизирует типовые операции сборки.

## Возможности

- поддержка Revit 2021–2027;
- выбор целевой версии Revit через имя конфигурации сборки;
- автоматический выбор `net48` для Revit до 2024 года и `net8.0-windows` для Revit 2025–2026;
- константы условной компиляции для разных версий API: `VERSION_2025`, `IS2025`, `BEFORE2026`, `AFTER2024` и другие;
- исключение `RevitAPI.dll` и `RevitAPIUI.dll` из выходного каталога;
- запуск установленного Revit при отладке;
- генерация манифеста `.addin` для внешних приложений и команд;
- опциональное объединение зависимостей с помощью ILRepack.

## Подключение

Укажите пакет SDK и его версию в файле проекта:

```xml
<Project Sdk="VolocyNazad.Revit.Sdk/VERSION">
  <PropertyGroup>
    <Configurations>Debug_2024;Debug_2025;Release_2024;Release_2025</Configurations>
  </PropertyGroup>
</Project>
```

Замените `VERSION` на требуемую версию пакета. Имя конфигурации должно иметь формат `<Debug|Release>_<год>`, например `Debug_2025`. SDK извлекает год после символа `_` и использует его как целевую версию Revit.

## Генерация манифеста

Чтобы создавать `.addin` при сборке, включите `GenerateAddinOnBuild` и объявите классы расширения:

```xml
<PropertyGroup>
  <GenerateAddinOnBuild>true</GenerateAddinOnBuild>
  <AddinOutputDirectory>$(MSBuildProjectDirectory)\output</AddinOutputDirectory>
  <Authors>YOUR_VENDOR_ID</Authors>
</PropertyGroup>

<ItemGroup>
  <ExternalApplication Include="My application">
    <Name>My application</Name>
    <FullClassName>MyPlugin.Application</FullClassName>
  </ExternalApplication>

  <ExternalCommand Include="My command">
    <Name>My command</Name>
    <FullClassName>MyPlugin.Commands.MyCommand</FullClassName>
    <Text>Run command</Text>
    <VisibilityMode>AlwaysVisible</VisibilityMode>
  </ExternalCommand>
</ItemGroup>
```

Значение `Authors` используется как `VendorId` и `VendorDescription` в сгенерированном манифесте.

После сборки манифест будет создан по пути:

```text
<AddinOutputDirectory>/addins/<RevitVersion>/<AssemblyName>.addin
```

По умолчанию `AddinOutputDirectory` указывает на каталог проекта.

## Отладка в Revit

Добавьте свойство `LaunchRevit`, чтобы запускать Revit вместе с отладчиком:

```xml
<PropertyGroup>
  <LaunchRevit>true</LaunchRevit>
</PropertyGroup>
```

По умолчанию SDK запускает `C:\Program Files\Autodesk\Revit <год>\Revit.exe` с аргументом `/language ENG`. Путь и аргументы можно переопределить свойствами `StartProgram` и `StartArguments`.

## Объединение зависимостей

Для объединения библиотек в сборку плагина добавьте пакет ILRepack и включите переупаковку:

```xml
<PropertyGroup>
  <IsRepackable>true</IsRepackable>
  <RepackBinariesExcludes>RevitAPI.dll;RevitAPIUI.dll</RepackBinariesExcludes>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="ILRepack" Version="VERSION" PrivateAssets="all" />
</ItemGroup>
```

## Сборка SDK

```powershell
dotnet build Toolkit.Revit.Sdk.slnx -p:Platform=x64
```

NuGet-пакет создаётся автоматически в каталоге `artifacts`.

## Лицензия

Проект распространяется по лицензии [MIT](LICENSE.md).
