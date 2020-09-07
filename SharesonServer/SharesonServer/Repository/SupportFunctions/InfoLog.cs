using System;
using System.IO;
using System.Text;

namespace Shareson.Support
{
    public class InfoLog
    {
        public string PathToFolder { get; }
        public string FileName { get; }
        public string PathToFileLog { get; set; }

        public InfoLog(string directory, string file)
        {
            PathToFolder = directory;
            FileName = file;

            PathToFileLog = directory + FileName;
        }

        public void Add(string error)
        {
            if (Directory.Exists(PathToFolder))
            {
                using (FileStream file = new FileStream(PathToFolder + "/" + FileName, FileMode.Append))
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
