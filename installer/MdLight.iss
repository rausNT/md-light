#define MyAppName "MdLight"
#define MyAppVersion "0.3.0"
#define MyAppPublisher "rausNT"
#define MyAppExeName "MdLight.exe"
#define PreviewHandlerCategoryGuid "8895B1C6-B41F-4C1C-A562-0D564250836F"
#define WindowsTextPreviewHandlerGuid "1531D583-8375-4D3F-B5FB-D23BBD169F22"

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
SetupIconFile=..\assets\MdLight.ico

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "korean"; MessagesFile: "compiler:Languages\Korean.isl"
Name: "chinesesimplified"; MessagesFile: "Languages\ChineseSimplified.isl"

[CustomMessages]
english.AssociateMarkdown=Associate .md and .markdown files with MdLight
english.WindowsIntegration=Windows integration:
english.DesktopIcon=Create a desktop shortcut
english.AdditionalIcons=Additional shortcuts:
english.SampleMarkdown=Markdown sample
english.MarkdownDocument=Markdown document
english.AppDescription=Lightweight free Markdown viewer and editor
english.LaunchApp=Launch MdLight
english.ConfirmDefaultApps=Confirm MdLight for Markdown files in Windows Settings
russian.AssociateMarkdown=Ассоциировать файлы .md и .markdown с MdLight
russian.WindowsIntegration=Интеграция с Windows:
russian.DesktopIcon=Создать ярлык на рабочем столе
russian.AdditionalIcons=Дополнительные значки:
russian.SampleMarkdown=Пример Markdown
russian.MarkdownDocument=Документ Markdown
russian.AppDescription=Лёгкий бесплатный просмотрщик и редактор Markdown
russian.LaunchApp=Запустить MdLight
russian.ConfirmDefaultApps=Подтвердить MdLight для файлов Markdown в настройках Windows
german.AssociateMarkdown=.md- und .markdown-Dateien mit MdLight verknüpfen
german.WindowsIntegration=Windows-Integration:
german.DesktopIcon=Desktop-Verknüpfung erstellen
german.AdditionalIcons=Zusätzliche Verknüpfungen:
german.SampleMarkdown=Markdown-Beispiel
german.MarkdownDocument=Markdown-Dokument
german.AppDescription=Leichtgewichtiger kostenloser Markdown-Viewer und -Editor
german.LaunchApp=MdLight starten
german.ConfirmDefaultApps=MdLight für Markdown-Dateien in Windows bestätigen
french.AssociateMarkdown=Associer les fichiers .md et .markdown à MdLight
french.WindowsIntegration=Intégration Windows :
french.DesktopIcon=Créer un raccourci sur le Bureau
french.AdditionalIcons=Raccourcis supplémentaires :
french.SampleMarkdown=Exemple Markdown
french.MarkdownDocument=Document Markdown
french.AppDescription=Visionneuse et éditeur Markdown légers et gratuits
french.LaunchApp=Lancer MdLight
french.ConfirmDefaultApps=Confirmer MdLight pour les fichiers Markdown dans Windows
spanish.AssociateMarkdown=Asociar archivos .md y .markdown con MdLight
spanish.WindowsIntegration=Integración con Windows:
spanish.DesktopIcon=Crear un acceso directo en el escritorio
spanish.AdditionalIcons=Accesos directos adicionales:
spanish.SampleMarkdown=Ejemplo de Markdown
spanish.MarkdownDocument=Documento Markdown
spanish.AppDescription=Visor y editor de Markdown ligero y gratuito
spanish.LaunchApp=Iniciar MdLight
spanish.ConfirmDefaultApps=Confirmar MdLight para archivos Markdown en Windows
italian.AssociateMarkdown=Associa i file .md e .markdown a MdLight
italian.WindowsIntegration=Integrazione con Windows:
italian.DesktopIcon=Crea un collegamento sul desktop
italian.AdditionalIcons=Collegamenti aggiuntivi:
italian.SampleMarkdown=Esempio Markdown
italian.MarkdownDocument=Documento Markdown
italian.AppDescription=Visualizzatore ed editor Markdown leggero e gratuito
italian.LaunchApp=Avvia MdLight
italian.ConfirmDefaultApps=Conferma MdLight per i file Markdown in Windows
brazilianportuguese.AssociateMarkdown=Associar arquivos .md e .markdown ao MdLight
brazilianportuguese.WindowsIntegration=Integração com o Windows:
brazilianportuguese.DesktopIcon=Criar um atalho na área de trabalho
brazilianportuguese.AdditionalIcons=Atalhos adicionais:
brazilianportuguese.SampleMarkdown=Exemplo de Markdown
brazilianportuguese.MarkdownDocument=Documento Markdown
brazilianportuguese.AppDescription=Visualizador e editor Markdown leve e gratuito
brazilianportuguese.LaunchApp=Iniciar o MdLight
brazilianportuguese.ConfirmDefaultApps=Confirmar o MdLight para arquivos Markdown no Windows
japanese.AssociateMarkdown=.md および .markdown ファイルを MdLight に関連付ける
japanese.WindowsIntegration=Windows との統合:
japanese.DesktopIcon=デスクトップにショートカットを作成する
japanese.AdditionalIcons=追加のショートカット:
japanese.SampleMarkdown=Markdown サンプル
japanese.MarkdownDocument=Markdown ドキュメント
japanese.AppDescription=軽量で無料の Markdown ビューアーとエディター
japanese.LaunchApp=MdLight を起動する
japanese.ConfirmDefaultApps=Windows で Markdown ファイル用の MdLight を確認する
korean.AssociateMarkdown=.md 및 .markdown 파일을 MdLight에 연결
korean.WindowsIntegration=Windows 통합:
korean.DesktopIcon=바탕 화면 바로 가기 만들기
korean.AdditionalIcons=추가 바로 가기:
korean.SampleMarkdown=Markdown 예제
korean.MarkdownDocument=Markdown 문서
korean.AppDescription=가볍고 무료인 Markdown 뷰어 및 편집기
korean.LaunchApp=MdLight 실행
korean.ConfirmDefaultApps=Windows에서 Markdown 파일용 MdLight 확인
chinesesimplified.AssociateMarkdown=将 .md 和 .markdown 文件关联到 MdLight
chinesesimplified.WindowsIntegration=Windows 集成：
chinesesimplified.DesktopIcon=创建桌面快捷方式
chinesesimplified.AdditionalIcons=其他快捷方式：
chinesesimplified.SampleMarkdown=Markdown 示例
chinesesimplified.MarkdownDocument=Markdown 文档
chinesesimplified.AppDescription=轻量免费的 Markdown 查看器和编辑器
chinesesimplified.LaunchApp=启动 MdLight
chinesesimplified.ConfirmDefaultApps=在 Windows 中确认使用 MdLight 打开 Markdown 文件

