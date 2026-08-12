using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace MdLight
{
    internal static class Localization
    {
        private const string SettingsKey = @"Software\MdLight";
        private const string LanguageValue = "Language";

        private static readonly Dictionary<string, Dictionary<string, string>> Translations =
            new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["en"] = Strings(
                    "Drop a Markdown file here", "Dark theme", "Light theme", "Open…", "Ready",
                    "Ctrl+O — open  ·  F5 — refresh", "Simple Markdown viewing",
                    "Drop an .md file into this window or select “Open…”.", "File not found.",
                    "Could not open file", "Error", "This link type is not supported.",
                    "Could not open link", "Open Markdown", "Markdown files", "Text files",
                    "All files", "Empty document", "Image", "Language"),
                ["ru"] = Strings(
                    "Перетащите сюда Markdown-файл", "Тёмная тема", "Светлая тема", "Открыть…", "Готово",
                    "Ctrl+O — открыть  ·  F5 — обновить", "Простой просмотр Markdown",
                    "Перетащите .md-файл в это окно или нажмите «Открыть…».", "Файл не найден.",
                    "Не удалось открыть файл", "Ошибка", "Этот тип ссылки не поддерживается.",
                    "Не удалось открыть ссылку", "Открыть Markdown", "Файлы Markdown", "Текстовые файлы",
                    "Все файлы", "Пустой документ", "Изображение", "Язык"),
                ["de"] = Strings(
                    "Markdown-Datei hier ablegen", "Dunkles Design", "Helles Design", "Öffnen…", "Bereit",
                    "Strg+O — öffnen  ·  F5 — aktualisieren", "Markdown einfach anzeigen",
                    "Eine .md-Datei hier ablegen oder „Öffnen…“ wählen.", "Datei nicht gefunden.",
                    "Datei konnte nicht geöffnet werden", "Fehler", "Dieser Linktyp wird nicht unterstützt.",
                    "Link konnte nicht geöffnet werden", "Markdown öffnen", "Markdown-Dateien", "Textdateien",
                    "Alle Dateien", "Leeres Dokument", "Bild", "Sprache"),
                ["fr"] = Strings(
                    "Déposez un fichier Markdown ici", "Thème sombre", "Thème clair", "Ouvrir…", "Prêt",
                    "Ctrl+O — ouvrir  ·  F5 — actualiser", "Lecture simple de Markdown",
                    "Déposez un fichier .md ici ou sélectionnez « Ouvrir… ».", "Fichier introuvable.",
                    "Impossible d’ouvrir le fichier", "Erreur", "Ce type de lien n’est pas pris en charge.",
                    "Impossible d’ouvrir le lien", "Ouvrir un fichier Markdown", "Fichiers Markdown", "Fichiers texte",
                    "Tous les fichiers", "Document vide", "Image", "Langue"),
                ["es"] = Strings(
                    "Suelta aquí un archivo Markdown", "Tema oscuro", "Tema claro", "Abrir…", "Listo",
                    "Ctrl+O — abrir  ·  F5 — actualizar", "Visor sencillo de Markdown",
                    "Suelta un archivo .md aquí o selecciona «Abrir…».", "No se encontró el archivo.",
                    "No se pudo abrir el archivo", "Error", "Este tipo de enlace no es compatible.",
                    "No se pudo abrir el enlace", "Abrir Markdown", "Archivos Markdown", "Archivos de texto",
                    "Todos los archivos", "Documento vacío", "Imagen", "Idioma"),
                ["it"] = Strings(
                    "Trascina qui un file Markdown", "Tema scuro", "Tema chiaro", "Apri…", "Pronto",
                    "Ctrl+O — apri  ·  F5 — aggiorna", "Visualizzazione semplice di Markdown",
                    "Trascina qui un file .md o seleziona «Apri…».", "File non trovato.",
                    "Impossibile aprire il file", "Errore", "Questo tipo di collegamento non è supportato.",
                    "Impossibile aprire il collegamento", "Apri Markdown", "File Markdown", "File di testo",
                    "Tutti i file", "Documento vuoto", "Immagine", "Lingua"),
                ["pt-BR"] = Strings(
                    "Solte um arquivo Markdown aqui", "Tema escuro", "Tema claro", "Abrir…", "Pronto",
                    "Ctrl+O — abrir  ·  F5 — atualizar", "Visualização simples de Markdown",
                    "Solte um arquivo .md aqui ou selecione “Abrir…”.", "Arquivo não encontrado.",
                    "Não foi possível abrir o arquivo", "Erro", "Este tipo de link não é compatível.",
                    "Não foi possível abrir o link", "Abrir Markdown", "Arquivos Markdown", "Arquivos de texto",
                    "Todos os arquivos", "Documento vazio", "Imagem", "Idioma"),
                ["zh-CN"] = Strings(
                    "将 Markdown 文件拖放到此处", "深色主题", "浅色主题", "打开…", "就绪",
                    "Ctrl+O — 打开  ·  F5 — 刷新", "轻松查看 Markdown",
                    "将 .md 文件拖放到此窗口，或选择“打开…”。", "找不到文件。",
                    "无法打开文件", "错误", "不支持此链接类型。",
                    "无法打开链接", "打开 Markdown", "Markdown 文件", "文本文件",
                    "所有文件", "空文档", "图像", "语言"),
                ["ja"] = Strings(
                    "Markdown ファイルをここにドロップ", "ダーク テーマ", "ライト テーマ", "開く…", "準備完了",
                    "Ctrl+O — 開く  ·  F5 — 更新", "シンプルな Markdown ビューアー",
                    ".md ファイルをここにドロップするか、［開く…］を選択してください。", "ファイルが見つかりません。",
                    "ファイルを開けませんでした", "エラー", "この種類のリンクはサポートされていません。",
                    "リンクを開けませんでした", "Markdown を開く", "Markdown ファイル", "テキスト ファイル",
                    "すべてのファイル", "空のドキュメント", "画像", "言語"),
                ["ko"] = Strings(
                    "Markdown 파일을 여기에 놓으세요", "어두운 테마", "밝은 테마", "열기…", "준비됨",
                    "Ctrl+O — 열기  ·  F5 — 새로 고침", "간편한 Markdown 보기",
                    ".md 파일을 여기에 놓거나 ‘열기…’를 선택하세요.", "파일을 찾을 수 없습니다.",
                    "파일을 열 수 없습니다", "오류", "이 링크 형식은 지원되지 않습니다.",
                    "링크를 열 수 없습니다", "Markdown 열기", "Markdown 파일", "텍스트 파일",
                    "모든 파일", "빈 문서", "이미지", "언어")
            };

        private static readonly LanguageOption[] LanguageOptions =
        {
            new LanguageOption("en", "English"),
            new LanguageOption("ru", "Русский"),
            new LanguageOption("de", "Deutsch"),
            new LanguageOption("fr", "Français"),
            new LanguageOption("es", "Español"),
            new LanguageOption("it", "Italiano"),
            new LanguageOption("pt-BR", "Português (Brasil)"),
            new LanguageOption("zh-CN", "简体中文"),
            new LanguageOption("ja", "日本語"),
            new LanguageOption("ko", "한국어")
        };

        private static readonly Dictionary<string, Dictionary<string, string>> EditorTranslations =
            new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["en"] = EditorStrings("New", "Save", "Save as…", "Edit", "Preview",
                    "Save changes to this document?", "Unsaved changes", "Could not save file",
                    "Save Markdown", "Untitled", "Saved", "Modified", "The file changed outside MdLight.",
                    "Ctrl+N — new  ·  Ctrl+S — save  ·  Ctrl+Shift+E — edit/preview"),
                ["ru"] = EditorStrings("Создать", "Сохранить", "Сохранить как…", "Правка", "Просмотр",
                    "Сохранить изменения в документе?", "Несохранённые изменения", "Не удалось сохранить файл",
                    "Сохранить Markdown", "Без имени", "Сохранено", "Изменено", "Файл изменён вне MdLight.",
                    "Ctrl+N — создать  ·  Ctrl+S — сохранить  ·  Ctrl+Shift+E — правка/просмотр"),
                ["de"] = EditorStrings("Neu", "Speichern", "Speichern unter…", "Bearbeiten", "Vorschau",
                    "Änderungen an diesem Dokument speichern?", "Nicht gespeicherte Änderungen", "Datei konnte nicht gespeichert werden",
                    "Markdown speichern", "Unbenannt", "Gespeichert", "Geändert", "Die Datei wurde außerhalb von MdLight geändert.",
                    "Strg+N — neu  ·  Strg+S — speichern  ·  Strg+Umschalt+E — Bearbeiten/Vorschau"),
                ["fr"] = EditorStrings("Nouveau", "Enregistrer", "Enregistrer sous…", "Modifier", "Aperçu",
                    "Enregistrer les modifications de ce document ?", "Modifications non enregistrées", "Impossible d’enregistrer le fichier",
                    "Enregistrer le Markdown", "Sans titre", "Enregistré", "Modifié", "Le fichier a été modifié en dehors de MdLight.",
                    "Ctrl+N — nouveau  ·  Ctrl+S — enregistrer  ·  Ctrl+Maj+E — modifier/aperçu"),
                ["es"] = EditorStrings("Nuevo", "Guardar", "Guardar como…", "Editar", "Vista previa",
                    "¿Guardar los cambios de este documento?", "Cambios sin guardar", "No se pudo guardar el archivo",
                    "Guardar Markdown", "Sin título", "Guardado", "Modificado", "El archivo cambió fuera de MdLight.",
                    "Ctrl+N — nuevo  ·  Ctrl+S — guardar  ·  Ctrl+Mayús+E — editar/vista previa"),
                ["it"] = EditorStrings("Nuovo", "Salva", "Salva con nome…", "Modifica", "Anteprima",
                    "Salvare le modifiche al documento?", "Modifiche non salvate", "Impossibile salvare il file",
                    "Salva Markdown", "Senza titolo", "Salvato", "Modificato", "Il file è stato modificato fuori da MdLight.",
                    "Ctrl+N — nuovo  ·  Ctrl+S — salva  ·  Ctrl+Maiusc+E — modifica/anteprima"),
                ["pt-BR"] = EditorStrings("Novo", "Salvar", "Salvar como…", "Editar", "Visualizar",
                    "Salvar as alterações deste documento?", "Alterações não salvas", "Não foi possível salvar o arquivo",
                    "Salvar Markdown", "Sem título", "Salvo", "Modificado", "O arquivo foi alterado fora do MdLight.",
                    "Ctrl+N — novo  ·  Ctrl+S — salvar  ·  Ctrl+Shift+E — editar/visualizar"),
                ["zh-CN"] = EditorStrings("新建", "保存", "另存为…", "编辑", "预览",
                    "是否保存对此文档的更改？", "未保存的更改", "无法保存文件",
                    "保存 Markdown", "无标题", "已保存", "已修改", "文件已在 MdLight 外部更改。",
                    "Ctrl+N — 新建  ·  Ctrl+S — 保存  ·  Ctrl+Shift+E — 编辑/预览"),
                ["ja"] = EditorStrings("新規", "保存", "名前を付けて保存…", "編集", "プレビュー",
                    "このドキュメントへの変更を保存しますか？", "未保存の変更", "ファイルを保存できませんでした",
                    "Markdown を保存", "無題", "保存済み", "変更あり", "ファイルが MdLight の外部で変更されました。",
                    "Ctrl+N — 新規  ·  Ctrl+S — 保存  ·  Ctrl+Shift+E — 編集/プレビュー"),
                ["ko"] = EditorStrings("새로 만들기", "저장", "다른 이름으로 저장…", "편집", "미리 보기",
                    "이 문서의 변경 내용을 저장하시겠습니까?", "저장되지 않은 변경 내용", "파일을 저장할 수 없습니다",
                    "Markdown 저장", "제목 없음", "저장됨", "수정됨", "파일이 MdLight 외부에서 변경되었습니다.",
                    "Ctrl+N — 새로 만들기  ·  Ctrl+S — 저장  ·  Ctrl+Shift+E — 편집/미리 보기")
            };

        private static readonly Dictionary<string, string[]> VisualEditorTranslations =
            new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["en"] = new[] { "Bold", "Italic", "Heading 1", "Heading 2", "Normal text", "Align left", "Center", "Align right", "Bulleted list", "Numbered list", "Insert 3×3 table", "Header", "Cell" },
                ["ru"] = new[] { "Жирный", "Курсив", "Заголовок 1", "Заголовок 2", "Обычный текст", "По левому краю", "По центру", "По правому краю", "Маркированный список", "Нумерованный список", "Вставить таблицу 3×3", "Заголовок", "Ячейка" },
                ["de"] = new[] { "Fett", "Kursiv", "Überschrift 1", "Überschrift 2", "Normaler Text", "Linksbündig", "Zentriert", "Rechtsbündig", "Aufzählung", "Nummerierte Liste", "3×3-Tabelle einfügen", "Kopfzeile", "Zelle" },
                ["fr"] = new[] { "Gras", "Italique", "Titre 1", "Titre 2", "Texte normal", "Aligner à gauche", "Centrer", "Aligner à droite", "Liste à puces", "Liste numérotée", "Insérer un tableau 3×3", "En-tête", "Cellule" },
                ["es"] = new[] { "Negrita", "Cursiva", "Título 1", "Título 2", "Texto normal", "Alinear a la izquierda", "Centrar", "Alinear a la derecha", "Lista con viñetas", "Lista numerada", "Insertar tabla 3×3", "Encabezado", "Celda" },
                ["it"] = new[] { "Grassetto", "Corsivo", "Titolo 1", "Titolo 2", "Testo normale", "Allinea a sinistra", "Centra", "Allinea a destra", "Elenco puntato", "Elenco numerato", "Inserisci tabella 3×3", "Intestazione", "Cella" },
                ["pt-BR"] = new[] { "Negrito", "Itálico", "Título 1", "Título 2", "Texto normal", "Alinhar à esquerda", "Centralizar", "Alinhar à direita", "Lista com marcadores", "Lista numerada", "Inserir tabela 3×3", "Cabeçalho", "Célula" },
                ["zh-CN"] = new[] { "粗体", "斜体", "标题 1", "标题 2", "普通文本", "左对齐", "居中", "右对齐", "项目符号列表", "编号列表", "插入 3×3 表格", "标题", "单元格" },
                ["ja"] = new[] { "太字", "斜体", "見出し 1", "見出し 2", "標準テキスト", "左揃え", "中央揃え", "右揃え", "箇条書き", "番号付きリスト", "3×3 表を挿入", "見出し", "セル" },
                ["ko"] = new[] { "굵게", "기울임꼴", "제목 1", "제목 2", "일반 텍스트", "왼쪽 맞춤", "가운데 맞춤", "오른쪽 맞춤", "글머리 기호 목록", "번호 매기기 목록", "3×3 표 삽입", "머리글", "셀" }
            };

        private static readonly string[] VisualEditorKeys =
        {
            "Bold", "Italic", "Heading1", "Heading2", "NormalText", "AlignLeft", "AlignCenter",
            "AlignRight", "BulletedList", "NumberedList", "InsertTable", "TableHeader", "TableCell"
        };

        private static string currentLanguage = "en";

        public static IEnumerable<LanguageOption> Languages => LanguageOptions;

        public static string CurrentLanguage => currentLanguage;

        public static string Get(string key)
        {
            string value;
            var visualIndex = Array.IndexOf(VisualEditorKeys, key);
            if (visualIndex >= 0)
                return VisualEditorTranslations[currentLanguage][visualIndex];
            if (EditorTranslations[currentLanguage].TryGetValue(key, out value))
                return value;
            if (Translations[currentLanguage].TryGetValue(key, out value))
                return value;
            if (EditorTranslations["en"].TryGetValue(key, out value))
                return value;
            return Translations["en"][key];
        }

        public static void LoadSavedLanguage()
        {
            var saved = Registry.GetValue(@"HKEY_CURRENT_USER\" + SettingsKey, LanguageValue, null) as string;
            SetLanguage(string.IsNullOrWhiteSpace(saved) ? "en" : saved, false);
        }

        public static void SetLanguage(string language, bool save)
        {
            currentLanguage = Translations.ContainsKey(language) ? language : "en";
            var culture = CultureInfo.GetCultureInfo(currentLanguage);
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            if (save)
                Registry.SetValue(@"HKEY_CURRENT_USER\" + SettingsKey, LanguageValue, currentLanguage, RegistryValueKind.String);
        }

        public static void Validate()
        {
            if (Translations.Count != 10 || EditorTranslations.Count != 10 || VisualEditorTranslations.Count != 10 || !Translations.ContainsKey("en"))
                throw new InvalidOperationException("Unexpected set of supported languages.");
            var expected = Translations["en"].Keys.OrderBy(key => key).ToArray();
            var expectedEditor = EditorTranslations["en"].Keys.OrderBy(key => key).ToArray();
            foreach (var language in Translations)
            {
                var actual = language.Value.Keys.OrderBy(key => key).ToArray();
                if (!expected.SequenceEqual(actual))
                    throw new InvalidOperationException("Incomplete localization: " + language.Key);
                var actualEditor = EditorTranslations[language.Key].Keys.OrderBy(key => key).ToArray();
                if (!expectedEditor.SequenceEqual(actualEditor))
                    throw new InvalidOperationException("Incomplete editor localization: " + language.Key);
                if (VisualEditorTranslations[language.Key].Length != VisualEditorKeys.Length)
                    throw new InvalidOperationException("Incomplete visual editor localization: " + language.Key);
            }
        }

        private static Dictionary<string, string> EditorStrings(params string[] values)
        {
            var keys = new[]
            {
                "New", "Save", "SaveAs", "Edit", "Preview", "UnsavedPrompt", "UnsavedTitle",
                "SaveError", "SaveDialog", "Untitled", "Saved", "Modified", "ExternalChange",
                "EditorShortcuts"
            };
            return keys.Select((key, index) => new { key, value = values[index] })
                .ToDictionary(item => item.key, item => item.value);
        }

        private static Dictionary<string, string> Strings(params string[] values)
        {
            var keys = new[]
            {
                "DropHint", "DarkTheme", "LightTheme", "Open", "Ready", "Shortcuts",
                "EmptyTitle", "EmptySubtitle", "FileNotFound", "OpenFileError", "Error",
                "UnsupportedLink", "OpenLinkError", "OpenDialog", "MarkdownFiles", "TextFiles",
                "AllFiles", "EmptyDocument", "Image", "Language"
            };
            return keys.Select((key, index) => new { key, value = values[index] })
                .ToDictionary(item => item.key, item => item.value);
        }
    }

    internal sealed class LanguageOption
    {
        public LanguageOption(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public string Code { get; }
        public string Name { get; }
        public override string ToString() => Name;
    }
}
