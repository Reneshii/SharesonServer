using SharesonServer.Interface;
using SharesonServer.Model.Support;
using SharesonServer.Repository.SupportFunctions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static SharesonServer.Model.ServerHelperModel;

namespace SharesonServer.Repository
{
    public class StartMainMenuRepository : IMainMenuRepositoryFunctionsWithoutLimit, IMainMenuRepositoryFunctionsWithLimit
    {
        public SqlHelper sql { get; set; }
        private ServerHelper server;
        private Task Task_startListening;
        private Task Task_startExecuteRequests;
        private int lastConnectedClientsNumber = 0;
        public bool RepeatCountConnectedClients_Task { get; set; }

        public StartMainMenuRepository()
        {
            sql = new SqlHelper();
            server = new ServerHelper(sql);
        }

        private void InitializeTasks()
        {
            Task_startListening = new Task(() =>
            {
                server.StartListening();
            });

            Task_startExecuteRequests = new Task(() => 
            {
                StartExecuteRequests();
            });
        }

        private bool InitializeServer()
        {
            InitializeTasks();
            server.IsServerOn = true;
            server.AwaitsNewConnections = true;
            server.AwaitsNewRequests = true;
            RepeatCountConnectedClients_Task = true;

            server.SetupServer();

            Task_startListening.Start();
            Task_startExecuteRequests.Start();
            return true;
        }

        public bool RunServer()
        {
            return InitializeServer();
        }

        public bool RunServerWithLimitedFunctions()
        {
            return InitializeServer();
        }

        public bool StopServer()
        {
            server.AwaitsNewConnections = false;
            server.AwaitsNewRequests = false;
            server.IsServerOn = false;
            RepeatCountConnectedClients_Task = false;
            server.DisposeConnection();
            server.CloseServerSocket();

            return true;
        }
        //public void UpdateSQL()
        //{
        //    Task Task_SQLStatus = Task.Factory.StartNew(() =>
        //    {
        //        Thread.Sleep(5000);
        //    });
        //}
        private void StartExecuteRequests()
        {
            while (true)
            {
                if (server.ConnectedClients.Count > 0)
                {
                    lock (server.ConnectedClients)
                    {
                        foreach (Client client in server.ConnectedClients)
                        {
                            if (client.IsTaskPerform == false)
                            {
                                client.Task = new Task(() =>
                                {
                                    server.Receive(client.socket);
                                });

                                client.Task.Start();
                                client.IsTaskPerform = true;
                            }
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }

        public int ConnectedUsers()
        {
            int currentConnectedClientsNumber = server.ConnectedClients.Count; 
            if (lastConnectedClientsNumber != currentConnectedClientsNumber)
            {
                return currentConnectedClientsNumber;
            }
            else
            {
                return lastConnectedClientsNumber;
            }
        }

        public List<FullClientInfoModel> UpdateUsersList()
        {
            return server.clientInfoModel;
        }
    }
}
