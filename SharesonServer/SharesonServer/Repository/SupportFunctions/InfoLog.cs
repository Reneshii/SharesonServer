using System;
using System.IO;
using System.Text;

namespace Shareson.Support
{
    public class InfoLog
    {
        public string PathToDirectory { get; }
        public string PathToFile { get; }
        public string PathToFileLog { get; set; }

        public InfoLog(string path)
        {
            if (path.Contains(".txt"))
            {
                var stop = path.LastIndexOf('\\');
                PathToDirectory = path.Remove(stop, path.Remove(0, stop).Length);
                PathToFile = path.Remove(0, stop + 1);
            }
            else
            {
                PathToFile = $@"\ErrorLogServer.txt";
                PathToFile = PathToFile.Remove(0, 1);
            }
        }

        public void Add(string error)
        {
            FindDirectory(PathToDirectory, error);
        }

        private void FindDirectory(string path, string error)
        {
            if (Directory.Exists(path))
            {
                WorkWithFile(error);
            }
            else
            {
                Directory.CreateDirectory(path);
                WorkWithFile(error);
            }
        }

        private void WorkWithFile(string error)
        {
            using (FileStream file = new FileStream(PathToDirectory + '\\' + PathToFile, FileMode.Append))
            {
                using (StreamWriter stream = new StreamWriter(file, encoding: Encoding.UTF8))
                {
                    stream.Write(DateTime.Now.ToString("h:mm:ss:yyyy") + "\n" + error + "\n");
                }
            }
        }
    }
}
