using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Services;
using ChatRoomServer.Utils.Enumerations;
using ChatRoomServer.Utils.Interfaces;
using System.Net.Sockets;

namespace ChatRoomServer.DomainLayer
{
    public delegate void MessageFromClientDelegate(string messageFromClient);
    public class ClientAction : IClientAction
    {
        
        //Private Variables
        private List<ClientInfo> _allConnectedClients;

        ISerializationProvider _serializationProvider;
        ITransmitter _transmitter;
        public ClientAction(ISerializationProvider serializationProvider, ITransmitter transmitter)
        {
            _serializationProvider = serializationProvider;
            _transmitter = transmitter;
        }


        public void SetAllConnectedClients(List<ClientInfo> allConnectedClients)
        {
            _allConnectedClients = allConnectedClients;
        }

        private static SemaphoreSlim _semaphoreSlim1 = new SemaphoreSlim(1, 1);
        public List<ClientInfo> GetAllConnectedClients()
        {
            _semaphoreSlim1.Wait();
            try
            {
                return _allConnectedClients;
            }
            finally { _semaphoreSlim1.Release(); }
        }

        private static SemaphoreSlim _semaphoreSlim2 = new SemaphoreSlim(1, 1);
        public void AddNewClientConnectionToAllConnectedClients(TcpClient client)
        {
            _semaphoreSlim2.Wait();
            try
            {
                if (client.Connected)
                {
                    ClientInfo clientInfo = new ClientInfo()
                    {
                        tcpClient = client,
                        Username = string.Empty,
                        ServerUserID = null
                    };
                    _allConnectedClients.Add(clientInfo);
                }                
            }
            finally{ _semaphoreSlim2.Release(); }
            
        }

        private static SemaphoreSlim _semaphoreSlim3 = new SemaphoreSlim(1, 1);
        public void RemoveDisconnectedClientFromAllConnectedClients(ClientInfo disconnectedClient)
        {
            _semaphoreSlim3.Wait();
            try
            {
                disconnectedClient.tcpClient.Close();
                _allConnectedClients.Remove(disconnectedClient);
            }
            finally { _semaphoreSlim3.Release(); }
        }


        #region Messages To Clients
        public string SendMessageServerStopping(TcpClient tcpClient, Guid ServerUserId, string username)
        {
            Payload payloadUsernameError = CreatePayload(MessageActionType.ServerStopped, ServerUserId, username);
            string messageSent = SendMessage(tcpClient, payloadUsernameError);
            return messageSent;
        }

        #endregion Messages To Clients
        public void ResolveClientCommunication(TcpClient tcpClient, ServerActivityInfo serverActivityInfo)
        {              
            void ProcessMessageFromClientCallback(string receivedMessage)
            {
                if (string.IsNullOrEmpty(receivedMessage) || receivedMessage.Contains(Notification.Exception))
                {
                    var log = Notification.CRLF + "Client is disconnected";
                    var disconnectedClient = _allConnectedClients.Where(a => a.tcpClient == tcpClient).FirstOrDefault();
                    if (disconnectedClient != null) 
                    {
                        disconnectedClient.tcpClient.Close();
                        _allConnectedClients.Remove(disconnectedClient);
                        serverActivityInfo.ConnectedClientsCountCallback(_allConnectedClients.Count);
                        serverActivityInfo.ConnectedClientsListCallback(_allConnectedClients);
                    }
                    
                    serverActivityInfo.ServerLoggerCallback(log);
                }
                else if (receivedMessage.Contains(Notification.ClientPayload))
                {
                    ResolveClientPayload(tcpClient, receivedMessage , serverActivityInfo);
                }
            }

            MessageFromClientDelegate messageFromClientCallback = new MessageFromClientDelegate(ProcessMessageFromClientCallback);

            _transmitter.ReceiveMessageFromClient(tcpClient, messageFromClientCallback);


        }

