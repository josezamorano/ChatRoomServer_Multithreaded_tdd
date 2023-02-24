using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Services;
using ChatRoomServer.Utils.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace ChatRoomServer.DomainLayer
{
   
    public class ServerManager : IServerManager
    {
        //Private Variables
        private bool _serverIsActive;        
        private TcpListener _tcpListener;
        private string _serverStatusLogger;
        private List<ClientInfo> _allConnectedClients;

        IClientAction _clientAction;
        IMessageDispatcher _messageDispatcher;
        public ServerManager(IClientAction clientAction, IMessageDispatcher messageDispatcher)
        {
            _serverIsActive = false;
            _allConnectedClients = new List<ClientInfo>();      
            _clientAction = clientAction;
            _messageDispatcher = messageDispatcher;
            _clientAction.SetAllConnectedClients(_allConnectedClients);
        }

        private void SeedTESTAllActiveServerUsers()
        {

            ClientInfo clientInfoTest = new ClientInfo()
            {
                TcpClient = null,
                Username = "abc",
                ServerUserID = Guid.NewGuid(),
            };
             _allConnectedClients.Add(clientInfoTest);
        }

        public string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return string.Empty;
        }


        public void StartServer(ServerActivityInfo serverActivityInfo)            
        {
            try
            {                                
                _allConnectedClients.Clear();
                SeedTESTAllActiveServerUsers();

                _serverStatusLogger = Notification.CRLF + "Starting Server...";
                serverActivityInfo.ServerLoggerCallback(_serverStatusLogger);
                Thread threadListener = new Thread(() => {
                    ListenForIncomingConnections(serverActivityInfo);
                });
                threadListener.Name = "ThreadServerListener";
                threadListener.IsBackground = true;
                threadListener.Start();
            }
            catch(Exception ex)
            {
                _serverIsActive = false;
                serverActivityInfo.ServerStatusCallback(_serverIsActive);
                _serverStatusLogger = Notification.CRLF + Notification.Exception + "Failure attempting to start the server" + Notification.CRLF + ex.ToString();
                serverActivityInfo.ServerLoggerCallback(_serverStatusLogger);
            }            
        }


        public void StopServer(ServerActivityInfo serverActivityInfo) 
        { 
            _serverIsActive= false;
            serverActivityInfo.ServerStatusCallback(_serverIsActive);
            _serverStatusLogger = Notification.CRLF + "Shutting down Server, disconnecting all clients...";
            serverActivityInfo.ServerLoggerCallback(_serverStatusLogger);
            try
            {
                foreach (ClientInfo clientInfo in _allConnectedClients)
                {
                    Guid serverUserId = (Guid)clientInfo.ServerUserID;
                    string messageSent = _messageDispatcher.SendMessageServerStopping(_allConnectedClients ,clientInfo.TcpClient, serverUserId, clientInfo.Username);
                    clientInfo?.TcpClient?.Close();
                }
               
                _tcpListener.Stop();
                _allConnectedClients.Clear();
                serverActivityInfo.ConnectedClientsCountCallback(_allConnectedClients.Count);
                serverActivityInfo.ConnectedClientsListCallback(_allConnectedClients);
                _serverStatusLogger = Notification.CRLF + "Server Stopped Successfully.";               
                serverActivityInfo.ServerLoggerCallback(_serverStatusLogger);
            }
            catch(Exception ex) 
            {
                _serverStatusLogger = Notification.CRLF + Notification.Exception + "Problem stopping the server, or client connections forcibly closed..." + Notification.CRLF + ex.ToString();
                serverActivityInfo.ServerLoggerCallback(_serverStatusLogger);
            }
        }


        #region Private Methods 
        private void ListenForIncomingConnections(ServerActivityInfo serverActivityInfo)            
        {
            try
            { 
                _serverIsActive = true;
                serverActivityInfo.ServerStatusCallback(_serverIsActive);
                _tcpListener = new TcpListener(IPAddress.Any, serverActivityInfo.Port);
                _tcpListener.Start();
                _serverStatusLogger = Notification.CRLF + "Server started. Listening on port: " + serverActivityInfo.Port;               
                serverActivityInfo.ServerLoggerCallback(_serverStatusLogger);
                while (_serverIsActive)
                {
                    _serverStatusLogger = Notification.CRLF + "Waiting for incoming client connection...";                    
                    serverActivityInfo.ServerLoggerCallback(_serverStatusLogger);
                    TcpClient tcpClient = _tcpListener.AcceptTcpClient();   // blocks here until client connects
                    _serverStatusLogger = Notification.CRLF + "Incoming client connection accepted...";
                    serverActivityInfo.ServerLoggerCallback(_serverStatusLogger);
                   
                    Thread threadWorkerClient = new Thread(() => 
                    {                        
                        _clientAction.AddNewClientConnectionToAllConnectedClients(tcpClient);
                        serverActivityInfo.ConnectedClientsCountCallback(_allConnectedClients.Count());   
                        serverActivityInfo.ConnectedClientsListCallback(_allConnectedClients);
                        _clientAction. ResolveCommunicationFromClient(tcpClient, serverActivityInfo);
                    });
                    threadWorkerClient.IsBackground = true;
                    threadWorkerClient.Name = "threadWorkerTcpClient_" + _allConnectedClients.Count();
                    threadWorkerClient.Start();                                
                }
            }
            catch (SocketException se)
            {   _tcpListener.Stop();
                _serverStatusLogger = Notification.CRLF + "Problem starting the server." + Notification.CRLF + se.ToString();
                serverActivityInfo.ServerLoggerCallback(_serverStatusLogger);
            }
            catch (Exception ex)
            {
                _tcpListener.Stop();
                _serverStatusLogger = Notification.CRLF + "Problem starting the server." + Notification.CRLF + ex.ToString();
                serverActivityInfo.ServerLoggerCallback(_serverStatusLogger);
            }

            _serverStatusLogger = Notification.CRLF + "Exiting listener thread...";
            serverActivityInfo.ServerLoggerCallback(_serverStatusLogger);
        }

        #endregion Private Methods
    }
}
