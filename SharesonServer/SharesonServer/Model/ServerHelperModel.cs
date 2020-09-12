using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SharesonServer.Model
{
    public class ServerHelperModel
    {
        public IPHostEntry ipHostInfo;
        public IPAddress iPAddress;
        public IPEndPoint iPEndPoint;

        public string receivedData;
        public const int BufferSize = 10000;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();

        public class Client
        {
            public Socket socket;
            public bool IsTaskPerform = false;
            public Task Task;
        }
    }
}
