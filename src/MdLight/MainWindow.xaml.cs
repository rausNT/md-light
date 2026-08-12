using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MdLight
{
    public partial class MainWindow : Window
    {
        private string currentPath;
        private string currentMarkdown;
        private bool darkTheme;
        private bool isEditing;
        private bool isDirty;
        private bool suppressDocumentChanged;
        private DateTime ignoreWatcherUntil;
        private FileSystemWatcher watcher;
        private readonly DispatcherTimer reloadTimer;

        public MainWindow()
        {
            InitializeComponent();
            LanguageBox.ItemsSource = Localization.Languages;
            LanguageBox.SelectedItem = LanguageBox.Items.Cast<LanguageOption>()
                .First(option => option.Code == Localization.CurrentLanguage);
            ApplyLocalization();
            reloadTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
            reloadTimer.Tick += delegate
            {
                reloadTimer.Stop();
                if (!string.IsNullOrEmpty(currentPath) && File.Exists(currentPath))
                    LoadCurrentFile(false);
            };
        }

        public void OpenPath(string path)
        {
            if (!ConfirmSaveChanges())
                return;

            try
            {
                var fullPath = Path.GetFullPath(path);
                if (!File.Exists(fullPath))
                    throw new FileNotFoundException(Localization.Get("FileNotFound"), fullPath);

                currentPath = fullPath;
                isEditing = false;
                LoadCurrentFile(true);
                StartWatching();
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, Localization.Get("OpenFileError"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadCurrentFile(bool resetScroll)
        {
            try
            {
                var restoreEditing = isEditing;
                currentMarkdown = DocumentStorage.Read(currentPath);

                suppressDocumentChanged = true;
                Viewer.Document = MarkdownRenderer.Render(currentMarkdown, OpenLink, darkTheme);
                suppressDocumentChanged = false;
                isDirty = false;
                SetEditing(restoreEditing);
                EmptyState.Visibility = Visibility.Collapsed;
                UpdateDocumentTitle();
                StatusText.Text = currentPath + "  ·  " + FormatSize(new FileInfo(currentPath).Length);

                if (resetScroll)
                    Viewer.ScrollToHome();
            }
            catch (IOException)
            {
                reloadTimer.Stop();
                reloadTimer.Start();
            }
            catch (Exception exception)
            {
                StatusText.Text = Localization.Get("Error") + ": " + exception.Message;
            }
        }

        private void StartWatching()
        {
            if (watcher != null)
            {
                watcher.Dispose();
                watcher = null;
            }

            watcher = new FileSystemWatcher(Path.GetDirectoryName(currentPath), Path.GetFileName(currentPath));
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName;
            watcher.Changed += FileChanged;
            watcher.Renamed += FileChanged;
            watcher.EnableRaisingEvents = true;
        }

        private void FileChanged(object sender, FileSystemEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                if (DateTime.UtcNow < ignoreWatcherUntil)
                    return;
                if (isDirty)
                {
                    StatusText.Text = Localization.Get("ExternalChange");
                    return;
                }
                reloadTimer.Stop();
                reloadTimer.Start();
            }));
        }

        private void OpenLink(string target)
        {
            try
            {
                Uri absolute;
                if (Uri.TryCreate(target, UriKind.Absolute, out absolute))
                {
                    if (absolute.Scheme != Uri.UriSchemeHttp && absolute.Scheme != Uri.UriSchemeHttps &&
                        absolute.Scheme != Uri.UriSchemeMailto && absolute.Scheme != Uri.UriSchemeFile)
                        throw new InvalidOperationException(Localization.Get("UnsupportedLink"));

                    Process.Start(new ProcessStartInfo(absolute.AbsoluteUri) { UseShellExecute = true });
                    return;
                }

                if (string.IsNullOrEmpty(currentPath))
                    return;

                var localPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(currentPath), target));
                if ((string.Equals(Path.GetExtension(localPath), ".md", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(Path.GetExtension(localPath), ".markdown", StringComparison.OrdinalIgnoreCase)) &&
                    File.Exists(localPath))
                {
                    OpenPath(localPath);
                }
                else if (File.Exists(localPath) || Directory.Exists(localPath))
                {
                    Process.Start(new ProcessStartInfo(localPath) { UseShellExecute = true });
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, Localization.Get("OpenLinkError"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = Localization.Get("OpenDialog"),
                Filter = Localization.Get("MarkdownFiles") + " (*.md;*.markdown)|*.md;*.markdown|" +
                         Localization.Get("TextFiles") + " (*.txt)|*.txt|" +
                         Localization.Get("AllFiles") + " (*.*)|*.*"
            };
            if (dialog.ShowDialog(this) == true)
                OpenPath(dialog.FileName);
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmSaveChanges())
                return;

            StopWatching();
            currentPath = null;
            currentMarkdown = string.Empty;
            suppressDocumentChanged = true;
            Viewer.Document = CreateBlankDocument();
            suppressDocumentChanged = false;
            isDirty = false;
            EmptyState.Visibility = Visibility.Collapsed;
            SetEditing(true);
            UpdateDocumentTitle();
            StatusText.Text = Localization.Get("Ready");
            Viewer.Focus();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveDocument(false);
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            SaveDocument(true);
        }

        private bool SaveDocument(bool saveAs)
        {
            try
            {
                var path = currentPath;
                if (saveAs || string.IsNullOrEmpty(path))
                {
                    var dialog = new SaveFileDialog
                    {
                        Title = Localization.Get("SaveDialog"),
                        FileName = string.IsNullOrEmpty(path) ? Localization.Get("Untitled") + ".md" : Path.GetFileName(path),
                        DefaultExt = ".md",
                        AddExtension = true,
                        Filter = Localization.Get("MarkdownFiles") + " (*.md;*.markdown)|*.md;*.markdown|" +
                                 Localization.Get("TextFiles") + " (*.txt)|*.txt|" +
                                 Localization.Get("AllFiles") + " (*.*)|*.*"
                    };
                    if (dialog.ShowDialog(this) != true)
                        return false;
                    path = dialog.FileName;
                }

                currentMarkdown = MarkdownSerializer.Serialize(Viewer.Document);
                ignoreWatcherUntil = DateTime.UtcNow.AddSeconds(1);
                DocumentStorage.Write(path, currentMarkdown);
                currentPath = Path.GetFullPath(path);
                isDirty = false;
                StartWatching();
                Viewer.Document = MarkdownRenderer.Render(currentMarkdown, OpenLink, darkTheme);
                UpdateDocumentTitle();
                StatusText.Text = Localization.Get("Saved") + "  ·  " + currentPath;
                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, Localization.Get("SaveError"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        private bool ConfirmSaveChanges()
        {
            if (!isDirty)
                return true;

            var answer = MessageBox.Show(this, Localization.Get("UnsavedPrompt"), Localization.Get("UnsavedTitle"),
                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (answer == MessageBoxResult.Cancel)
                return false;
            return answer != MessageBoxResult.Yes || SaveDocument(false);
        }

        private void ModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentMarkdown == null)
            {
                NewButton_Click(sender, e);
                return;
            }
            SetEditing(!isEditing);
        }

        private void Heading1Button_Click(object sender, RoutedEventArgs e)
        {
            ApplyParagraphStyle(30, FontWeights.SemiBold);
        }

        private void Heading2Button_Click(object sender, RoutedEventArgs e)
        {
            ApplyParagraphStyle(25, FontWeights.SemiBold);
        }

        private void NormalButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyParagraphStyle(15, FontWeights.Normal);
        }

        private void ApplyParagraphStyle(double fontSize, FontWeight fontWeight)
        {
            if (!isEditing)
                return;
            var paragraph = Viewer.CaretPosition.Paragraph;
            if (paragraph != null)
            {
                paragraph.FontSize = fontSize;
                paragraph.FontWeight = fontWeight;
            }
            MarkDirty();
            Viewer.Focus();
        }

        private void FormattingButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isEditing)
                return;
            Dispatcher.BeginInvoke(new Action(MarkDirty), DispatcherPriority.Background);
        }

        private void MarkDirty()
        {
            isDirty = true;
            UpdateDocumentTitle();
            StatusText.Text = Localization.Get("Modified");
        }

        private void TableButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isEditing)
                return;

            var table = new System.Windows.Documents.Table
            {
                CellSpacing = 0,
                Margin = new Thickness(0, 7, 0, 16),
                BorderBrush = Brush(darkTheme ? "#FF4B5563" : "#FFD1D5DB"),
                BorderThickness = new Thickness(1)
            };
            for (var column = 0; column < 3; column++)
                table.Columns.Add(new TableColumn());
            var group = new TableRowGroup();
            table.RowGroups.Add(group);
            for (var rowIndex = 0; rowIndex < 3; rowIndex++)
            {
                var row = new TableRow();
                group.Rows.Add(row);
                for (var column = 0; column < 3; column++)
                {
                    var paragraph = new Paragraph(new Run(rowIndex == 0
                        ? Localization.Get("TableHeader")
                        : Localization.Get("TableCell")));
                    if (rowIndex == 0)
                        paragraph.FontWeight = FontWeights.SemiBold;
                    row.Cells.Add(new TableCell(paragraph)
                    {
                        Padding = new Thickness(9, 6, 9, 6),
                        BorderBrush = table.BorderBrush,
                        BorderThickness = new Thickness(0, 0, 1, 1)
                    });
                }
            }

            var paragraphAtCaret = Viewer.CaretPosition.Paragraph;
            if (paragraphAtCaret != null && paragraphAtCaret.Parent == Viewer.Document)
                Viewer.Document.Blocks.InsertAfter(paragraphAtCaret, table);
            else
                Viewer.Document.Blocks.Add(table);
            MarkDirty();
        }

        private void SetEditing(bool editing)
        {
            isEditing = editing;
            if (!editing)
            {
                currentMarkdown = MarkdownSerializer.Serialize(Viewer.Document);
                suppressDocumentChanged = true;
                Viewer.Document = MarkdownRenderer.Render(currentMarkdown, OpenLink, darkTheme);
                suppressDocumentChanged = false;
            }
            else if (string.IsNullOrWhiteSpace(currentMarkdown))
            {
                suppressDocumentChanged = true;
                Viewer.Document = CreateBlankDocument();
                suppressDocumentChanged = false;
            }
            Viewer.IsReadOnly = !editing;
            Viewer.IsDocumentEnabled = !editing;
            Viewer.Visibility = Visibility.Visible;
            FormatToolbar.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            EmptyState.Visibility = currentMarkdown == null ? Visibility.Visible : Visibility.Collapsed;
            ModeButton.Content = Localization.Get(editing ? "Preview" : "Edit");
            if (editing)
                Viewer.Focus();
        }

        private void Viewer_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (suppressDocumentChanged || !isEditing)
                return;
            MarkDirty();
        }

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            if (isEditing)
                currentMarkdown = MarkdownSerializer.Serialize(Viewer.Document);
            darkTheme = !darkTheme;
            ApplyTheme();
            if (currentMarkdown != null)
            {
                suppressDocumentChanged = true;
                Viewer.Document = MarkdownRenderer.Render(currentMarkdown, OpenLink, darkTheme);
                suppressDocumentChanged = false;
            }
        }

        private void LanguageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = LanguageBox.SelectedItem as LanguageOption;
            if (selected == null || selected.Code == Localization.CurrentLanguage)
                return;

            if (isEditing)
                currentMarkdown = MarkdownSerializer.Serialize(Viewer.Document);
            Localization.SetLanguage(selected.Code, true);
            ApplyLocalization();
            if (currentMarkdown != null)
            {
                suppressDocumentChanged = true;
                Viewer.Document = MarkdownRenderer.Render(currentMarkdown, OpenLink, darkTheme);
                suppressDocumentChanged = false;
            }
        }

        private void ApplyLocalization()
        {
            FileCaption.Text = string.IsNullOrEmpty(currentPath)
                ? (currentMarkdown == null ? Localization.Get("DropHint") : Localization.Get("Untitled"))
                : Path.GetFileName(currentPath);
            NewButton.Content = Localization.Get("New");
            SaveButton.Content = Localization.Get("Save");
            SaveAsButton.Content = Localization.Get("SaveAs");
            OpenButton.Content = Localization.Get("Open");
            ShortcutText.Text = Localization.Get("EditorShortcuts");
            EmptyTitle.Text = Localization.Get("EmptyTitle");
            EmptySubtitle.Text = Localization.Get("EmptySubtitle");
            LanguageBox.ToolTip = Localization.Get("Language");
            if (string.IsNullOrEmpty(currentPath))
                StatusText.Text = Localization.Get("Ready");
            ModeButton.Content = Localization.Get(isEditing ? "Preview" : "Edit");
            BoldButton.ToolTip = Localization.Get("Bold");
            ItalicButton.ToolTip = Localization.Get("Italic");
            Heading1Button.ToolTip = Localization.Get("Heading1");
            Heading2Button.ToolTip = Localization.Get("Heading2");
            NormalButton.Content = Localization.Get("NormalText");
            AlignLeftButton.ToolTip = Localization.Get("AlignLeft");
            AlignCenterButton.ToolTip = Localization.Get("AlignCenter");
            AlignRightButton.ToolTip = Localization.Get("AlignRight");
            BulletsButton.Content = Localization.Get("BulletedList");
            NumberingButton.Content = Localization.Get("NumberedList");
            TableButton.Content = Localization.Get("InsertTable");
            UpdateDocumentTitle();
            ApplyTheme();
        }

        private void UpdateDocumentTitle()
        {
            if (string.IsNullOrEmpty(currentPath) && currentMarkdown == null)
            {
                FileCaption.Text = Localization.Get("DropHint");
                Title = "MdLight";
                return;
            }
            var name = string.IsNullOrEmpty(currentPath) ? Localization.Get("Untitled") : Path.GetFileName(currentPath);
            FileCaption.Text = name;
            Title = (isDirty ? "*" : string.Empty) + name + " — MdLight";
        }

        private void ApplyTheme()
        {
            Root.Background = Brush(darkTheme ? "#FF111827" : "#FFF7F7F8");
            Toolbar.Background = StatusBar.Background = Brush(darkTheme ? "#FF1F2937" : "#FFFFFFFF");
            Toolbar.BorderBrush = StatusBar.BorderBrush = Brush(darkTheme ? "#FF374151" : "#FFE5E7EB");
            AppTitle.Foreground = Brush(darkTheme ? "#FFF9FAFB" : "#FF111827");
            FileCaption.Foreground = StatusText.Foreground = Brush(darkTheme ? "#FF9CA3AF" : "#FF6B7280");
            EmptyState.Children[1].SetValue(System.Windows.Controls.TextBlock.ForegroundProperty,
                Brush(darkTheme ? "#FFF9FAFB" : "#FF111827"));
            EmptyState.Children[2].SetValue(System.Windows.Controls.TextBlock.ForegroundProperty,
                Brush(darkTheme ? "#FF9CA3AF" : "#FF6B7280"));
            ThemeButton.Content = Localization.Get(darkTheme ? "LightTheme" : "DarkTheme");
            ThemeButton.Foreground = Brush(darkTheme ? "#FFE5E7EB" : "#FF374151");
            ThemeButton.BorderBrush = Brush(darkTheme ? "#FF4B5563" : "#FFD1D5DB");
            FormatToolbar.Background = Brush(darkTheme ? "#FF1F2937" : "#FFFFFFFF");
            FormatToolbar.BorderBrush = Brush(darkTheme ? "#FF374151" : "#FFE5E7EB");
            Viewer.CaretBrush = Brush(darkTheme ? "#FFFFFFFF" : "#FF111827");
            foreach (var button in new[] { NewButton, ModeButton, SaveButton, SaveAsButton })
            {
                button.Foreground = Brush(darkTheme ? "#FFE5E7EB" : "#FF374151");
                button.BorderBrush = Brush(darkTheme ? "#FF4B5563" : "#FFD1D5DB");
            }
            foreach (var button in new[] { BoldButton, ItalicButton, Heading1Button, Heading2Button, NormalButton,
                AlignLeftButton, AlignCenterButton, AlignRightButton, BulletsButton, NumberingButton, TableButton })
            {
                button.Foreground = Brush(darkTheme ? "#FFE5E7EB" : "#FF374151");
                button.BorderBrush = Brush(darkTheme ? "#FF4B5563" : "#FFD1D5DB");
            }
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            var paths = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (paths != null && paths.Length > 0)
                OpenPath(paths[0]);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (isEditing && (e.Key == Key.Back || e.Key == Key.Delete ||
                (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) &&
                 (e.Key == Key.X || e.Key == Key.V || e.Key == Key.Z || e.Key == Key.Y ||
                  e.Key == Key.B || e.Key == Key.I || e.Key == Key.E || e.Key == Key.L || e.Key == Key.R))))
                MarkDirty();

            if (e.Key == Key.O && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                OpenButton_Click(sender, e);
                e.Handled = true;
            }
            else if (e.Key == Key.N && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                NewButton_Click(sender, e);
                e.Handled = true;
            }
            else if (e.Key == Key.S && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                SaveDocument(Keyboard.Modifiers.HasFlag(ModifierKeys.Shift));
                e.Handled = true;
            }
            else if (e.Key == Key.E && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                SetEditing(!isEditing);
                e.Handled = true;
            }
            else if ((e.Key == Key.R && Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) || e.Key == Key.F5)
            {
                if (isDirty)
                    StatusText.Text = Localization.Get("Modified");
                else if (!string.IsNullOrEmpty(currentPath))
                    LoadCurrentFile(false);
                e.Handled = true;
            }
        }

        private void Window_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (isEditing && !string.IsNullOrEmpty(e.Text))
                MarkDirty();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = !ConfirmSaveChanges();
        }

        protected override void OnClosed(EventArgs e)
        {
            StopWatching();
            base.OnClosed(e);
        }

        private void StopWatching()
        {
            if (watcher == null)
                return;
            watcher.Dispose();
            watcher = null;
        }

        private FlowDocument CreateBlankDocument()
        {
            return new FlowDocument(new Paragraph())
            {
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 15,
                Foreground = Brush(darkTheme ? "#FFE5E7EB" : "#FF1F2937"),
                PagePadding = new Thickness(0),
                LineHeight = 23
            };
        }

        private static SolidColorBrush Brush(string color)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFromString(color);
        }

        private static string FormatSize(long bytes)
        {
            if (bytes < 1024) return bytes + " B";
            if (bytes < 1024 * 1024) return (bytes / 1024d).ToString("0.0") + " KB";
            return (bytes / 1024d / 1024d).ToString("0.0") + " MB";
        }
    }
}