[Tasks]
Name: "associate_md"; Description: "{cm:AssociateMarkdown}"; GroupDescription: "{cm:WindowsIntegration}"; Flags: checkedonce
Name: "desktopicon"; Description: "{cm:DesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\src\MdLight\bin\Release\net48\MdLight.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\README.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\README.ru.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\LICENSE"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\sample.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\sample.ru.md"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\MdLight"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:SampleMarkdown}"; Filename: "{app}\sample.md"
Name: "{autodesktop}\MdLight"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Registry]
; ProgID and capabilities make MdLight available in "Open with" and Default Apps.
Root: HKCU; Subkey: "Software\Classes\MdLight.Markdown"; ValueType: string; ValueName: ""; ValueData: "{cm:MarkdownDocument}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\MdLight.Markdown"; ValueType: string; ValueName: "FriendlyTypeName"; ValueData: "{cm:MarkdownDocument}"
Root: HKCU; Subkey: "Software\Classes\MdLight.Markdown\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName},0"
Root: HKCU; Subkey: "Software\Classes\MdLight.Markdown\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""
; Explorer's Preview pane uses a Shell Preview Handler, independently of the
; default application. Markdown is text, so reuse Windows' built-in TXT
; previewer instead of installing an additional in-process COM component.
Root: HKCU; Subkey: "Software\Classes\MdLight.Markdown\shellex\{{{#PreviewHandlerCategoryGuid}}"; ValueType: string; ValueName: ""; ValueData: "{{{#WindowsTextPreviewHandlerGuid}}"
; Do not replace a preview handler already chosen by the user or another app.
Root: HKCU; Subkey: "Software\Classes\.md\shellex\{{{#PreviewHandlerCategoryGuid}}"; ValueType: string; ValueName: ""; ValueData: "{{{#WindowsTextPreviewHandlerGuid}}"; Flags: uninsdeletekey; Check: PreviewHandlerIsMissing('.md')
Root: HKCU; Subkey: "Software\Classes\.markdown\shellex\{{{#PreviewHandlerCategoryGuid}}"; ValueType: string; ValueName: ""; ValueData: "{{{#WindowsTextPreviewHandlerGuid}}"; Flags: uninsdeletekey; Check: PreviewHandlerIsMissing('.markdown')
Root: HKCU; Subkey: "Software\Classes\.md"; ValueType: string; ValueName: "Content Type"; ValueData: "text/markdown"; Flags: uninsdeletevalue; Check: ExtensionValueIsMissing('.md', 'Content Type')
Root: HKCU; Subkey: "Software\Classes\.md"; ValueType: string; ValueName: "PerceivedType"; ValueData: "text"; Flags: uninsdeletevalue; Check: ExtensionValueIsMissing('.md', 'PerceivedType')
Root: HKCU; Subkey: "Software\Classes\.markdown"; ValueType: string; ValueName: "Content Type"; ValueData: "text/markdown"; Flags: uninsdeletevalue; Check: ExtensionValueIsMissing('.markdown', 'Content Type')
Root: HKCU; Subkey: "Software\Classes\.markdown"; ValueType: string; ValueName: "PerceivedType"; ValueData: "text"; Flags: uninsdeletevalue; Check: ExtensionValueIsMissing('.markdown', 'PerceivedType')

