using Shareson.Support;
using SharesonServer.Model;
using SharesonServer.Model.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static SharesonServer.Model.ServerHelperModel;

namespace SharesonServer.Repository.SupportFunctions
{
    public class ServerHelper
    {
        //public ManualResetEvent ProceedExecuteRequest;
        public List<Client> ConnectedClients;
        public List<FullClientInfoModel> clientInfoModel;
        private RequestsHelper requestHelper;
        private SqlHelper sql;
        private Socket serverSocket;
        private ServerHelperModel Model;

        public bool AwaitsNewConnections = false;
        public bool AwaitsNewRequests = false;
        public bool IsServerOn = false;


        public ServerHelper(SqlHelper sqlInstance = null)
        {
            sql = sqlInstance != null ? sqlInstance: null;
            ConnectedClients = new List<Client>();
            clientInfoModel = new List<FullClientInfoModel>();
            Model = new ServerHelperModel();
            requestHelper = new RequestsHelper(sql);
            IsServerOn = false;

            //ProceedExecuteRequest = new ManualResetEvent(false);
        }

        public Socket SetupServer()
        {
            Socket socket;

            All_Images.TotaltemsInFoldersAnime = System.IO.Directory.GetFiles(@"D:\Nowy folder\Anime");
            All_Images.TotaltemsInFoldersReal = System.IO.Directory.GetFiles(@"D:\Nowy folder\Real");
            All_Images.TotaltemsInFoldersMemy = System.IO.Directory.GetFiles(@"D:\Nowy folder\Memy");

            Model.ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            Model.iPAddress = Model.ipHostInfo.AddressList[0];
            Model.iPEndPoint = new IPEndPoint(Model.iPAddress, 11000);

            socket = new Socket(Model.iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(Model.iPEndPoint);

            socket.Listen(100);

            serverSocket = socket;

            return socket;
        }

        public void DisposeConnection()
        {
            lock(ConnectedClients)
            {
                foreach (Client item in ConnectedClients)
                {
                    item.Task.Dispose();
                    item.socket.Disconnect(false);
                    item.IsTaskPerformJob = false;
                }
            }
            ConnectedClients = new List<Client>();
        }

        public void CloseServerSocket()
        {
            this.serverSocket.Close(0);
        }

        //private bool StartListeningAsync(Socket serverSetup)
        //{
        //    Socket listener;
        //    if (serverSetup != null)
        //    {
        //        listener = serverSetup;
        //    }
        //    else
        //    {
        //        listener = serverSocket;
        //    }
            
        //    try
        //    {
        //        listener.BeginAccept(new AsyncCallback(AcceptCallBack), listener);
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        return false;
        //    }
        //}
        public void StartListening(Socket serverSetup = null)
        {
            Socket listener;
            if (serverSetup != null)
            {
                listener = serverSetup;
            }
            else
            {
                listener = serverSocket;
            }

            while (AwaitsNewConnections)
            {
                try
                {
                    Socket clientSocket = listener.Accept();

                    if (clientSocket != null)
                    {
                        lock (ConnectedClients)
                        {
                            ConnectedClients.Add(new Client
                            {
                                socket = clientSocket,
                            });
                        }
                    }
                }
                catch(Exception e)
                {
                    return;
                }
            }
        }

        public void StopListening()
        {

        }

        private void AcceptCallBack(IAsyncResult AR)
        {
            Socket listener = (Socket)AR.AsyncState;
            Socket handler;
            
            try
            {
                handler = listener.EndAccept(AR);
            }
            catch (SocketException se)
            {
                return;
            }
            lock (ConnectedClients)
            {
                ConnectedClients.Add(new Client
                {
                    socket = handler,
                });
            }
            if (AwaitsNewConnections == true)
            {
                listener.BeginAccept(new AsyncCallback(AcceptCallBack), listener);
            }
        }

        public void Receive(Socket client)
        {
            try
            {
                Socket socket = client;

                socket.BeginReceive(Model.buffer, 0, ServerHelperModel.BufferSize, 0, new AsyncCallback(ReceiveCallBack), socket);
            }
            catch(Exception e)
            {
                InfoLog infoLog = new InfoLog($@"C:\Users\Reneshi\Downloads","ServerReceive.txt");
                infoLog.Add(e.ToString());
            }
            //ProceedExecuteRequest.WaitOne();
        }
        bool IsStillSocketConnected(Socket client)
        {
            bool part1 = client.Poll(2000, SelectMode.SelectRead);
            bool part2 = (client.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }
        private void ReceiveCallBack(IAsyncResult AR)
        {
            string content;
            int contentSize;
            
            Socket client = (Socket)AR.AsyncState;
            if (IsStillSocketConnected(client))
            {
                try
                {
                    contentSize = client.EndReceive(AR);
                    if (contentSize > 0)
                    {
                        Model.sb = new StringBuilder(Encoding.ASCII.GetString(Model.buffer, 0, contentSize));
                        content = Model.sb.ToString();
                        if (content.IndexOf("<EOS>") > -1)
                        {
                            content = content.Remove(content.IndexOf("<EOS>"), "<EOS>".Length);

                            if(AwaitsNewRequests == true)
                            {
                                client.BeginReceive(Model.buffer, 0, ServerHelperModel.BufferSize, SocketFlags.None, ReceiveCallBack, client);
                            }
                            else
                            {
                                DisconnectSocket(client);
                                return;
                            }
                        }
                        else
                        {
                            // Not all data received. Get more.  
                            if(AwaitsNewRequests == true)
                            {
                                client.BeginReceive(Model.buffer, 0, ServerHelperModel.BufferSize, 0, new AsyncCallback(ReceiveCallBack), client);
                            }
                            else
                            {
                                DisconnectSocket(client);
                                return;
                            }
                        }
                        //ProceedExecuteRequest.Set(); /*ProceedExecuteRequest.Reset();*/
                       
                        ExecuteMethod(content, client);
                    }
                }
                catch (SocketException se)
                {
                    DisconnectSocket(client);

                    return;
                }
            }
            else
            {
                DisconnectSocket(client);
                return;
            }
        }

        public void Send(Socket clientSocket, byte[] data = null)
        {
            if (data != null)
            {
                clientSocket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallBack), clientSocket);
                //ProceedExecuteRequest.WaitOne();
            }
        }
        private void SendCallBack(IAsyncResult asyncResult)
        {
            try
            {
                Socket socket = (Socket)asyncResult.AsyncState;

                int sendedBytes = socket.EndSend(asyncResult);
            }
            catch(Exception e)
            {
            }
        }