        #region Private Methods
        private void ResolveClientPayload(TcpClient tcpClient, string receivedMessage, ServerActivityInfo serverActivityInfo)
        {
            string payloadMessage = receivedMessage.Replace(Notification.ClientPayload, string.Empty);
            Payload payload = _serializationProvider.DeserializeObject<Payload>(payloadMessage);
            var duplicateServerUser = _allConnectedClients.Where(a =>a.Username.ToLower() == payload.ClientUsername.ToLower()).FirstOrDefault();
            if (duplicateServerUser != null)
            {
                string messageSentError = SendMessageUsernameTaken(tcpClient, payload.ClientUsername);
                serverActivityInfo.ServerLoggerCallback(messageSentError);
                return;
            }
            ClientActionResolvedReport clientActionResolvedReport = resolveActionRequestedByClient(payload);
            UpdateClientInfoInAllConnectedClients(tcpClient, clientActionResolvedReport);
            serverActivityInfo.ConnectedClientsListCallback(_allConnectedClients);
            Guid serverUserID = (Guid)clientActionResolvedReport.ServerUser.ServerUserID;
            string messageSent = SendMessageUserActivated(tcpClient, serverUserID, payload.ClientUsername);
            if (messageSent.Contains(Notification.Exception))
            {
                //remove count 
                ClientInfo disconnectedClient = _allConnectedClients.Where(a => a.tcpClient == tcpClient).FirstOrDefault();
                if(disconnectedClient != null)
                {
                    disconnectedClient.tcpClient.Close();
                    _allConnectedClients.Remove(disconnectedClient);
                }               
                serverActivityInfo.ConnectedClientsCountCallback(_allConnectedClients.Count);
                serverActivityInfo.ConnectedClientsListCallback(_allConnectedClients);
            }
            serverActivityInfo.ServerLoggerCallback(messageSent);
        }

        private string SendMessageUsernameTaken(TcpClient tcpClient, string username)
        {
            Payload payloadUsernameError = CreatePayload(MessageActionType.RetryUsernameTaken, null, username);
            string messageSent = SendMessage(tcpClient, payloadUsernameError);
            return messageSent;
        }

        private string SendMessageUserActivated(TcpClient tcpClient, Guid ServerUserID, string username)
        {
            Payload payloadUsernameOk = CreatePayload(MessageActionType.UserActivated, ServerUserID, username);
            string messageSent = SendMessage(tcpClient, payloadUsernameOk);
            return messageSent;
        }

        private Payload CreatePayload(MessageActionType messageActionType, Guid? userId, string username)
        {
            var serverUsers = _allConnectedClients.Select(a => new { a?.Username, a?.ServerUserID });
            List<ServerUser> allActiveServerUsers = new List<ServerUser>();
            foreach(var user in serverUsers)
            {
                ServerUser serverUser = new ServerUser() 
                { 
                    Username = user.Username,
                    ServerUserID= user.ServerUserID,
                };
                allActiveServerUsers.Add(serverUser);
            }
            Payload createdPayload = new Payload()
            {
                MessageActionType = messageActionType,
                UserGuid = userId,
                ClientUsername = username,
                ActiveServerUsers = allActiveServerUsers,
            };
            return createdPayload;
        }

        private string SendMessage(TcpClient tcpClient,  Payload payload)
        {
            string serializedObject = _serializationProvider.SerializeObject(payload);
            string messageSent = _transmitter.sendMessageToClient(tcpClient, serializedObject);
            return messageSent;
        }

        private void UpdateClientInfoInAllConnectedClients(TcpClient tcpClient, ClientActionResolvedReport clientActionResolvedReport)
        {            
            var selectedClientInfo = _allConnectedClients.Where(a => a.tcpClient == tcpClient).FirstOrDefault();
            if(selectedClientInfo != null)
            {
                selectedClientInfo.ServerUserID = clientActionResolvedReport.ServerUser.ServerUserID;
                selectedClientInfo.Username = clientActionResolvedReport.ServerUser.Username;
            }            
        }
                

        private ClientActionResolvedReport resolveActionRequestedByClient(Payload payload)
        {
            ClientActionResolvedReport clientActionResolvedReport = new ClientActionResolvedReport();
            switch (payload.MessageActionType)
            {
                case MessageActionType.ClientConnectToServer:
                case MessageActionType.CreateUser:
                    ServerUser serverUser = new ServerUser()
                    {
                        Username = payload.ClientUsername,
                        ServerUserID = Guid.NewGuid(),
                    };

                    clientActionResolvedReport.MessageActionType = payload.MessageActionType;
                    clientActionResolvedReport.ServerUser = serverUser;
                    break;
            }

            return clientActionResolvedReport;
        }

        #endregion Private Methods

    }
}
