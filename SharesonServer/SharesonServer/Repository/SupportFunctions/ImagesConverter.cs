using Newtonsoft.Json;
using Shareson.Support;
using SharesonServer.Model;
using System.IO;
using System.Text;

namespace SharesonServer.Repository.SupportFunctions
{
    public class ImagesConverter
    {
        private ImageConverterModel model = new ImageConverterModel();
        private InfoLog log = new InfoLog(Properties.Settings.Default.LogsFilePath);
        private bool ImageInfo = false;

        private ImageConverterModel GetImageInfo(string FileFullPath)
        {
            byte[] dataToReturn;
            if(ImageInfo == false)
            {
                FileInfo info = new FileInfo(FileFullPath);
                dataToReturn = new byte[info.Length];

                model.Folder = info.DirectoryName;
                model.Name = info.Name;
                model.CreationTime = info.CreationTime.Date.ToString("dd/MM/yyyy");
                //model.Creator = File.GetAccessControl(searchingFile).GetOwner(typeof(System.Security.Principal.NTAccount)).ToString();

                ImageInfo = true;
                return model;
            }
            return new ImageConverterModel();
        }
        public byte[] ReturnImageWithInfoAsBytes(string PathToFolder, string FileName, string[] ExcludedExtensions = null)
        {
            try
            {
                ImageConverterModel model;
                byte[] dataToReturn = new byte[0];
                string searchingFile; 

                if(string.IsNullOrEmpty(FileName))
                {
                    searchingFile = PathToFolder + All_Images.GetRandom(PathToFolder, ExcludedExtensions);
                    model = GetImageInfo(searchingFile);
                }
                else
                {
                    searchingFile = PathToFolder + FileName;
                    model = GetImageInfo(searchingFile);
                }

                
                FileInfo info = new FileInfo(searchingFile);

                dataToReturn = new byte[info.Length];
                if (info.Length > 0)
                {
                    using (FileStream fstream = File.OpenRead(searchingFile))
                    {
                        fstream.Read(dataToReturn, 0, dataToReturn.Length);
                    }
                }

                model.Size = info.Length;
                model.Image = dataToReturn;

                var json = JsonConvert.SerializeObject(model);
                var jsonAsBytes = Encoding.ASCII.GetBytes(json);

                ImageInfo = false;
                return jsonAsBytes;
            }
            catch(System.Exception e)
            {
                var json = JsonConvert.SerializeObject(model);
                var jsonAsBytes = Encoding.ASCII.GetBytes(json);

                ImageInfo = false;
                log.Add(e.ToString());
                return jsonAsBytes;
            }
            
        }
    }
}
