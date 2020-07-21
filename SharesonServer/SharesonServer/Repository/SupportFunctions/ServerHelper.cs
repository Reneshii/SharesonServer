using SharesonServer.Model;
using System;
using System.Collections.Generic;
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
        RequestsReader request;

        public string content;

        public ServerHelper()
        {
            ProceedExecuteRequest = new ManualResetEvent(false);
            ConnectedClients = new List<Client>();
            Model = new ServerHelperModel();
            request = new RequestsReader();
            content = string.Empty;
        }

        public Socket SetupServer()
        {
            TotalImages.TotaltemsInFoldersAnime = System.IO.Directory.GetFiles(@"D:\Nowy folder\Anime");
            TotalImages.TotaltemsInFoldersReal = System.IO.Directory.GetFiles(@"D:\Nowy folder\Real");
            TotalImages.TotaltemsInFoldersMemy = System.IO.Directory.GetFiles(@"D:\Nowy folder\Memy");

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
            listener.BeginAccept(new AsyncCallback(AcceptCallBack), listener);
        }

        public void Receive(Socket client)
        {
            Socket socket = client;

            socket.BeginReceive(Model.buffer, 0, ServerHelperModel.BufferSize, 0, new AsyncCallback(ReceiveCallBack), socket);
            //ProceedExecuteRequest.WaitOne();
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
        private void ReceiveCallBack(IAsyncResult AR)
        {
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
                            //int startAt = content.IndexOf("<EOS>");
                            //int endAt = "<EOS>".Length;
                            content = content.Remove(content.IndexOf("<EOS>"), "<EOS>".Length);

                            if (Model.dataReadyToSend != null)
                            {
                                Send(client, Model.dataReadyToSend);
                                Model.dataReadyToSend = null;
                            }

                            client.BeginReceive(Model.buffer, 0, ServerHelperModel.BufferSize, SocketFlags.None, ReceiveCallBack, client);
                        }
                        else
                        {
                            // Not all data received. Get more.  
                            client.BeginReceive(Model.buffer, 0, ServerHelperModel.BufferSize, 0,
                                new AsyncCallback(ReceiveCallBack), client);
                        }
                        //ProceedExecuteRequest.Set(); /*ProceedExecuteRequest.Reset();*/

                        Send(client, request.ConvertRequest(content));
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
                //ConnectedClients.Remove(ConnectedClients.Where());
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
                //Log.Add("File size: " + sendedBytes.ToString() + "KB");

                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();
                //ProceedExecuteRequest.Set();
                //ProceedExecuteRequest.Reset();
            }
            catch(Exception e)
            {
            }

        }
    }
}