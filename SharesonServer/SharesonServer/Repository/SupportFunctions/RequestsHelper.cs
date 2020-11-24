using Newtonsoft.Json;
using SharesonServer.Enum;
using SharesonServer.Model.Support;
using SharesonServer.Model.Support.SQL;
using System;

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

        private AccountModelForShareson DeserializeAccountServiceRequest(string request)
        {
            AccountModelForShareson accountModel = new AccountModelForShareson();
            accountModel = JsonConvert.DeserializeObject<AccountModelForShareson>(request);

            return accountModel;
        }

        private ImagesRequestModel DeserializeImagesRequest(string request)
        {
            ImagesRequestModel model = new ImagesRequestModel();
            model = JsonConvert.DeserializeObject<ImagesRequestModel>(request);

            return model;
        }

        public byte[] GetImage(string request, AccountModelForShareson account = null, bool UseSQL = false)
        {
            byte[] result;

            if (UseSQL)
            {
                if(sql.CheckIfUserIsLogedIn(account.ID, account.Email))
                {
                    ImageOptions convert = new ImageOptions();
                    var model = DeserializeImagesRequest(request);
                    result = convert.ReturnImageWithInfoAsBytes(model.PathToDirectory, model.FileName, model.ExcludedExtensions);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                ImageOptions convert = new ImageOptions();
                var model = DeserializeImagesRequest(request);
                result = convert.ReturnImageWithInfoAsBytes(model.PathToDirectory, model.FileName, model.ExcludedExtensions);
            }

                return result;
        }
        public byte[] GetRandomImage(string request, AccountModelForShareson account = null, bool UseSQL = false)
        {
            byte[] result;
            if (UseSQL)
            {
                if(sql.CheckIfUserIsLogedIn(account.ID, account.Email))
                {
                    ImageOptions convert = new ImageOptions();
                    var model = DeserializeImagesRequest(request);
                    result = convert.ReturnImageWithInfoAsBytes(model.PathToDirectory, All_Images.GetRandom(model.PathToDirectory), model.ExcludedExtensions);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                ImageOptions convert = new ImageOptions();
                var model = DeserializeImagesRequest(request);
                result = convert.ReturnImageWithInfoAsBytes(model.PathToDirectory, All_Images.GetRandom(model.PathToDirectory), model.ExcludedExtensions);
            }
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

        public byte[] LoginToAccount(string request, bool IsSQLOn = true)
        {
            try
            {
                AccountModelForShareson model = new AccountModelForShareson();
                
                if(IsSQLOn)
                {
                    var data = DeserializeAccountServiceRequest(request);

                    sql.LogInToUserAccount(data.Email, data.Password);
                    model = sql.GetUserInfo(data.Email, data.Password);
                }
                else
                {
                    System.Collections.Generic.List<string> AD = new System.Collections.Generic.List<string>();
                    foreach (var item in All_Images.ImagesData)
                    {
                        AD.Add(item.Directory);
                    }
                    model = new AccountModelForShareson()
                    {
                        AccessedDirectory = AD.ToArray(),
                    };
                    
                }
                string json = JsonConvert.SerializeObject(model);
                var result = MessageAsBytes(json);

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
        /// Separate method type from request. In string array first is method name and second request and third is sql model.
        /// </summary>
        public string[] SpreadRequest(string rawRequest)
        {
            string Method;
            string Request = rawRequest;
            string Acc = string.Empty;

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

            if(Request.Contains("<Req>"))
            {
                string removed = Request.Remove(Request.IndexOf("<Req>"));
                int startAt = Request.IndexOf(removed);
                int EndAt = "<Req>".Length + removed.Length;
                Acc = Request.Remove(startAt, EndAt);
                Request = Request.Remove(Request.IndexOf("<Req>"));
            }

            string[] Result = { Method, Request };
            
            if(! string.IsNullOrEmpty(Acc))
            {
                string[] add = { Method, Request, Acc };
                Result = add;
            }
            return Result;
        }
    }
}
