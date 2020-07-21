using Shareson.Support;
using SharesonServer.Repository.SupportFunctions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
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

        public MainMenuRepository()
        {
            server = new ServerHelper();
            ConnectedClients = new List<Client>();
        }

        public bool RunServer()
        {
            try
            {
                serverSocket = server.SetupServer();
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
                System.Threading.Thread.Sleep(700);
            }
        }
    }
}
