using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace MdLight
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Localization.LoadSavedLanguage();

            if (e.Args.Any(arg => string.Equals(arg, "--smoke-test", StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    Localization.Validate();
                    foreach (var language in Localization.Languages)
                    {
                        Localization.SetLanguage(language.Code, false);
                        var sample = "# Title\n\n- one\n- [x] two\n\n> quote\n\n![image](test.png)\n\n```cs\nvar ok = true;\n```";
                        var document = MarkdownRenderer.Render(sample, delegate { }, false);
                        if (document.Blocks.Count < 5)
                            throw new InvalidOperationException("Markdown smoke test produced too few blocks for " + language.Code + ".");
                    }

                    var smokePath = Path.Combine(Path.GetTempPath(), "MdLight-smoke-" + Guid.NewGuid().ToString("N") + ".md");
                    try
                    {
                        const string savedMarkdown = "# UTF-8\n\nEnglish · Русский · 日本語 · 한국어 · 简体中文";
                        DocumentStorage.Write(smokePath, savedMarkdown);
                        if (DocumentStorage.Read(smokePath) != savedMarkdown)
                            throw new InvalidOperationException("Editor storage smoke test failed.");
                    }
                    finally
                    {
                        if (File.Exists(smokePath))
                            File.Delete(smokePath);
                    }

                    Localization.SetLanguage("en", false);
                    var smokeWindow = new MainWindow();
                    if (smokeWindow.Title != "MdLight")
                        throw new InvalidOperationException("Localized window smoke test failed.");
                    smokeWindow.Close();
                    Environment.ExitCode = 0;
                }
                catch (Exception exception)
                {
                    Console.Error.WriteLine(exception);
                    Environment.ExitCode = 1;
                }

                Shutdown(Environment.ExitCode);
                return;
            }

            var window = new MainWindow();
            MainWindow = window;
            window.Show();

            var file = e.Args.FirstOrDefault(arg => !arg.StartsWith("-", StringComparison.Ordinal));
            if (!string.IsNullOrWhiteSpace(file))
                window.OpenPath(file);
        }
    }
}
