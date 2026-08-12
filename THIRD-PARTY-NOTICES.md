# Third-party notices

MdLight does not bundle third-party runtime libraries. It relies on the .NET
Framework 4.8 components supplied with supported versions of Windows.

The following tools and packages are used to build or package releases:

## Microsoft.NETFramework.ReferenceAssemblies 1.0.3

Build-only reference assemblies distributed by Microsoft under the MIT
License. The package is not included in the installed application.

- Project: https://github.com/microsoft/dotnet
- Package: https://www.nuget.org/packages/Microsoft.NETFramework.ReferenceAssemblies

## Inno Setup 6

The Windows installer is produced with Inno Setup. Its redistributable setup
runtime is included in `MdLight-Setup.exe` under the Inno Setup license.

- Project and license: https://jrsoftware.org/isinfo.php
- Source: https://github.com/jrsoftware/issrc

The installer includes the official Simplified Chinese Inno Setup message file
from that project. It is used only by Setup and remains covered by the Inno
Setup license.
