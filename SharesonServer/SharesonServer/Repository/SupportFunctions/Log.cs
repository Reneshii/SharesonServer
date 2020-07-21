using System;
using System.IO;
using System.Text;

namespace Shareson.Support
{
    public class Log
    {
        public string PathToFolder { get; }
        public string PathToFile { get; }
        public string PathToFileLog { get; set; }

        public Log(string directory, string file)
        {
            PathToFolder = directory;
            PathToFile = file;

            PathToFileLog = directory + PathToFile;
        }

        public void Add(string error)
        {
            if (Directory.Exists(PathToFolder))
            {
                using (FileStream file = new FileStream(PathToFolder + "/" + PathToFile, FileMode.Append))
                {
                    using (StreamWriter stream = new StreamWriter(file, encoding: Encoding.UTF8))
                    {
                        stream.Write(DateTime.Now.ToString("h:mm:ss:yyyy") + "\n" + error + "\n");
                    }
                }
            }
        }
    }
}
