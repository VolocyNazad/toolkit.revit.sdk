# Revit.Sdk

[![Revit 2021-2027](https://img.shields.io/badge/Revit-2021–2027-green.svg)](https://www.autodesk.com/products/revit/overview)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.md)
[![VolocyNazad](https://img.shields.io/badge/VolocyNazad-blue.svg)](https://github.com/VolocyNazad)

An MSBuild SDK for developing Autodesk Revit extensions. It streamlines plugin project setup, selects the target platform for a given Revit version, and automates common build operations.

## Features

- support for Revit 2021–2027;
- selecting the target Revit version via the build configuration name;
- automatic selection of `net48` for Revit before 2024 and `net8.0-windows` for Revit 2025–2026;
- conditional compilation constants for different API versions: `VERSION_2025`, `IS2025`, `BEFORE2026`, `AFTER2024`, and others;
- excluding `RevitAPI.dll` and `RevitAPIUI.dll` from the output directory;
- launching the installed Revit during debugging;
- generating an `.addin` manifest for external applications and commands;
- optional dependency merging via ILRepack.

## Usage

Specify the SDK package and its version in the project file:

```xml
<Project Sdk="VolocyNazad.Revit.Sdk/VERSION">
  <PropertyGroup>
    <Configurations>Debug_2024;Debug_2025;Release_2024;Release_2025</Configurations>
  </PropertyGroup>
</Project>
```

Replace `VERSION` with the package version you need. The configuration name must follow the `<Debug|Release>_<year>` format, e.g. `Debug_2025`. The SDK extracts the year after the `_` character and uses it as the target Revit version.

## Manifest generation

To generate `.addin` on build, enable `GenerateAddinOnBuild` and declare the extension classes:

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

The `Authors` value is used as `VendorId` and `VendorDescription` in the generated manifest.

After the build, the manifest will be created at:

```text
<AddinOutputDirectory>/addins/<RevitVersion>/<AssemblyName>.addin
```

By default, `AddinOutputDirectory` points to the project directory.

## Debugging in Revit

Add the `LaunchRevit` property to launch Revit together with the debugger:

```xml
<PropertyGroup>
  <LaunchRevit>true</LaunchRevit>
</PropertyGroup>
```

By default, the SDK launches `C:\Program Files\Autodesk\Revit <year>\Revit.exe` with the `/language ENG` argument. The path and arguments can be overridden via the `StartProgram` and `StartArguments` properties.

## Dependency merging

To merge libraries into the plugin assembly, add the ILRepack package and enable repacking:

```xml
<PropertyGroup>
  <IsRepackable>true</IsRepackable>
  <RepackBinariesExcludes>RevitAPI.dll;RevitAPIUI.dll</RepackBinariesExcludes>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="ILRepack" Version="VERSION" PrivateAssets="all" />
</ItemGroup>
```

## Building the SDK

```powershell
dotnet build Toolkit.Revit.Sdk.slnx -p:Platform=x64
```

The NuGet package is created automatically in the `artifacts` directory.

## License

The project is distributed under the [MIT](LICENSE.md) license.

## Development documentation

- [Development policy](docs/policies/development.md)
- [Repository guide and technology stack](docs/repository.md)

## Contributing

Read [CONTRIBUTING.md](CONTRIBUTING.md) before submitting changes.
