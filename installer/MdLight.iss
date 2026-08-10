#define MyAppName "MdLight"
#define MyAppVersion "0.2.0"
#define MyAppPublisher "rausNT"
#define MyAppExeName "MdLight.exe"

[Setup]
AppId={{6ACB4591-C0C9-4784-BA74-04E37D67261A}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL=https://github.com/rausNT/md-light
AppSupportURL=https://github.com/rausNT/md-light/issues
AppUpdatesURL=https://github.com/rausNT/md-light/releases
DefaultDirName={localappdata}\Programs\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
LicenseFile=..\LICENSE
OutputDir=..\dist
OutputBaseFilename=MdLight-Setup
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
MinVersion=10.0.18362
UninstallDisplayIcon={app}\{#MyAppExeName}
ChangesAssociations=yes
CloseApplications=yes
RestartApplications=no
SetupLogging=yes

[Languages]
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "associate_md"; Description: "Ассоциировать файлы .md и .markdown с MdLight"; GroupDescription: "Интеграция с Windows:"; Flags: checkedonce
Name: "desktopicon"; Description: "Создать ярлык на рабочем столе"; GroupDescription: "Дополнительные значки:"; Flags: unchecked

[Files]
Source: "..\src\MdLight\bin\Release\net48\MdLight.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\README.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\LICENSE"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\sample.md"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\MdLight"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Пример Markdown"; Filename: "{app}\sample.md"
Name: "{autodesktop}\MdLight"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Registry]
; ProgID and capabilities make MdLight available in "Open with" and Default Apps.
Root: HKCU; Subkey: "Software\Classes\MdLight.Markdown"; ValueType: string; ValueName: ""; ValueData: "Документ Markdown"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\MdLight.Markdown"; ValueType: string; ValueName: "FriendlyTypeName"; ValueData: "Документ Markdown"
Root: HKCU; Subkey: "Software\Classes\MdLight.Markdown\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName},0"
Root: HKCU; Subkey: "Software\Classes\MdLight.Markdown\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""

Root: HKCU; Subkey: "Software\Classes\.md\OpenWithProgids"; ValueType: none; ValueName: "MdLight.Markdown"; Flags: uninsdeletevalue
Root: HKCU; Subkey: "Software\Classes\.markdown\OpenWithProgids"; ValueType: none; ValueName: "MdLight.Markdown"; Flags: uninsdeletevalue

Root: HKCU; Subkey: "Software\MdLight\Capabilities"; ValueType: string; ValueName: "ApplicationName"; ValueData: "MdLight"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\MdLight\Capabilities"; ValueType: string; ValueName: "ApplicationDescription"; ValueData: "Лёгкий бесплатный просмотрщик Markdown"
Root: HKCU; Subkey: "Software\MdLight\Capabilities\FileAssociations"; ValueType: string; ValueName: ".md"; ValueData: "MdLight.Markdown"
Root: HKCU; Subkey: "Software\MdLight\Capabilities\FileAssociations"; ValueType: string; ValueName: ".markdown"; ValueData: "MdLight.Markdown"
Root: HKCU; Subkey: "Software\RegisteredApplications"; ValueType: string; ValueName: "MdLight"; ValueData: "Software\MdLight\Capabilities"; Flags: uninsdeletevalue

; Windows honors these defaults when there is no protected UserChoice yet.
; If another default already exists, the confirmation page opens after setup.
Root: HKCU; Subkey: "Software\Classes\.md"; ValueType: string; ValueName: ""; ValueData: "MdLight.Markdown"; Tasks: associate_md
Root: HKCU; Subkey: "Software\Classes\.markdown"; ValueType: string; ValueName: ""; ValueData: "MdLight.Markdown"; Tasks: associate_md

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Запустить MdLight"; Flags: nowait postinstall skipifsilent
Filename: "ms-settings:defaultapps?registeredAppUser=MdLight"; Description: "Подтвердить MdLight для файлов Markdown в настройках Windows"; Flags: shellexec nowait postinstall skipifsilent; Tasks: associate_md
