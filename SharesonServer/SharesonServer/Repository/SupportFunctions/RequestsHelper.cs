using Newtonsoft.Json;
using SharesonServer.Enum;
using SharesonServer.Model.Support;
using System;
using System.Net.Sockets;

namespace SharesonServer.Repository.SupportFunctions
{
    public class RequestsHelper
    {
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

        private AccountModel DeserializeAccountServiceRequest(string request)
        {
            AccountModel accountModel = new AccountModel();
            accountModel = JsonConvert.DeserializeObject<AccountModel>(request);

            return accountModel;
        }

        private ImagesRequestModel DeserializeImagesRequest(string request)
        {
            ImagesRequestModel model = new ImagesRequestModel();
            model = JsonConvert.DeserializeObject<ImagesRequestModel>(request);

            return model;
        }

        public byte[] GetImage(string request)
        {
            ImageOptions convert = new ImageOptions();

            var model = DeserializeImagesRequest(request);
            var result = convert.GetImageWithInfoAsBytes(model.PathToDirectory, model.FileName, model.ExcludedExtensions);
            return result;
        }
        public byte[] GetRandomImage(string request)
        {
            ImageOptions convert = new ImageOptions();
            var model = DeserializeImagesRequest(request);
            var result = convert.GetImageWithInfoAsBytes(model.PathToDirectory, All_Images.GetRandom(model.PathToDirectory), model.ExcludedExtensions);
            return result;
        }

        public void PutImage()
        {

        }

        public void GetImagesList()
        {

        }

        public byte[] MessageAsBytes(string text)
        {
            var toReturn = System.Text.Encoding.ASCII.GetBytes(text);
            return toReturn;
        }

        public byte[] LoginToAccount(string request, Socket client, ref System.Collections.Generic.List<FullClientInfoModel> model)
        {
            try
            {
                var data = DeserializeAccountServiceRequest(request);
                sql.LogInToUserAccount(data.Email, data.Password);
                var dataToSend = sql.GetUserInfo(data.Email, data.Password);
                string json = JsonConvert.SerializeObject(dataToSend);
                var result = MessageAsBytes(json);

                model.Add(new FullClientInfoModel()
                {
                    Email = data.Email,
                    Login = data.Login,
                    Name = data.Name,
                    ID = dataToSend.ID,
                    IP = client.RemoteEndPoint.ToString(),

                });
                return result;
            }
            catch(Exception e)
            {
                return null;
            }
            
        }

        public string CreateAccount(string request)
        {
            var data = DeserializeAccountServiceRequest(request);
            var result = sql.CreateAccount(data, true);
            return result;            
        }
        public byte[] MethodDisabledMessage()
        {
            string info = "This method is disabled by server";
            return System.Text.Encoding.ASCII.GetBytes(info);
        }

        /// <summary>
        /// Separate method type from request. In string array first is method name and second request.
        /// </summary>
        public string[] SpreadRequest(string rawRequest)
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
