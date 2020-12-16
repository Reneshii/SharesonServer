using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SharesonServer.Model
{
    public class ServerData
    {
        public static class ServerOptions
        {
            public static bool WLAN { get; set; }
            public static string[] AvailableFoldersModel { get; set; }
            public static string LogFile { get; set; }
            public static int BufferSize = 10000; //AddToSettings
        }

        public class TemporaryResources
        {
            public string Directory { get; set; }
            public string[] FilesFoundInSharedFolders { get; set; }

            public object Clone()
            {
                return new TemporaryResources
                {
                    Directory = this.Directory,
                    FilesFoundInSharedFolders = this.FilesFoundInSharedFolders
                };
            }
        }

        public class ServerHelperModel
        {
            public IPHostEntry ipHostInfo;
            public IPAddress iPAddress;
            public IPEndPoint iPEndPoint;

            public string receivedData;
            public byte[] buffer = new byte[ServerOptions.BufferSize];
            public StringBuilder sb = new StringBuilder();
            public static bool InternetMode;

            public class Client
            {
                public Socket socket;
                public bool IsTaskPerform = false;
                public Task Task;
            }
        }
    }
}
