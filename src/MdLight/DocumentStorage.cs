using System.IO;
using System.Text;

namespace MdLight
{
    internal static class DocumentStorage
    {
        public static string Read(string path)
        {
            using (var reader = new StreamReader(path, Encoding.UTF8, true))
                return reader.ReadToEnd();
        }

        public static void Write(string path, string markdown)
        {
            using (var writer = new StreamWriter(path, false, new UTF8Encoding(false)))
                writer.Write(markdown ?? string.Empty);
        }
    }
}
