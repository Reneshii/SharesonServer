using Newtonsoft.Json;
using Shareson.Support;
using SharesonServer.Enum;
using SharesonServer.Model;
using SharesonServer.Model.Support;

namespace SharesonServer.Repository.SupportFunctions
{
    public class RequestsHelper
    {
        private Log Error = new Log($@"C:\Users\Reneshi\Downloads", "ErrorLogServer.txt");
        private SqlHelper sql;

        public RequestsHelper(SqlHelper sqlHelper)
        {
            sql = sqlHelper;
        }

        public ServerMethods GetServerMethod(string methodName)
        {
            ServerMethods availableMethod;
            //string[] separators = { "<Meth>", "<PTDir>", "<File>", "<ExclExt>"};            

            foreach (var item in (ServerMethods[]) System.Enum.GetValues(typeof(ServerMethods)))
            {
                if(System.Enum.GetName(typeof(ServerMethods), item) == methodName)
                {
                    availableMethod = item;
                    return availableMethod;
                }
            }
            return availableMethod = ServerMethods.GetImage; // default
        }

        private AccountModel DeserializeLoginRequest(string request)
        {
            AccountModel accountModel = new AccountModel();
            accountModel = JsonConvert.DeserializeObject<AccountModel>(request);

            return accountModel;
        }

        private ReceivedImagesRequestModel DeserializeImagesRequestAsJson(string request)
        {
            ReceivedImagesRequestModel model = new ReceivedImagesRequestModel();
            model = JsonConvert.DeserializeObject<ReceivedImagesRequestModel>(request);

            return model;
        }

        public byte[] GetImage(string request)
        {
            ImageOptions convert = new ImageOptions();

            var model = DeserializeImagesRequestAsJson(request);
            var result = convert.GetImageAsBytes(model.PathToDirectory, model.FileName, model.ExcludedExtensions);
            return result;
        }
        public byte[] GetRandomImage(string request)
        {
            ImageOptions convert = new ImageOptions();
            var model = DeserializeImagesRequestAsJson(request);
            var result = convert.GetImageAsBytes(model.PathToDirectory, All_Images.GetRandom(model.PathToDirectory), model.ExcludedExtensions);
            return result;
        }

        public void PutImage()
        {

        }

        //public byte[] GetImageInfo(string request)
        //{
        //    ImageOptions convert = new ImageOptions();

        //    var result = convert.GetImageInfo(PathToFolder, FileName);
        //    return null;
        //}

        public void GetImagesList()
        {

        }

        public byte[] LoginToAccount(string request)
        {
            var data = DeserializeLoginRequest(request);
            var ID = sql.LogInToUserAccount(data.Email, data.Password);
            string json = JsonConvert.SerializeObject(ID);
            var result = System.Text.Encoding.ASCII.GetBytes(json);
            return result;
        }

        public byte[] GetAccountInfo()
        {
            var result = new byte[1];
            return result;
        }
        //public byte[] ClientLeave(ref List<Client> list, Socket toRemove )
        //{
        //    lock(list)
        //    {
        //        list.Remove(list.Where(f => f.socket == toRemove).FirstOrDefault());
        //    }
        //    var result = System.Text.Encoding.ASCII.GetBytes("Disconnected");
        //    return result;
        //}

        /// <summary>
        /// Separate method type from request. In string array first is method name and second request.
        /// </summary>
        public string[] CleanRequest(string rawRequest)
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

            string[] Result = { Method, Request };
            return Result;
        }
    }
}
