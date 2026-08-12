using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace MdLight
{
    internal static class MarkdownSerializer
    {
        public static string Serialize(FlowDocument document)
        {
            var output = new StringBuilder();
            foreach (var block in document.Blocks)
                AppendBlock(output, block, 0);
            return output.ToString().TrimEnd() + Environment.NewLine;
        }

        private static void AppendBlock(StringBuilder output, Block block, int depth)
        {
            var paragraph = block as Paragraph;
            if (paragraph != null)
            {
                AppendParagraph(output, paragraph, depth);
                return;
            }

            var list = block as List;
            if (list != null)
            {
                var number = 1;
                foreach (var item in list.ListItems)
                {
                    var marker = list.MarkerStyle == TextMarkerStyle.Decimal ? number++ + ". " : "- ";
                    var first = item.Blocks.FirstBlock as Paragraph;
                    output.Append(new string(' ', depth * 2)).Append(marker);
                    if (first != null)
                    {
                        var itemText = SerializeInlines(first.Inlines);
                        if (itemText.StartsWith("☐ ", StringComparison.Ordinal))
                            itemText = "[ ] " + itemText.Substring(2);
                        else if (itemText.StartsWith("☑ ", StringComparison.Ordinal))
                            itemText = "[x] " + itemText.Substring(2);
                        output.Append(itemText);
                    }
                    output.AppendLine();
                    foreach (var nested in item.Blocks.Skip(1))
                        AppendBlock(output, nested, depth + 1);
                }
                output.AppendLine();
                return;
            }

            var table = block as Table;
            if (table != null)
            {
                AppendTable(output, table);
                return;
            }

            var section = block as Section;
            if (section != null)
            {
                foreach (var child in section.Blocks)
                    AppendBlock(output, child, depth);
            }
        }

        private static void AppendParagraph(StringBuilder output, Paragraph paragraph, int depth)
        {
            var text = SerializeInlines(paragraph.Inlines);
            var tag = paragraph.Tag as string;
            if (string.Equals(tag, "divider", StringComparison.Ordinal))
            {
                output.AppendLine("---").AppendLine();
                return;
            }
            if (tag != null && tag.StartsWith("code:", StringComparison.Ordinal))
            {
                output.Append("```").AppendLine(tag.Substring(5));
                output.AppendLine(new TextRange(paragraph.ContentStart, paragraph.ContentEnd).Text.TrimEnd('\r', '\n'));
                output.AppendLine("```").AppendLine();
                return;
            }
            if (string.Equals(tag, "quote", StringComparison.Ordinal))
            {
                foreach (var line in text.Replace("\r\n", "\n").Split('\n'))
                    output.Append("> ").AppendLine(line);
                output.AppendLine();
                return;
            }
            var headingLevel = HeadingLevel(paragraph);
            if (headingLevel > 0)
                output.Append(new string('#', headingLevel)).Append(' ');

            if (paragraph.TextAlignment == TextAlignment.Center || paragraph.TextAlignment == TextAlignment.Right)
            {
                var alignment = paragraph.TextAlignment == TextAlignment.Center ? "center" : "right";
                output.Append("<p align=\"").Append(alignment).Append("\">").Append(text).AppendLine("</p>");
            }
            else
            {
                output.Append(text).AppendLine();
            }
            output.AppendLine();
        }

        private static void AppendTable(StringBuilder output, Table table)
        {
            var rows = table.RowGroups.SelectMany(group => group.Rows).ToList();
            if (rows.Count == 0)
                return;

            var columns = rows.Max(row => row.Cells.Count);
            AppendTableRow(output, rows[0], columns);
            output.Append('|');
            for (var index = 0; index < columns; index++)
            {
                var alignment = GetCellAlignment(rows[0], index);
                output.Append(alignment == TextAlignment.Center ? " :---: |" :
                    alignment == TextAlignment.Left ? " :--- |" :
                    alignment == TextAlignment.Right ? " ---: |" : " --- |" );
            }
            output.AppendLine();
            foreach (var row in rows.Skip(1))
                AppendTableRow(output, row, columns);
            output.AppendLine();
        }

        private static void AppendTableRow(StringBuilder output, TableRow row, int columns)
        {
            output.Append('|');
            for (var index = 0; index < columns; index++)
            {
                var value = index < row.Cells.Count ? SerializeCell(row.Cells[index]) : string.Empty;
                output.Append(' ').Append(value.Replace("|", "\\|").Replace("\r", "").Replace("\n", "<br>"))
                    .Append(" |");
            }
            output.AppendLine();
        }

        private static TextAlignment GetCellAlignment(TableRow row, int index)
        {
            if (index >= row.Cells.Count)
                return TextAlignment.Left;
            var paragraph = row.Cells[index].Blocks.FirstBlock as Paragraph;
            return paragraph == null ? TextAlignment.Left : paragraph.TextAlignment;
        }

        private static string SerializeCell(TableCell cell)
        {
            return string.Join("<br>", cell.Blocks.OfType<Paragraph>().Select(paragraph => SerializeInlines(paragraph.Inlines)));
        }

        private static string SerializeInlines(InlineCollection inlines)
        {
            var output = new StringBuilder();
            foreach (var inline in inlines)
                AppendInline(output, inline);
            return output.ToString();
        }

        private static void AppendInline(StringBuilder output, Inline inline)
        {
            var run = inline as Run;
            if (run != null)
            {
                if (string.Equals(run.Tag as string, "inline-code", StringComparison.Ordinal))
                {
                    output.Append('`').Append(run.Text.Replace("`", "\\`")).Append('`');
                    return;
                }
                var runContent = Escape(run.Text);
                var localBold = run.ReadLocalValue(TextElement.FontWeightProperty) != DependencyProperty.UnsetValue &&
                                run.FontWeight >= FontWeights.SemiBold;
                var localItalic = run.ReadLocalValue(TextElement.FontStyleProperty) != DependencyProperty.UnsetValue &&
                                  run.FontStyle == FontStyles.Italic;
                if (localBold && localItalic)
                    output.Append("***").Append(runContent).Append("***");
                else if (localBold)
                    output.Append("**").Append(runContent).Append("**");
                else if (localItalic)
                    output.Append('*').Append(runContent).Append('*');
                else
                    output.Append(runContent);
                return;
            }

            if (inline is LineBreak)
            {
                output.Append("  ").AppendLine();
                return;
            }

            var hyperlink = inline as Hyperlink;
            if (hyperlink != null)
            {
                var label = SerializeInlines(hyperlink.Inlines);
                var target = hyperlink.ToolTip == null ? string.Empty : hyperlink.ToolTip.ToString();
                var tag = hyperlink.Tag as string;
                if (tag != null && tag.StartsWith("image:", StringComparison.Ordinal))
                {
                    output.Append("![").Append(tag.Substring(6)).Append("](").Append(target).Append(')');
                    return;
                }
                output.Append('[').Append(label).Append("](").Append(target).Append(')');
                return;
            }

            var span = inline as Span;
            if (span == null)
                return;

            var content = SerializeInlines(span.Inlines);
            if (inline is Bold)
                output.Append("**").Append(content).Append("**");
            else if (inline is Italic)
                output.Append('*').Append(content).Append('*');
            else if (span.TextDecorations == TextDecorations.Strikethrough)
                output.Append("~~").Append(content).Append("~~");
            else
                output.Append(content);
        }

        private static int HeadingLevel(Paragraph paragraph)
        {
            if (paragraph.FontSize >= 28) return 1;
            if (paragraph.FontSize >= 23) return 2;
            if (paragraph.FontSize >= 20) return 3;
            if (paragraph.FontSize >= 17) return 4;
            return 0;
        }

        private static string Escape(string text)
        {
            return (text ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("`", "\\`")
                .Replace("*", "\\*")
                .Replace("_", "\\_")
                .Replace("[", "\\[")
                .Replace("]", "\\]");
        }
    }
}
