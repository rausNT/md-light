using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        private bool suppressEditorChanged;
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

                suppressEditorChanged = true;
                Editor.Text = currentMarkdown;
                suppressEditorChanged = false;
                isDirty = false;
                Viewer.Document = MarkdownRenderer.Render(currentMarkdown, OpenLink, darkTheme);
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
            suppressEditorChanged = true;
            Editor.Text = string.Empty;
            suppressEditorChanged = false;
            isDirty = false;
            EmptyState.Visibility = Visibility.Collapsed;
            SetEditing(true);
            UpdateDocumentTitle();
            StatusText.Text = Localization.Get("Ready");
            Editor.Focus();
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

                currentMarkdown = Editor.Text;
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

        private void SetEditing(bool editing)
        {
            isEditing = editing;
            if (!editing)
            {
                currentMarkdown = Editor.Text;
                Viewer.Document = MarkdownRenderer.Render(currentMarkdown, OpenLink, darkTheme);
            }
            Editor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            Viewer.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            EmptyState.Visibility = currentMarkdown == null ? Visibility.Visible : Visibility.Collapsed;
            ModeButton.Content = Localization.Get(editing ? "Preview" : "Edit");
            if (editing)
                Editor.Focus();
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (suppressEditorChanged)
                return;
            currentMarkdown = Editor.Text;
            isDirty = true;
            UpdateDocumentTitle();
            StatusText.Text = Localization.Get("Modified");
        }

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            darkTheme = !darkTheme;
            ApplyTheme();
            if (currentMarkdown != null)
                Viewer.Document = MarkdownRenderer.Render(currentMarkdown, OpenLink, darkTheme);
        }

        private void LanguageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = LanguageBox.SelectedItem as LanguageOption;
            if (selected == null || selected.Code == Localization.CurrentLanguage)
                return;

            Localization.SetLanguage(selected.Code, true);
            ApplyLocalization();
            if (currentMarkdown != null)
                Viewer.Document = MarkdownRenderer.Render(currentMarkdown, OpenLink, darkTheme);
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
            Editor.Foreground = Brush(darkTheme ? "#FFE5E7EB" : "#FF1F2937");
            Editor.CaretBrush = Brush(darkTheme ? "#FFFFFFFF" : "#FF111827");
            foreach (var button in new[] { NewButton, ModeButton, SaveButton, SaveAsButton })
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
            else if (e.Key == Key.E && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                SetEditing(!isEditing);
                e.Handled = true;
            }
            else if ((e.Key == Key.R && Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) || e.Key == Key.F5)
            {
                if (isDirty)
                    Viewer.Document = MarkdownRenderer.Render(Editor.Text, OpenLink, darkTheme);
                else if (!string.IsNullOrEmpty(currentPath))
                    LoadCurrentFile(false);
                e.Handled = true;
            }
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
