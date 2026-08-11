# MdLight

[![Build Windows app](https://github.com/rausNT/md-light/actions/workflows/build.yml/badge.svg)](https://github.com/rausNT/md-light/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

**Лёгкий бесплатный просмотрщик Markdown (`.md`) для Windows 10 и 11.**

MdLight открывает Markdown-файлы без браузера, Electron, WebView и платных
компонентов. Это небольшое WPF-приложение, которому достаточно уже встроенного
в Windows .NET Framework 4.8.

## Возможности

- открытие файла кнопкой, перетаскиванием или передачей пути в командной строке;
- заголовки, списки и задачи, цитаты, таблицы, ссылки, выделение и блоки кода;
- автоматическое обновление открытого документа после сохранения;
- светлая и тёмная темы;
- горячие клавиши: `Ctrl+O` — открыть, `Ctrl+R` или `F5` — обновить;
- никаких сетевых запросов и runtime-зависимостей сверх компонентов Windows.

## Скачать

Готовые `MdLight-Setup.exe` и `MdLight-portable.zip` публикуются в разделе
[Releases](https://github.com/rausNT/md-light/releases). Проверочные сборки для
каждого изменения доступны в [GitHub Actions](https://github.com/rausNT/md-light/actions).

Запустите `MdLight-Setup.exe`. На странице дополнительных задач можно оставить
включённым пункт **«Ассоциировать файлы .md и .markdown с MdLight»**. Программа
будет зарегистрирована в Windows как обработчик Markdown; если для расширения
уже защищён системный выбор, установщик откроет страницу «Приложения по
умолчанию», где достаточно подтвердить MdLight.

`MdLight-portable.zip` — версия без установки. После распаковки запустите
`MdLight.exe`; ассоциацию для portable-версии можно задать через
**Открыть с помощью → Выбрать другое приложение**.

## Сборка

Требуется .NET SDK (только для разработки):

```powershell
dotnet restore MdLight.sln
dotnet build MdLight.sln -c Release --no-restore
```

Готовое приложение находится в `src/MdLight/bin/Release/net48/`.
Установщик собирается бесплатным [Inno Setup](https://jrsoftware.org/isinfo.php):

```powershell
$iscc = "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe"
& $iscc installer\MdLight.iss
```

## Поддерживаемый Markdown

Заголовки, абзацы, **жирный**, *курсив*, ~~зачёркнутый~~, `inline code`,
ссылки, маркированные и нумерованные списки, списки задач, цитаты, таблицы,
горизонтальные линии и fenced code blocks. HTML намеренно показывается как
обычный текст — так локальные документы безопаснее открывать.

## Безопасность и подпись кода

- [Политика подписи кода](SIGNING_POLICY.md)
- [Политика конфиденциальности](PRIVACY.md)
- [Сообщить об уязвимости](SECURITY.md)
- [Сторонние компоненты](THIRD-PARTY-NOTICES.md)

Free code signing provided by [SignPath.io](https://signpath.io/), certificate
by [SignPath Foundation](https://signpath.org/). Подписанные релизы будут
создаваться только из исходного кода этого репозитория на GitHub-hosted runners.

## Лицензия

[MIT](LICENSE) — можно свободно использовать, изменять и распространять.
