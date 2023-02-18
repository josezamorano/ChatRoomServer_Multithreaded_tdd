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
        private List<ServerUser> _allActiveServerUsers;

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

        private static SemaphoreSlim _semaphoreSlim4 = new SemaphoreSlim(1, 1);
        public void SetAllActiveServerUsers(List<ServerUser> allActiveServerUsers)
        {
            _semaphoreSlim4.Wait();
            try
            {
                _allActiveServerUsers = allActiveServerUsers;
            }
            finally { _semaphoreSlim4.Release(); }            
        }

        private static SemaphoreSlim _semaphoreSlim5 = new SemaphoreSlim(1, 1);
        public List<ServerUser> GetAllActiveServerUsers( )
        {
            _semaphoreSlim5.Wait();
            try
            {
                return _allActiveServerUsers;
            }
            finally { _semaphoreSlim5.Release(); }
        }

        private static SemaphoreSlim _semaphoreSlim6 = new SemaphoreSlim(1, 1);
       

        public string SendMessageServerStopping(TcpClient tcpClient, Guid ServerUserId, string username)
        {
            Payload payloadUsernameError = CreatePayload(MessageActionType.ServerStopped, ServerUserId, username);
            string messageSent = SendMessage(tcpClient, payloadUsernameError);
            return messageSent;
        }

        public void ResolveClientCommunication(TcpClient tcpClient, ServerActivationInfo serverActivationInfo)
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
                        serverActivationInfo.ConnectedClientsCallback(_allConnectedClients.Count);
                    }
                    
                    serverActivationInfo.ServerLoggerCallback(log);
                }
                else if (receivedMessage.Contains(Notification.ClientPayload))
                {
                    ResolveClientPayload(tcpClient, receivedMessage , serverActivationInfo);
                }
            }

            MessageFromClientDelegate messageFromClientCallback = new MessageFromClientDelegate(ProcessMessageFromClientCallback);

            _transmitter.ReceiveMessageFromClient(tcpClient, messageFromClientCallback);


        }

        #region Private Methods
        private void ResolveClientPayload(TcpClient tcpClient, string receivedMessage, ServerActivationInfo serverActivationInfo)
        {
            string payloadMessage = receivedMessage.Replace(Notification.ClientPayload, string.Empty);
            Payload payload = _serializationProvider.DeserializeObject<Payload>(payloadMessage);
            var duplicateServerUser = _allActiveServerUsers.Where(a => a.Username.ToLower() == payload.ClientUsername.ToLower()).FirstOrDefault();
            if (duplicateServerUser != null)
            {
                string messageSentError = SendMessageUsernameTaken(tcpClient, payload.ClientUsername);
                serverActivationInfo.ServerLoggerCallback(messageSentError);
                return;
            }
            ClientActionResolvedReport clientActionResolvedReport = resolveActionRequestedByClient(payload);
            _allActiveServerUsers.Add(clientActionResolvedReport.ServerUser);
            UpdateClientInfoInAllConnectedClients(tcpClient, clientActionResolvedReport);
            string messageSent = SendMessageUserActivated(tcpClient, clientActionResolvedReport.ServerUser.ServerUserID, payload.ClientUsername);
            if (messageSent.Contains(Notification.Exception))
            {
                //remove count 
                ClientInfo disconnectedClient = _allConnectedClients.Where(a => a.tcpClient == tcpClient).FirstOrDefault();
                disconnectedClient.tcpClient.Close();
                _allConnectedClients.Remove(disconnectedClient);
                serverActivationInfo.ConnectedClientsCallback(_allConnectedClients.Count);

            }
            serverActivationInfo.ServerLoggerCallback(messageSent);
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
            Payload createdPayload = new Payload()
            {
                MessageActionType = messageActionType,
                UserGuid = userId,
                ClientUsername = username,
                ActiveServerUsers = _allActiveServerUsers,
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
