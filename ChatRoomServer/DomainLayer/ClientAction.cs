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
        IChatRoomManager _chatRoomManager;
        public ClientAction(ISerializationProvider serializationProvider,
                            ITransmitter transmitter, 
                            IMessageDispatcher messageDisptcher, 
                            IChatRoomManager chatRoomManager)
        {
            _serializationProvider = serializationProvider;
            _transmitter = transmitter;
            _messageDispatcher = messageDisptcher;
            _chatRoomManager = chatRoomManager;
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
                        TcpClient = client,
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
                var disconnectedClient = _allConnectedClients.Where(a => a.TcpClient == tcpClient).FirstOrDefault();
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
                disconnectedClient.TcpClient.Close();
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
                {
                    ClientInfo duplicateServerUser = _allConnectedClients.Where(a => a.Username.ToLower() == payload.ClientUsername.ToLower()).FirstOrDefault();
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
                        UpdateClientInfo(tcpClient, serverUser);
                        string messageSent = _messageDispatcher.SendMessageUserActivated(_allConnectedClients, (Guid)serverUser.ServerUserID, payload.ClientUsername);
                        VerifyIfMessageIsNullOrContainsException(messageSent, tcpClient, serverActivityInfo);

                        serverActivityInfo.ConnectedClientsListCallback(_allConnectedClients);
                    }
                }
                break;

                case MessageActionType.ManagerCreateChatRoomAndSendInvites:
                {
                    ChatRoom newChatRoomCreated = ResolveCreateChatRoom(payload.ChatRoomCreated);// _chatRoomManager.CreateChatRoom(payload.ChatRoomCreated);
                    _chatRoomManager.AddChatRoomToAllChatRooms(newChatRoomCreated);
                    SendChatRoomInviteToGuestUsers(newChatRoomCreated, tcpClient, serverActivityInfo);

                    Guid serverUserId = (Guid)newChatRoomCreated.Creator.ServerUserID;
                    SendCompleteChatRoomInfoToCreator(serverUserId, newChatRoomCreated, tcpClient, serverActivityInfo);
                    serverActivityInfo.ConnectedClientsListCallback(_allConnectedClients);
                }
                break;

                case MessageActionType.ClientSendMessageToChatRoom:
                {
                    string message = payload.MessageToChatRoom;
                    ChatRoom selectedChatRoom = _chatRoomManager.GetAllCreatedChatRooms().Where(a => a.ChatRoomId == payload.ChatRoomCreated.ChatRoomId).FirstOrDefault();
                    if(selectedChatRoom != null)
                    {
                        _chatRoomManager.RecordMessageInChatRoomConversation(selectedChatRoom.ChatRoomId, message);
                        foreach (var activeUser in selectedChatRoom.AllActiveUsersInChatRoom)
                        {
                            ClientInfo clientInfo = _allConnectedClients.Where(a=>a.ServerUserID == activeUser.ServerUserID).FirstOrDefault();
                            if (clientInfo != null && clientInfo.TcpClient !=null) 
                            {
                               string messageSent = _messageDispatcher.SendMessageBroadcastMessageToChatRoomActiveUser(_allConnectedClients, clientInfo, selectedChatRoom, message);
                                VerifyIfMessageIsNullOrContainsException(messageSent, tcpClient, serverActivityInfo);
                            }
                        }
                    }

                    serverActivityInfo.ConnectedClientsListCallback(_allConnectedClients);
                }
                break;

                case MessageActionType.ServerUserAcceptInvite:
                {
                    var chatRoomId = payload.ChatRoomCreated?.ChatRoomId;
                    var inviteId = payload.InviteToGuestUser?.InviteId;
                    var targetChatRoom = _chatRoomManager.GetAllCreatedChatRooms().Where(a => a.ChatRoomId == chatRoomId).FirstOrDefault();
                    if(targetChatRoom != null) 
                    {
                        var targetInvite = targetChatRoom.AllInvitesSentToGuestUsers.Where(b=>b.InviteId == inviteId ).FirstOrDefault();
                        if (targetInvite != null) 
                        {
                            targetInvite.InviteStatus = InviteStatus.Accepted;
                        }
                        targetChatRoom.AllActiveUsersInChatRoom.Add(targetInvite.GuestServerUser);
                    }

                    //find chatroom, and accept invite, add active user to chartoom and 
                    //update status of invites list.
                    //Send message to client that accepted invite has been recorded

                }
                break;
            }
        }

        
        private void UpdateClientInfo(TcpClient tcpClient, ServerUser serverUser)
        {
            var selectedClientInfo = _allConnectedClients.Where(a => a.TcpClient == tcpClient).FirstOrDefault();
            if (selectedClientInfo != null)
            {
                selectedClientInfo.ServerUserID = serverUser.ServerUserID;
                selectedClientInfo.Username = serverUser.Username;
            }
        }
        

        private ChatRoom ResolveCreateChatRoom(ChatRoom chatRoom )
        {
            ChatRoom newChatRoomCreated = _chatRoomManager.CreateChatRoom(chatRoom);
            ServerUser serverUserCreator = new ServerUser()
            {
                Username = chatRoom.Creator.Username,
                ServerUserID = chatRoom.Creator.ServerUserID
            };
            ServerUser serverUserDuplicated = newChatRoomCreated.AllActiveUsersInChatRoom.Where(a => a.ServerUserID == serverUserCreator.ServerUserID).FirstOrDefault();
            if (serverUserDuplicated == null)
            {
                newChatRoomCreated.AllActiveUsersInChatRoom.Add(serverUserCreator);
            }
            return newChatRoomCreated;
        }
        
        private void SendChatRoomInviteToGuestUsers(ChatRoom chatRoom, TcpClient tcpClient, ServerActivityInfo serverActivityInfo)
        {

            foreach (Invite inviteToGuest in chatRoom.AllInvitesSentToGuestUsers)
            {
                ClientInfo clientInfo = _allConnectedClients.Where(a => a.ServerUserID == inviteToGuest.GuestServerUser.ServerUserID).FirstOrDefault();

                if (clientInfo != null && clientInfo.TcpClient != null && clientInfo.TcpClient.Connected)
                {
                    string messageSent = _messageDispatcher.SendMessageInviteDispatchedToUser(_allConnectedClients, clientInfo, inviteToGuest);
                    VerifyIfMessageIsNullOrContainsException(messageSent, tcpClient, serverActivityInfo);
                }
            }
        }
        
        private void SendCompleteChatRoomInfoToCreator(Guid userId, ChatRoom chatRoomCreated, TcpClient tcpClient, ServerActivityInfo serverActivityInfo)
        {
            if (userId == chatRoomCreated.Creator.ServerUserID)
            {
                ClientInfo clientInfo = _allConnectedClients.Where(a => a.ServerUserID == userId).FirstOrDefault();
                if (clientInfo != null)
                {
                    string messageSent = _messageDispatcher.SendMessageChatRoomCreated(_allConnectedClients, clientInfo, chatRoomCreated);
                    VerifyIfMessageIsNullOrContainsException(messageSent, tcpClient, serverActivityInfo);
                }
            }
        }

        #endregion Private Methods

    }
}
