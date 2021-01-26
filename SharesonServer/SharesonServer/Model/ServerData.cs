using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SharesonServer.Model
{
    public class ServerData
    {
        public class ServerOptions
        {
            public bool WLAN { get; set; }
            public string[] AvailableFoldersModel { get; set; }
            public string LogFile { get; set; }
            public int BufferSize { get; set; }
            public int Port { get; set; }
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
            public byte[] buffer;
            public StringBuilder sb = new StringBuilder();
            public static bool WLAN;

            public class Client
            {
                public Socket socket;
                public bool IsTaskPerform = false;
                public Task Task;
            }
        }
    }
}
