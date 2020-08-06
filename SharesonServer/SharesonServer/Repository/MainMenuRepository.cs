using Shareson.Support;
using SharesonServer.Model.MainMenu;
using SharesonServer.Repository.SupportFunctions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using static SharesonServer.Model.ServerHelperModel;

namespace SharesonServer.Repository
{
    public class MainMenuRepository 
    {
        Log error = new Log($@"C:\Users\Reneshi\Downloads", "ErrorLogServer.txt");
        ServerHelper server;
        Socket serverSocket;
        List<Client> ConnectedClients;
        public SqlHelper sql;

        int lastConnectedClientsNumber = 0;

        public MainMenuRepository()
        {
            server = new ServerHelper();
            ConnectedClients = new List<Client>();
            sql = new SqlHelper();
        }

        public bool RunServer()
        {
            try
            {
                server.IsServerOn = true;
                serverSocket = server.SetupServer();

                server.AwaitsNewConnections = true;
                server.AwaitsNewRequests = true;

                Task Task_startListening = new Task(() => server.StartListening(serverSocket));
                Task Task_startExecuteRequests = new Task(() => StartExecuteRequests());

                Task_startListening.Start();
                Task_startExecuteRequests.Start();

                return true;
            }
            catch(Exception e)
            {
                error.Add(e.ToString());
                return false;
            }
        }
        public bool StopServer()
        {
            server.AwaitsNewConnections = false;
            server.AwaitsNewRequests = false;
            server.IsServerOn = false;

            return true;
        }
        public void UpdateSQLStatus()
        {
            Task Task_SQLStatus = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5000);
            });
        }

        public int ConnectedUsers()
        {
            int currentConnectedClientsNumber = ConnectedClients.Count; 
            if(lastConnectedClientsNumber != currentConnectedClientsNumber)
            {
                lastConnectedClientsNumber = currentConnectedClientsNumber;
                return lastConnectedClientsNumber;
            }
            else
            {
                return lastConnectedClientsNumber;
            }
        }
        bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }

        private void StartExecuteRequests()
        {
            while (true)
            {
                if (server.ConnectedClients.Count > 0)
                {
                    lock (server.ConnectedClients)
                    {
                        ConnectedClients = server.ConnectedClients;
                        foreach (Client client in server.ConnectedClients)
                        {
                            if (client.IsTaskPerformJob == false)
                            {
                                client.Task = new Task(() =>
                                {
                                    server.Receive(client.socket);
                                });

                                client.Task.Start();
                                client.IsTaskPerformJob = true;
                            }
                        }
                    }
                }
                Thread.Sleep(700);
            }
        }
    }
}
