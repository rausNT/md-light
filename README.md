# MdLight

[English](README.md) | [Русский](README.ru.md)

<img src="assets/MdLight-icon.png" alt="MdLight icon" width="96">

[![Build Windows app](https://github.com/rausNT/md-light/actions/workflows/build.yml/badge.svg)](https://github.com/rausNT/md-light/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

**A lightweight, free Markdown (`.md`) viewer and editor for Windows 10 and 11.**

MdLight opens Markdown files without a browser, Electron, WebView, paid
components, or an additional runtime download. It is a small WPF application
built for the .NET Framework 4.8 already included with supported Windows
versions.

## Features

- Open files with a button, drag and drop, or a command-line path.
- Edit visually with Bold, Italic, headings, paragraph alignment, lists, and
  editable tables; switch to Preview before saving if desired.
- Render headings, lists and tasks, quotes, tables, links, emphasis, and code.
- Refresh the open document automatically after it is saved.
- Preview raw Markdown in the Windows File Explorer Preview pane.
- Use a light or dark theme.
- Choose from English, Russian, German, French, Spanish, Italian, Brazilian
  Portuguese, Simplified Chinese, Japanese, and Korean.
- Use `Ctrl+N` to create, `Ctrl+O` to open, `Ctrl+S` to save, switch between
  editing and preview with `Ctrl+Shift+E`, and use `Ctrl+E` to
  center a paragraph while editing.
- Make no network requests and require no runtime beyond Windows components.

English is used on the first launch. A language selected in the toolbar is
saved for subsequent launches, with English serving as the fallback.

Visual editing uses the native WPF rich-text engine—there is still no browser,
Electron, WebView, or background service involved.

## Download

Ready-to-use `MdLight-Setup.exe` and `MdLight-portable.zip` packages are
published under [Releases](https://github.com/rausNT/md-light/releases).
Validation builds for every change are available in
[GitHub Actions](https://github.com/rausNT/md-light/actions).

Run `MdLight-Setup.exe`. On the Additional Tasks page, leave
**Associate .md and .markdown files with MdLight** enabled if you want MdLight
registered as a Markdown handler. If Windows already protects a different
default choice, Setup opens Default Apps so you can confirm MdLight.

`MdLight-portable.zip` requires no installation. Extract it and run
`MdLight.exe`; use **Open with → Choose another app** to associate the portable
version manually.

Setup also connects `.md` and `.markdown` to the built-in Windows text Preview
Handler. When the File Explorer Preview pane is enabled (`Alt+P`), it displays
the raw Markdown source. You may need to reopen the File Explorer window after
the first installation.

## Build

The .NET SDK is required only for development:

```powershell
dotnet restore MdLight.sln
dotnet build MdLight.sln -c Release --no-restore
```

The application is written to `src/MdLight/bin/Release/net48/`. Build the
installer with the free [Inno Setup](https://jrsoftware.org/isinfo.php):

```powershell
$iscc = "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe"
& $iscc installer\MdLight.iss
```

## Supported Markdown

Headings, paragraphs, **bold**, *italic*, ~~strikethrough~~, `inline code`,
links, ordered and unordered lists, task lists, quotes, tables, horizontal
rules, and fenced code blocks are supported. HTML is intentionally displayed
as plain text to make local documents safer to open.

The visual editor saves standard Markdown for emphasis, headings, lists, and
GFM tables. Because Markdown has no standard paragraph-alignment syntax,
centered and right-aligned paragraphs are saved as restricted `<p align>` HTML
and are rendered visually by MdLight. Other embedded HTML remains plain text.

## Security and code signing

- [Code signing policy](SIGNING_POLICY.md)
- [Privacy policy](PRIVACY.md)
- [Report a vulnerability](SECURITY.md)
- [Third-party notices](THIRD-PARTY-NOTICES.md)

Free code signing provided by [SignPath.io](https://signpath.io/), certificate
by [SignPath Foundation](https://signpath.org/). Signed releases will be
created only from this repository's source code on GitHub-hosted runners.

## License

[MIT](LICENSE) — free to use, modify, and distribute.
