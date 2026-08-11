using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
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
        private FileSystemWatcher watcher;
        private readonly DispatcherTimer reloadTimer;

        public MainWindow()
        {
            InitializeComponent();
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
            try
            {
                var fullPath = Path.GetFullPath(path);
                if (!File.Exists(fullPath))
                    throw new FileNotFoundException("Файл не найден.", fullPath);

                currentPath = fullPath;
                LoadCurrentFile(true);
                StartWatching();
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, "Не удалось открыть файл",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadCurrentFile(bool resetScroll)
        {
            try
            {
                using (var reader = new StreamReader(currentPath, Encoding.UTF8, true))
                    currentMarkdown = reader.ReadToEnd();

                Viewer.Document = MarkdownRenderer.Render(currentMarkdown, OpenLink, darkTheme);
                Viewer.Visibility = Visibility.Visible;
                EmptyState.Visibility = Visibility.Collapsed;
                FileCaption.Text = Path.GetFileName(currentPath);
                Title = Path.GetFileName(currentPath) + " — MdLight";
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
                StatusText.Text = "Ошибка: " + exception.Message;
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
                        throw new InvalidOperationException("Этот тип ссылки не поддерживается.");

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
                MessageBox.Show(this, exception.Message, "Не удалось открыть ссылку",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Открыть Markdown",
                Filter = "Markdown (*.md;*.markdown)|*.md;*.markdown|Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"
            };
            if (dialog.ShowDialog(this) == true)
                OpenPath(dialog.FileName);
        }

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            darkTheme = !darkTheme;
            ApplyTheme();
            if (currentMarkdown != null)
                Viewer.Document = MarkdownRenderer.Render(currentMarkdown, OpenLink, darkTheme);
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
            ThemeButton.Content = darkTheme ? "Светлая тема" : "Тёмная тема";
            ThemeButton.Foreground = Brush(darkTheme ? "#FFE5E7EB" : "#FF374151");
            ThemeButton.BorderBrush = Brush(darkTheme ? "#FF4B5563" : "#FFD1D5DB");
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
            else if ((e.Key == Key.R && Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) || e.Key == Key.F5)
            {
                if (!string.IsNullOrEmpty(currentPath))
                    LoadCurrentFile(false);
                e.Handled = true;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (watcher != null)
                watcher.Dispose();
            base.OnClosed(e);
        }

        private static SolidColorBrush Brush(string color)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFromString(color);
        }

        private static string FormatSize(long bytes)
        {
            if (bytes < 1024) return bytes + " Б";
            if (bytes < 1024 * 1024) return (bytes / 1024d).ToString("0.0") + " КБ";
            return (bytes / 1024d / 1024d).ToString("0.0") + " МБ";
        }
    }
}
