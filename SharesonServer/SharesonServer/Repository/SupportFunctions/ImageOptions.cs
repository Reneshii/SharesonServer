using Newtonsoft.Json;
using Shareson.Support;
using SharesonServer.Model;
using System;
using System.IO;
using System.Text;

namespace SharesonServer.Repository.SupportFunctions
{
    public class ImageOptions
    {
        private ImageOptionsModel model = new ImageOptionsModel();
        Log error = new Log($@"C:\Users\Reneshi\Downloads", "ImageConvertErrorLogServer.txt");

        public byte[] GetImageInfo(string PathToFolder, string FileName)
        {
            byte[] dataToReturn;

            if (Directory.Exists(PathToFolder))
            {
                string searchingFile = PathToFolder + FileName;

                if (File.Exists(searchingFile))
                {
                    FileInfo info = new FileInfo(searchingFile);
                    dataToReturn = new byte[info.Length];

                    model.Folder = info.DirectoryName;
                    model.Name = info.Name;
                    model.CreationTime = info.CreationTime.Date.ToString("dd/MM/yyyy");
                    model.Creator = File.GetAccessControl(searchingFile).GetOwner(typeof(System.Security.Principal.NTAccount)).ToString();

                    string json = JsonConvert.SerializeObject(model);

                    dataToReturn = Encoding.ASCII.GetBytes(json);

                    return dataToReturn;
                }
            }
            return null;
        }

        public byte[] GetImageAsBytes(string PathToFolder, string FileName, string[] ExcludedExtensions = null)
        {
            try
            {
                GetImageInfo(PathToFolder, FileName);
                byte[] dataToReturn;

                if (Directory.Exists(PathToFolder))
                {
                    string searchingFile = PathToFolder + FileName;

                    if (File.Exists(searchingFile))
                    {
                        if(ExcludedExtensions != null && ExcludedExtensions.Length > 0)
                        foreach (var item in ExcludedExtensions)
                        {
                            if (FileName.Contains(item))
                            {
                                searchingFile = PathToFolder + "ExcludedExtension.png";
                            }
                        }

                        FileInfo info = new FileInfo(searchingFile);
                        dataToReturn = new byte[info.Length];
                        using (FileStream fstream = File.OpenRead(searchingFile))
                        {
                            fstream.Read(dataToReturn, 0, dataToReturn.Length);
                        }
                        return dataToReturn;
                    }
                    else
                    {
                        searchingFile = PathToFolder + "FileDoNotExist.png";
                        FileInfo info = new FileInfo(searchingFile);
                        dataToReturn = new byte[info.Length];
                        using (FileStream fstream = File.OpenRead(searchingFile))
                        {
                            fstream.Read(dataToReturn, 0, dataToReturn.Length);
                        }
                        return dataToReturn;
                        
                    }
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                error.Add(e.ToString() + "\nERROR ZWIĄZANY Z PRZETWARZANIEM OBRAZU");
                return null;
            }

        }
    }
}
