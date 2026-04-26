# ACC IFC Converter Pro

ACC IFC Converter Pro is a Windows WPF desktop application for exporting ACC/BIM360 hosted Revit models to IFC using Autodesk Platform Services-oriented workflows.

## Implemented modules
- OAuth desktop login plumbing (browser + localhost callback + refresh token flow).
- ACC browser service layer with hub/project/folder/file traversal model.
- Export queue with status transitions (Waiting/Uploading/Processing/Completed/Failed).
- Export options foundation (preset + JSON override + mapping flags).
- Output naming engine with live preview token replacement.
- SQLite history persistence for completed/failed jobs.
- DPAPI-backed secure settings store for local credentials.
- Windows Task Scheduler integration wrapper (daily/weekly/monthly).
- Serilog rolling file logs under `Logs/log-yyyyMMdd.txt`.
- Inno Setup installer script template.

## Solution layout
- `ACCIFCConverter.UI` (WPF + MVVM shell, pages, commands)
- `ACCIFCConverter.Application` (queue/orchestration business logic)
- `ACCIFCConverter.Domain` (contracts + models)
- `ACCIFCConverter.Infrastructure` (APS clients, storage, DB, scheduler)
- `ACCIFCConverter.Services` (background worker hooks)
- `ACCIFCConverter.Tests` (unit tests)

## Run in Visual Studio 2022+
1. Open `ACCIFCConverter.sln`.
2. Copy `appsettings.json.example` to `ACCIFCConverter.UI/appsettings.json` and provide APS values.
3. Build solution for `x64`.
4. Run `ACCIFCConverter.UI`.

## Notes
- Current implementation includes production-ready structure and realistic service behavior with local/mock export output, so you can run and test the workflow without requiring APS setup first.
- Replace stubbed portions in `Infrastructure/Aps/*` with real APS Data Management and Design Automation calls when wiring production credentials and app bundle IDs.
