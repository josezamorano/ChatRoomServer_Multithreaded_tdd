using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Services;
using ChatRoomServer.Utils.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace ChatRoomServer.DomainLayer
{
    public delegate void MessageFromClientDelegate(string messageFromClient);
    public class ServerManager : IServerManager
    {
        //Private Variables
        private bool _serverIsActive;        
        private TcpListener _tcpListener;
        private string _serverStatusLogger;
        private List<ClientInfo> _allConnectedClients;
        private List<ServerUser> _allActiveServerUsers;

        IClientAction _clientAction;
        public ServerManager(IClientAction clientAction)
        {
            _serverIsActive = false;
            _allConnectedClients = new List<ClientInfo>();
            _allActiveServerUsers = new List<ServerUser>();           
            _clientAction = clientAction;            
        }

        private void SeedTESTAllActiveServerUsers()
        {

            ServerUser serverUserTest = new ServerUser()
            {
                Username = "abc",
                ServerUserID = Guid.NewGuid(),
            };
            _allActiveServerUsers.Add(serverUserTest);
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


        public void StartServer(ServerActivationInfo serverActivationInfo)            
        {
            try
            {                                
                _allConnectedClients.Clear();
                _allActiveServerUsers.Clear();
                SeedTESTAllActiveServerUsers();

                _serverStatusLogger = Notification.CRLF + "Starting Server...";
                serverActivationInfo.ServerLoggerCallback(_serverStatusLogger);
                Thread threadListener = new Thread(() => {
                    ListenForIncomingConnections(serverActivationInfo);
                });
                threadListener.Name = "ThreadServerListener";
                threadListener.IsBackground = true;
                threadListener.Start();
            }
            catch(Exception ex)
            {
                _serverIsActive = false;
                serverActivationInfo.ServerStatusCallback(_serverIsActive);
                _serverStatusLogger = Notification.CRLF + Notification.Exception + "Failure attempting to start the server" + Notification.CRLF + ex.ToString();
                serverActivationInfo.ServerLoggerCallback(_serverStatusLogger);
            }            
        }


        public void StopServer(ServerLoggerDelegate serverLoggerCallback, ServerStatusDelegate serverStatusCallback) 
        { 
            _serverIsActive= false;
            serverStatusCallback(_serverIsActive);
            _serverStatusLogger = Notification.CRLF + "Shutting down Server, disconnecting all clients...";
            serverLoggerCallback(_serverStatusLogger);
            try
            {
                foreach (ClientInfo clientInfo in _allConnectedClients)
                {
                    clientInfo.tcpClient.Close();
                }
                _allConnectedClients.Clear();
                _tcpListener.Stop();
                _allActiveServerUsers.Clear();
                _serverStatusLogger = Notification.CRLF + "Server Stopped Successfully.";               
                serverLoggerCallback(_serverStatusLogger);
            }
            catch(Exception ex) 
            {
                _serverStatusLogger = Notification.CRLF + Notification.Exception + "Problem stopping the server, or client connections forcibly closed..." + Notification.CRLF + ex.ToString();
                serverLoggerCallback(_serverStatusLogger);
            }
        }


        #region Private Methods 
        private void ListenForIncomingConnections(ServerActivationInfo serverActivationInfo)            
        {
            try
            { 
                _serverIsActive = true;
                serverActivationInfo.ServerStatusCallback(_serverIsActive);
                _tcpListener = new TcpListener(IPAddress.Any, serverActivationInfo.Port);
                _tcpListener.Start();
                _serverStatusLogger = Notification.CRLF + "Server started. Listening on port: " + serverActivationInfo.Port;               
                serverActivationInfo.ServerLoggerCallback(_serverStatusLogger);
                while (_serverIsActive)
                {
                    _serverStatusLogger = Notification.CRLF + "Waiting for incoming client connection...";                    
                    serverActivationInfo.ServerLoggerCallback(_serverStatusLogger);
                    TcpClient tcpClient = _tcpListener.AcceptTcpClient();   // blocks here until client connects
                    _serverStatusLogger = Notification.CRLF + "Incoming client connection accepted...";
                    serverActivationInfo.ServerLoggerCallback(_serverStatusLogger);
                   
                    Thread threadWorkerClient = new Thread(() => 
                    {
                        _clientAction.SetAllConnectedClients(_allConnectedClients);
                        _clientAction.SetAllActiveServerUsers(_allActiveServerUsers);
                        _clientAction.AddNewClientConnectionToAllConnectedClients(tcpClient);
                        serverActivationInfo.ConnectedClientsCallback(_allConnectedClients.Count());
                        _clientAction. ResolveClientCommunication(tcpClient, serverActivationInfo);
                    });
                    threadWorkerClient.IsBackground = true;
                    threadWorkerClient.Name = "threadWorkerTcpClient_" + _allConnectedClients.Count();
                    threadWorkerClient.Start();
                                
                }
            }
            catch (SocketException se)
            {   _tcpListener.Stop();
                _serverStatusLogger = Notification.CRLF + "Problem starting the server." + Notification.CRLF + se.ToString();
                serverActivationInfo.ServerLoggerCallback(_serverStatusLogger);
            }
            catch (Exception ex)
            {
                _tcpListener.Stop();
                _serverStatusLogger = Notification.CRLF + "Problem starting the server." + Notification.CRLF + ex.ToString();
                serverActivationInfo.ServerLoggerCallback(_serverStatusLogger);
            }

            _serverStatusLogger = Notification.CRLF + "Exiting listener thread...";
            serverActivationInfo.ServerLoggerCallback(_serverStatusLogger);
        }

        #endregion Private Methods
    }
}
