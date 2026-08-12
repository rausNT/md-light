using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace MdLight
{
    internal static class MarkdownRenderer
    {
        private static readonly Regex Heading = new Regex(@"^\s{0,3}(#{1,6})\s+(.+?)\s*#*\s*$", RegexOptions.Compiled);
        private static readonly Regex UnorderedItem = new Regex(@"^\s{0,3}[-+*]\s+(.+)$", RegexOptions.Compiled);
        private static readonly Regex OrderedItem = new Regex(@"^\s{0,3}\d+[.)]\s+(.+)$", RegexOptions.Compiled);
        private static readonly Regex Divider = new Regex(@"^\s{0,3}((\*\s*){3,}|(-\s*){3,}|(_\s*){3,})$", RegexOptions.Compiled);
        private static readonly Regex TableDivider = new Regex(@"^\s*\|?\s*:?-{3,}:?\s*(\|\s*:?-{3,}:?\s*)+\|?\s*$", RegexOptions.Compiled);

        public static FlowDocument Render(string markdown, Action<string> openLink, bool dark)
        {
            var foreground = Color(dark ? "#FFE5E7EB" : "#FF1F2937");
            var muted = Color(dark ? "#FF9CA3AF" : "#FF6B7280");
            var accent = Color(dark ? "#FF60A5FA" : "#FF2563EB");
            var codeBackground = Color(dark ? "#FF1F2937" : "#FFF0F2F5");
            var quoteBackground = Color(dark ? "#FF172033" : "#FFF3F6FA");

            var document = new FlowDocument
            {
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 15,
                Foreground = foreground,
                PagePadding = new Thickness(0),
                LineHeight = 23,
                TextAlignment = TextAlignment.Left
            };

            var lines = (markdown ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            var index = 0;
            while (index < lines.Length)
            {
                var line = lines[index];
                if (string.IsNullOrWhiteSpace(line))
                {
                    index++;
                    continue;
                }

                if (line.TrimStart().StartsWith("```", StringComparison.Ordinal) ||
                    line.TrimStart().StartsWith("~~~", StringComparison.Ordinal))
                {
                    var marker = line.TrimStart().Substring(0, 3);
                    var language = line.TrimStart().Substring(3).Trim();
                    var code = new StringBuilder();
                    index++;
                    while (index < lines.Length && !lines[index].TrimStart().StartsWith(marker, StringComparison.Ordinal))
                    {
                        if (code.Length > 0) code.AppendLine();
                        code.Append(lines[index++]);
                    }
                    if (index < lines.Length) index++;

                    var codeBlock = new Paragraph(new Run(code.ToString()))
                    {
                        FontFamily = new FontFamily("Consolas"),
                        FontSize = 13,
                        LineHeight = 20,
                        Background = codeBackground,
                        Padding = new Thickness(16, 13, 16, 13),
                        Margin = new Thickness(0, 7, 0, 16)
                    };
                    if (!string.IsNullOrEmpty(language))
                        codeBlock.ToolTip = language;
                    document.Blocks.Add(codeBlock);
                    continue;
                }

                var heading = Heading.Match(line);
                if (heading.Success)
                {
                    var level = heading.Groups[1].Value.Length;
                    var sizes = new[] { 30d, 25d, 21d, 18d, 16d, 15d };
                    var paragraph = new Paragraph
                    {
                        FontSize = sizes[level - 1],
                        FontWeight = level <= 3 ? FontWeights.SemiBold : FontWeights.Bold,
                        Foreground = level == 1 ? accent : foreground,
                        Margin = new Thickness(0, level == 1 ? 6 : 12, 0, 8),
                        LineHeight = sizes[level - 1] * 1.25
                    };
                    AddInlines(paragraph.Inlines, heading.Groups[2].Value, openLink, foreground, accent, codeBackground);
                    document.Blocks.Add(paragraph);
                    index++;
                    continue;
                }

                if (Divider.IsMatch(line))
                {
                    document.Blocks.Add(new Paragraph(new Run("────────────────────────────────────────"))
                    {
                        Foreground = muted,
                        FontSize = 11,
                        Margin = new Thickness(0, 8, 0, 12)
                    });
                    index++;
                    continue;
                }

                if (line.TrimStart().StartsWith(">", StringComparison.Ordinal))
                {
                    var quoteText = new StringBuilder();
                    while (index < lines.Length && lines[index].TrimStart().StartsWith(">", StringComparison.Ordinal))
                    {
                        var content = lines[index].TrimStart().Substring(1).TrimStart();
                        if (quoteText.Length > 0) quoteText.AppendLine();
                        quoteText.Append(content);
                        index++;
                    }
                    var quote = new Paragraph
                    {
                        Background = quoteBackground,
                        BorderBrush = accent,
                        BorderThickness = new Thickness(4, 0, 0, 0),
                        Padding = new Thickness(14, 10, 14, 10),
                        Margin = new Thickness(0, 6, 0, 14),
                        Foreground = muted
                    };
                    AddMultilineInlines(quote.Inlines, quoteText.ToString(), openLink, muted, accent, codeBackground);
                    document.Blocks.Add(quote);
                    continue;
                }

                Match listMatch;
                bool ordered;
                if ((listMatch = UnorderedItem.Match(line)).Success || (listMatch = OrderedItem.Match(line)).Success)
                {
                    ordered = OrderedItem.IsMatch(line);
                    var list = new System.Windows.Documents.List
                    {
                        MarkerStyle = ordered ? TextMarkerStyle.Decimal : TextMarkerStyle.Disc,
                        Margin = new Thickness(18, 3, 0, 13),
                        Padding = new Thickness(4, 0, 0, 0)
                    };
                    while (index < lines.Length)
                    {
                        listMatch = ordered ? OrderedItem.Match(lines[index]) : UnorderedItem.Match(lines[index]);
                        if (!listMatch.Success) break;
                        var itemText = listMatch.Groups[1].Value;
                        var task = Regex.Match(itemText, @"^\[([ xX])\]\s+(.*)$");
                        if (task.Success)
                            itemText = (task.Groups[1].Value == " " ? "☐ " : "☑ ") + task.Groups[2].Value;
                        var itemParagraph = new Paragraph { Margin = new Thickness(0, 1, 0, 3) };
                        AddInlines(itemParagraph.Inlines, itemText, openLink, foreground, accent, codeBackground);
                        list.ListItems.Add(new ListItem(itemParagraph));
                        index++;
                    }
                    document.Blocks.Add(list);
                    continue;
                }

                if (index + 1 < lines.Length && lines[index].Contains("|") && TableDivider.IsMatch(lines[index + 1]))
                {
                    var rows = new List<string[]> { SplitTableRow(lines[index]) };
                    index += 2;
                    while (index < lines.Length && lines[index].Contains("|") && !string.IsNullOrWhiteSpace(lines[index]))
                        rows.Add(SplitTableRow(lines[index++]));
                    document.Blocks.Add(CreateTable(rows, openLink, foreground, accent, codeBackground, quoteBackground, muted));
                    continue;
                }

                var paragraphText = new StringBuilder(line.Trim());
                index++;
                while (index < lines.Length && !string.IsNullOrWhiteSpace(lines[index]) && !IsBlockStart(lines, index))
                {
                    paragraphText.AppendLine();
                    paragraphText.Append(lines[index].Trim());
                    index++;
                }
                var body = new Paragraph { Margin = new Thickness(0, 2, 0, 13) };
                AddMultilineInlines(body.Inlines, paragraphText.ToString(), openLink, foreground, accent, codeBackground);
                document.Blocks.Add(body);
            }

            if (document.Blocks.Count == 0)
                document.Blocks.Add(new Paragraph(new Run(Localization.Get("EmptyDocument"))) { Foreground = muted });
            return document;
        }

        private static bool IsBlockStart(string[] lines, int index)
        {
            var line = lines[index];
            return Heading.IsMatch(line) || Divider.IsMatch(line) || UnorderedItem.IsMatch(line) ||
                   OrderedItem.IsMatch(line) || line.TrimStart().StartsWith(">", StringComparison.Ordinal) ||
                   line.TrimStart().StartsWith("```", StringComparison.Ordinal) ||
                   line.TrimStart().StartsWith("~~~", StringComparison.Ordinal) ||
                   (index + 1 < lines.Length && line.Contains("|") && TableDivider.IsMatch(lines[index + 1]));
        }

        private static System.Windows.Documents.Table CreateTable(List<string[]> rows, Action<string> openLink,
            Brush foreground, Brush accent, Brush codeBackground, Brush headerBackground, Brush border)
        {
            var table = new System.Windows.Documents.Table
            {
                CellSpacing = 0,
                Margin = new Thickness(0, 7, 0, 16),
                BorderBrush = border,
                BorderThickness = new Thickness(1)
            };
            var columns = 0;
            foreach (var row in rows) columns = Math.Max(columns, row.Length);
            for (var column = 0; column < columns; column++)
                table.Columns.Add(new TableColumn());

            var group = new TableRowGroup();
            table.RowGroups.Add(group);
            for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                var row = new TableRow { Background = rowIndex == 0 ? headerBackground : Brushes.Transparent };
                group.Rows.Add(row);
                for (var column = 0; column < columns; column++)
                {
                    var paragraph = new Paragraph { Margin = new Thickness(0) };
                    AddInlines(paragraph.Inlines, column < rows[rowIndex].Length ? rows[rowIndex][column] : string.Empty,
                        openLink, foreground, accent, codeBackground);
                    if (rowIndex == 0) paragraph.FontWeight = FontWeights.SemiBold;
                    row.Cells.Add(new TableCell(paragraph)
                    {
                        Padding = new Thickness(9, 6, 9, 6),
                        BorderBrush = border,
                        BorderThickness = new Thickness(0, 0, 1, 1)
                    });
                }
            }
            return table;
        }

        private static string[] SplitTableRow(string line)
        {
            line = line.Trim();
            if (line.StartsWith("|", StringComparison.Ordinal)) line = line.Substring(1);
            if (line.EndsWith("|", StringComparison.Ordinal)) line = line.Substring(0, line.Length - 1);
            return Regex.Split(line, @"(?<!\\)\|").Select(cell => cell.Trim().Replace("\\|", "|")).ToArray();
        }

        private static void AddMultilineInlines(InlineCollection target, string text, Action<string> openLink,
            Brush foreground, Brush accent, Brush codeBackground)
        {
            var lines = text.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                if (i > 0) target.Add(new LineBreak());
                AddInlines(target, lines[i], openLink, foreground, accent, codeBackground);
            }
        }

        private static void AddInlines(InlineCollection target, string text, Action<string> openLink,
            Brush foreground, Brush accent, Brush codeBackground)
        {
            var position = 0;
            while (position < text.Length)
            {
                if (text[position] == '\\' && position + 1 < text.Length && @"\\`*_{}[]()#+-.!|>".Contains(text[position + 1].ToString()))
                {
                    target.Add(new Run(text[position + 1].ToString()));
                    position += 2;
                    continue;
                }

                if (text[position] == '`')
                {
                    var end = text.IndexOf('`', position + 1);
                    if (end > position + 1)
                    {
                        target.Add(new Run(text.Substring(position + 1, end - position - 1))
                        {
                            FontFamily = new FontFamily("Consolas"),
                            FontSize = 13,
                            Background = codeBackground,
                            Foreground = foreground
                        });
                        position = end + 1;
                        continue;
                    }
                }

                if (position + 1 < text.Length && (text.Substring(position, 2) == "**" || text.Substring(position, 2) == "__"))
                {
                    var marker = text.Substring(position, 2);
                    var end = text.IndexOf(marker, position + 2, StringComparison.Ordinal);
                    if (end > position + 2)
                    {
                        var bold = new Bold();
                        AddInlines(bold.Inlines, text.Substring(position + 2, end - position - 2), openLink, foreground, accent, codeBackground);
                        target.Add(bold);
                        position = end + 2;
                        continue;
                    }
                }

                if (position + 1 < text.Length && text.Substring(position, 2) == "~~")
                {
                    var end = text.IndexOf("~~", position + 2, StringComparison.Ordinal);
                    if (end > position + 2)
                    {
                        var strike = new Span { TextDecorations = TextDecorations.Strikethrough };
                        AddInlines(strike.Inlines, text.Substring(position + 2, end - position - 2), openLink, foreground, accent, codeBackground);
                        target.Add(strike);
                        position = end + 2;
                        continue;
                    }
                }

                var image = position + 1 < text.Length && text[position] == '!' && text[position + 1] == '[';
                if (text[position] == '[' || image)
                {
                    var labelStart = position + (image ? 2 : 1);
                    var labelEnd = text.IndexOf(']', labelStart);
                    if (labelEnd >= labelStart && labelEnd + 1 < text.Length && text[labelEnd + 1] == '(')
                    {
                        var targetEnd = text.IndexOf(')', labelEnd + 2);
                        if (targetEnd > labelEnd + 2)
                        {
                            var label = text.Substring(labelStart, labelEnd - labelStart);
                            var destination = text.Substring(labelEnd + 2, targetEnd - labelEnd - 2).Trim().Trim('<', '>');
                            var link = new Hyperlink(new Run(image ? Localization.Get("Image") + ": " + label : label))
                            {
                                Foreground = accent,
                                TextDecorations = image ? null : TextDecorations.Underline,
                                ToolTip = destination,
                                Cursor = System.Windows.Input.Cursors.Hand
                            };
                            link.Click += delegate { openLink(destination); };
                            target.Add(link);
                            position = targetEnd + 1;
                            continue;
                        }
                    }
                }

                if (text[position] == '*' || text[position] == '_')
                {
                    var marker = text[position];
                    var end = text.IndexOf(marker, position + 1);
                    if (end > position + 1)
                    {
                        var italic = new Italic();
                        AddInlines(italic.Inlines, text.Substring(position + 1, end - position - 1), openLink, foreground, accent, codeBackground);
                        target.Add(italic);
                        position = end + 1;
                        continue;
                    }
                }

                var next = FindNextMarkup(text, position + 1);
                target.Add(new Run(text.Substring(position, next - position)));
                position = next;
            }
        }

        private static int FindNextMarkup(string text, int start)
        {
            for (var i = start; i < text.Length; i++)
            {
                if (text[i] == '\\' || text[i] == '`' || text[i] == '*' || text[i] == '_' ||
                    text[i] == '[' || text[i] == '!' || text[i] == '~')
                    return i;
            }
            return text.Length;
        }

        private static SolidColorBrush Color(string value)
        {
            var brush = (SolidColorBrush)new BrushConverter().ConvertFromString(value);
            brush.Freeze();
            return brush;
        }
    }
}
