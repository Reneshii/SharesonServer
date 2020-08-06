using SharesonServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static SharesonServer.Model.ServerHelperModel;

namespace SharesonServer.Repository.SupportFunctions
{
    public class ServerHelper
    {
        private ServerHelperModel Model;
        public ManualResetEvent ProceedExecuteRequest;
        public List<Client> ConnectedClients;
        RequestsHelper requestHelper;
        SqlHelper sql;

        public bool AwaitsNewConnections = false;
        public bool AwaitsNewRequests = false;
        public bool IsServerOn = false;


        public ServerHelper()
        {
            sql = new SqlHelper();
            ConnectedClients = new List<Client>();
            Model = new ServerHelperModel();
            requestHelper = new RequestsHelper(sql);
            IsServerOn = false;

            //ProceedExecuteRequest = new ManualResetEvent(false);
        }

        public Socket SetupServer()
        {
            All_Images.TotaltemsInFoldersAnime = System.IO.Directory.GetFiles(@"D:\Nowy folder\Anime");
            All_Images.TotaltemsInFoldersReal = System.IO.Directory.GetFiles(@"D:\Nowy folder\Real");
            All_Images.TotaltemsInFoldersMemy = System.IO.Directory.GetFiles(@"D:\Nowy folder\Memy");

            Model.ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            Model.iPAddress = Model.ipHostInfo.AddressList[0];
            Model.iPEndPoint = new IPEndPoint(Model.iPAddress, 11000);

            Socket socket = new Socket(Model.iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(Model.iPEndPoint);
            socket.Listen(100);

            return socket;
        }

        public bool StartListening(Socket serverSetup)
        {
            Socket listener = serverSetup;

            try
            {
                listener.BeginAccept(new AsyncCallback(AcceptCallBack), listener);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
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
                listener.BeginAccept(new AsyncCallback(AcceptCallBack), listener);
                return;
            }

            lock (ConnectedClients)
            {
                ConnectedClients.Add(new Client { socket = handler });
            }
            if (AwaitsNewConnections == true)
            {
                listener.BeginAccept(new AsyncCallback(AcceptCallBack), listener);
            }
        }

        public void Receive(Socket client)
        {
            Socket socket = client;

            socket.BeginReceive(Model.buffer, 0, ServerHelperModel.BufferSize, 0, new AsyncCallback(ReceiveCallBack), socket);
            //ProceedExecuteRequest.WaitOne();
        }
        bool SocketConnected(Socket client)
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
            if (SocketConnected(client))
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
                                KickClient(client);
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
                                KickClient(client);
                                return;
                            }
                        }
                        //ProceedExecuteRequest.Set(); /*ProceedExecuteRequest.Reset();*/
                       
                        ExecuteMethod(content, client);
                    }
                }
                catch (SocketException se)
                {
                    client.Close();
                    var toRemove = ConnectedClients.Find(h => h.socket == client);
                    ConnectedClients.Remove(toRemove);
                    return;
                }
            }
            else
            {
                ConnectedClients.Remove(ConnectedClients.Where(f => f.socket == client).FirstOrDefault());
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

        private void KickClient(Socket client)
        {
            client.Shutdown(SocketShutdown.Both);
            client.Dispose();
            client.Disconnect(false);
        }

        private void ExecuteMethod(string content, Socket client)
        {
            var separatedRequest = requestHelper.CleanRequest(content);
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
                        Send(client, requestHelper.LoginToAccount(separatedRequest[1]));
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