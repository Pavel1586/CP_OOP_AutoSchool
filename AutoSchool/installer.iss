#define MyAppName "AutoSchool"
#define MyAppExeName "AutoSchool.exe"

[Setup]
AppId={{B2C0A5A9-7C34-4E6F-A8A8-2AAB3E8C1234}
AppName={#MyAppName}
AppVersion=1.0.0
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=Output
OutputBaseFilename=AutoSchoolSetup
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Files]
; Копируем ВСЕ файлы из publish
Source: "D:\univer\2_course\4 семестр\ооп\кп\AutoSchool\AutoSchool\bin\Release\net8.0-windows\publish\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Создать значок на рабочем столе"; GroupDescription: "Дополнительно:"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Запустить {#MyAppName}"; Flags: nowait postinstall skipifsilent