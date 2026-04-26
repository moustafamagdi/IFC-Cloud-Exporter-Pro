[Setup]
AppId={{E2B09138-96CE-4F4D-9DF2-DF0D017DAD87}
AppName=ACC IFC Converter Pro
AppVersion=1.0.0
DefaultDirName={autopf}\ACC IFC Converter Pro
DefaultGroupName=ACC IFC Converter Pro
OutputDir=.
OutputBaseFilename=ACC-IFC-Converter-Pro-Setup
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64

[Files]
Source: "..\ACCIFCConverter.UI\bin\Release\net8.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion

[Icons]
Name: "{autoprograms}\ACC IFC Converter Pro"; Filename: "{app}\ACCIFCConverter.UI.exe"
Name: "{autodesktop}\ACC IFC Converter Pro"; Filename: "{app}\ACCIFCConverter.UI.exe"