Root: HKCU; Subkey: "Software\Classes\.md\OpenWithProgids"; ValueType: none; ValueName: "MdLight.Markdown"; Flags: uninsdeletevalue
Root: HKCU; Subkey: "Software\Classes\.markdown\OpenWithProgids"; ValueType: none; ValueName: "MdLight.Markdown"; Flags: uninsdeletevalue

Root: HKCU; Subkey: "Software\MdLight\Capabilities"; ValueType: string; ValueName: "ApplicationName"; ValueData: "MdLight"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\MdLight\Capabilities"; ValueType: string; ValueName: "ApplicationDescription"; ValueData: "{cm:AppDescription}"
Root: HKCU; Subkey: "Software\MdLight\Capabilities\FileAssociations"; ValueType: string; ValueName: ".md"; ValueData: "MdLight.Markdown"
Root: HKCU; Subkey: "Software\MdLight\Capabilities\FileAssociations"; ValueType: string; ValueName: ".markdown"; ValueData: "MdLight.Markdown"
Root: HKCU; Subkey: "Software\RegisteredApplications"; ValueType: string; ValueName: "MdLight"; ValueData: "Software\MdLight\Capabilities"; Flags: uninsdeletevalue

; Windows honors these defaults when there is no protected UserChoice yet.
; If another default already exists, the confirmation page opens after setup.
Root: HKCU; Subkey: "Software\Classes\.md"; ValueType: string; ValueName: ""; ValueData: "MdLight.Markdown"; Tasks: associate_md
Root: HKCU; Subkey: "Software\Classes\.markdown"; ValueType: string; ValueName: ""; ValueData: "MdLight.Markdown"; Tasks: associate_md

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchApp}"; Flags: nowait postinstall skipifsilent
Filename: "ms-settings:defaultapps?registeredAppUser=MdLight"; Description: "{cm:ConfirmDefaultApps}"; Flags: shellexec nowait postinstall skipifsilent; Tasks: associate_md

[Code]
function ExtensionValueIsMissing(const Extension, ValueName: String): Boolean;
begin
  Result := not RegValueExists(HKCR, Extension, ValueName);
end;

function PreviewHandlerIsMissing(const Extension: String): Boolean;
begin
  Result := ExtensionValueIsMissing(
    Extension + '\shellex\{' + '{#PreviewHandlerCategoryGuid}' + '}', '');
end;
