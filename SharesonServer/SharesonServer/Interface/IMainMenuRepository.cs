using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharesonServer.Repository
{
    public interface IMainMenuRepository
    {
        void SetupServer();
        void AcceptCallBack(IAsyncResult AR);
        void ReceiveCallBack(IAsyncResult AR);
        void CloseAllSockets();
    }
}
