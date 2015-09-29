using System.IO;

namespace Messages
{
    public class Logs
    {
        public static string Get(string root)
        {
            string fullPath = Path.Combine(root, "nsb-logs");
            if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
            return fullPath;
        }
    }
}
