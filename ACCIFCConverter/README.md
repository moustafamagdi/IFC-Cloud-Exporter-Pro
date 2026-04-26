# ACC IFC Converter Pro

Production-oriented desktop application for Autodesk Construction Cloud/BIM360 Revit-to-IFC conversion workflows.

## Stack
- .NET 8
- WPF + MVVM (CommunityToolkit.Mvvm)
- APS OAuth + Data Management + Design Automation clients
- Serilog logging
- SQLite history storage
- Polly retry policies

## Solution Projects
- `ACCIFCConverter.UI` – WPF desktop shell and pages.
- `ACCIFCConverter.Application` – use cases and orchestration services.
- `ACCIFCConverter.Domain` – core entities and contracts.
- `ACCIFCConverter.Infrastructure` – APS clients, secure storage, scheduler integration, SQLite persistence.
- `ACCIFCConverter.Services` – cross-cutting worker/background services.
- `ACCIFCConverter.Tests` – tests.

## Setup
1. Open `ACCIFCConverter.sln` in Visual Studio 2022+.
2. Copy `appsettings.json.example` to `appsettings.json` in `ACCIFCConverter.UI`.
3. Fill APS credentials.
4. Build and run `ACCIFCConverter.UI`.

## Installer
Inno Setup script is available at `installer/ACCIFCConverter.iss`.
