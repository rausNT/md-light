using System;
using System.Linq;
using System.Windows;

namespace MdLight
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Any(arg => string.Equals(arg, "--smoke-test", StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    var sample = "# Title\n\n- one\n- [x] two\n\n> quote\n\n```cs\nvar ok = true;\n```";
                    var document = MarkdownRenderer.Render(sample, delegate { }, false);
                    if (document.Blocks.Count < 4)
                        throw new InvalidOperationException("Markdown smoke test produced too few blocks.");
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