        public void DisconnectSocket(Socket user)
        {
            lock (ConnectedClients)
            {
                lock(clientInfoModel)
                {
                    clientInfoModel.Remove(clientInfoModel.Where(f => string.Equals(user.RemoteEndPoint.ToString(), f.IP)).FirstOrDefault());
                }
                user.Close();
                ConnectedClients.Remove(ConnectedClients.Where(f => f.socket == user).FirstOrDefault());
            }
        }

        private void ExecuteMethod(string content, Socket client)
        {
            var separatedRequest = requestHelper.SpreadRequest(content);
            var MethodType = requestHelper.GetServerMethod(separatedRequest[0]);

            switch (MethodType)
            {
                case Enum.ServerMethods.GetImage:
                    {
                        Send(client, requestHelper.GetImage(separatedRequest[1]));
                        break;
                    }
                case Enum.ServerMethods.PutImage:
                    {
                        break;
                    }
                case Enum.ServerMethods.GetRandomImage:
                    {
                        Send(client, requestHelper.GetRandomImage(separatedRequest[1]));
                        break;
                    }
                case Enum.ServerMethods.GetImageInfo:
                    {
                        break;
                    }
                case Enum.ServerMethods.GetImagesList:
                    {
                        break;
                    }
                case Enum.ServerMethods.LoginToAccount:
                    {
                        lock(clientInfoModel)
                        {
                            Send(client, requestHelper.LoginToAccount(separatedRequest[1], client, ref clientInfoModel));
                        }
                        break;
                    }
                case Enum.ServerMethods.GetAccountInfo:
                    {
                        //Send(client, requestHelper.GetImageInfo(separatedRequest[1]));
                        break;
                    }
                case Enum.ServerMethods.Ping:
                    {
                        break;
                    }
                case Enum.ServerMethods.CreateAccount:
                    {
                        requestHelper.CreateAccount(separatedRequest[1]);
                        Send(client, requestHelper.MessageAsBytes("AccountCreated"));
                        break;
                    }
                //case Enum.ServerMethods.Leave:
                //    {
                //        requestHelper.ClientLeave(ref ConnectedClients, client);
                //        break;
                //    }
                case Enum.ServerMethods.IsServerOn:
                    {

                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
}