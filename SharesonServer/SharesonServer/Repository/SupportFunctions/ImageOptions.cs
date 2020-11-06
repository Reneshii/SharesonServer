using Newtonsoft.Json;
using SharesonServer.Model;
using System.IO;
using System.Text;

namespace SharesonServer.Repository.SupportFunctions
{
    public class ImageOptions
    {
        private ImageOptionsModel model = new ImageOptionsModel();

        private ImageOptionsModel GetImageInfo(string PathToFolder, string FileName)
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

                    //string json = JsonConvert.SerializeObject(model);

                    //dataToReturn = Encoding.ASCII.GetBytes(json);

                    //return dataToReturn;
                    return model;
                }
            }
            return null;
        }

        public byte[] GetImageWithInfoAsBytes(string PathToFolder, string FileName, string[] ExcludedExtensions = null)
        {
            ImageOptionsModel model = GetImageInfo(PathToFolder, FileName);
            byte[] dataToReturn;

            if (Directory.Exists(PathToFolder))
            {
                string searchingFile = PathToFolder + FileName;

                if (File.Exists(searchingFile))
                {
                    if (ExcludedExtensions != null && ExcludedExtensions.Length > 0)
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

                    model.Size = info.Length;
                    model.Image = dataToReturn;

                    var json = JsonConvert.SerializeObject(model);
                    var jsonAsBytes = Encoding.ASCII.GetBytes(json);
                    
                    return jsonAsBytes;
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

                    model.Size = info.Length;
                    model.Image = dataToReturn;

                    var json = JsonConvert.SerializeObject(model);
                    var jsonAsBytes = Encoding.ASCII.GetBytes(json);

                    return jsonAsBytes;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
