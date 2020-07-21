using Newtonsoft.Json;
using Shareson.Support;
using SharesonServer.Enum;
using SharesonServer.Model.Support;

namespace SharesonServer.Repository.SupportFunctions
{
    public class RequestsReader
    {
        private Log Error = new Log($@"C:\Users\Reneshi\Downloads", "ErrorLogServer.txt");
        private SqlHelper sql = new SqlHelper();
        private AccountModel accountModel;
        private string PathToFolder;
        private string FileName;
        private string Email;
        private string Password;
        private string[] ExcludedExstensions;

        public byte[] ConvertRequest(string rawRequest)
        {
            string Method;
            string Request = rawRequest;

            if (Request.Contains("<Meth>"))
            {
                Method = Request.Remove(Request.IndexOf("<Meth>"));
                int startAt = Request.IndexOf(Method);
                int EndAt = "<Meth>".Length + Method.Length;
                Request = Request.Remove(startAt, EndAt);
            }
            else
            {
                Method = "";
            }

            return Result(GetServerMethod(Method), Request);
        }

        private AvailableImageMethods GetServerMethod(string Method)
        {
            AvailableImageMethods availableMethod;
            //string[] separators = { "<Meth>", "<PTDir>", "<File>", "<ExclExt>"};            

            foreach (var item in (AvailableImageMethods[]) System.Enum.GetValues(typeof(AvailableImageMethods)))
            {
                if(System.Enum.GetName(typeof(AvailableImageMethods), item) == Method)
                {
                    availableMethod = item;
                    Method = null;
                    return availableMethod;
                }
            }
            return availableMethod = AvailableImageMethods.GetImage; // default
        }

        private AccountModel DeserializeLoginRequest(string request)
        {
            accountModel = new AccountModel();
            accountModel = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.Support.AccountModel>(request);

            this.Email = accountModel.Email;
            this.Password = accountModel.Password;

            return accountModel;
        }
        private ReceivedImagesRequestModel DeserializeImagesRequestAsJson(string request)
        {
            Model.Support.ReceivedImagesRequestModel model = new Model.Support.ReceivedImagesRequestModel();
            model = JsonConvert.DeserializeObject<ReceivedImagesRequestModel>(request);

            this.PathToFolder = model.PathToDirectory;
            this.FileName = model.FileName;
            this.ExcludedExstensions = model.ExcludedExtensions;
            return model;
        }

        private byte[] Result(AvailableImageMethods Method, string request)
        {
            ImageOptions convert = new ImageOptions();

            switch (Method)
            {
                case AvailableImageMethods.GetImagesList:
                    {
                        return null;
                    }
                case AvailableImageMethods.GetImage:
                    {
                        DeserializeImagesRequestAsJson(request);
                        var result = convert.GetImageAsBytes(PathToFolder, FileName, ExcludedExstensions);
                        return result;
                    }
                case AvailableImageMethods.GetRandomImage:
                    {
                        DeserializeImagesRequestAsJson(request);
                        var result = convert.GetImageAsBytes(PathToFolder, Model.TotalImages.GetRandom(PathToFolder), ExcludedExstensions);
                        return result;
                    }
                case AvailableImageMethods.PutImage:
                    {
                        return null;
                    }
                case AvailableImageMethods.GetImageInfo:
                    {
                        var result = convert.GetImageInfo(PathToFolder, FileName);
                        return null;
                    }
                case AvailableImageMethods.LoginToAccount:
                    {
                        DeserializeLoginRequest(request);
                        accountModel.ID = sql.LogInToUserAccount(Email, Password);
                        string json = JsonConvert.SerializeObject(accountModel);
                        var result = System.Text.Encoding.ASCII.GetBytes(json); 
                        return result;
                    }
                case AvailableImageMethods.GetAccountInfo:
                    {
                        var result = new byte[1];
                        return result;
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }
}
