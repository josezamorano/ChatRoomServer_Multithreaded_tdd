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
        IMessageDispatcher _messageDispatcher;
        public ClientAction(ISerializationProvider serializationProvider, ITransmitter transmitter, IMessageDispatcher messageDisptcher)
        {
            _serializationProvider = serializationProvider;
            _transmitter = transmitter;
            _messageDispatcher = messageDisptcher;
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


        public void ResolveCommunicationFromClient(TcpClient tcpClient, ServerActivityInfo serverActivityInfo)
        {              
            void ProcessMessageFromClientCallback(string receivedMessage)
            {
                var messageIsFailed = VerifyIfMessageIsNullOrContainsException(receivedMessage, tcpClient, serverActivityInfo);
                if(messageIsFailed) { return; }

                if (receivedMessage.Contains(Notification.ClientPayload))
                {                    
                    ResolveActionRequestedByClient(receivedMessage, tcpClient, serverActivityInfo);
                }
            }

            MessageFromClientDelegate messageFromClientCallback = new MessageFromClientDelegate(ProcessMessageFromClientCallback);

            _transmitter.ReceiveMessageFromClient(tcpClient, messageFromClientCallback);
        }

        #region Private Methods 

        private bool VerifyIfMessageIsNullOrContainsException(string message, TcpClient tcpClient, ServerActivityInfo serverActivityInfo)
        {
            if (string.IsNullOrEmpty(message) || message.Contains(Notification.Exception))
            {
                var disconnectedClient = _allConnectedClients.Where(a => a.tcpClient == tcpClient).FirstOrDefault();
                CloseDisconnectedClient(disconnectedClient, serverActivityInfo);
                return true;
            }
            serverActivityInfo.ServerLoggerCallback(message);
            return false;
        }


        private void CloseDisconnectedClient(ClientInfo disconnectedClient, ServerActivityInfo serverActivityInfo)
        {
            if (disconnectedClient != null)
            {
                disconnectedClient.tcpClient.Close();
                _allConnectedClients.Remove(disconnectedClient);

                var log = Notification.CRLF + "Client is disconnected";
                serverActivityInfo.ServerLoggerCallback(log);
            }
            serverActivityInfo.ConnectedClientsCountCallback(_allConnectedClients.Count);
            serverActivityInfo.ConnectedClientsListCallback(_allConnectedClients);
        }



        private void ResolveActionRequestedByClient(string receivedMessage,  TcpClient tcpClient , ServerActivityInfo serverActivityInfo)
        {
            string payloadMessage = receivedMessage.Replace(Notification.ClientPayload, string.Empty);
            Payload payload = _serializationProvider.DeserializeObject<Payload>(payloadMessage);
                       
            switch (payload.MessageActionType)
            {
                case MessageActionType.ClientConnectToServer:
                case MessageActionType.CreateUser:

                    var duplicateServerUser = _allConnectedClients.Where(a => a.Username.ToLower() == payload.ClientUsername.ToLower()).FirstOrDefault();
                    if (duplicateServerUser != null)
                    {
                        string messageSentError = _messageDispatcher.SendMessageUsernameTaken(_allConnectedClients, tcpClient, payload.ClientUsername);
                        VerifyIfMessageIsNullOrContainsException(messageSentError, tcpClient, serverActivityInfo);
                    }
                    else
                    {
                        ServerUser serverUser = new ServerUser()
                        {
                            Username = payload.ClientUsername,
                            ServerUserID = Guid.NewGuid(),
                        };
                        string messageSent = _messageDispatcher.SendMessageUserActivated(_allConnectedClients, (Guid)serverUser.ServerUserID, payload.ClientUsername);
                        VerifyIfMessageIsNullOrContainsException(messageSent,tcpClient,serverActivityInfo);
                        UpdateClientInfo(tcpClient, serverUser);
                        serverActivityInfo.ConnectedClientsListCallback(_allConnectedClients);
                    }
                   
                    break;

                case MessageActionType.AssistantCreateChatRoomAndSendInvites:

                    //Add ChatRoomID and send to the chatRoom manager

                    //Add InviteID and sent to all guest Server Users



                    break;
            }
        }

        
        private void UpdateClientInfo(TcpClient tcpClient, ServerUser serverUser)
        {
            var selectedClientInfo = _allConnectedClients.Where(a => a.tcpClient == tcpClient).FirstOrDefault();
            if (selectedClientInfo != null)
            {
                selectedClientInfo.ServerUserID = serverUser.ServerUserID;
                selectedClientInfo.Username = serverUser.Username;
            }
        }
        #endregion Private Methods

    }
}
