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

                case MessageActionType.ClientDisconnect:
                {
                    var serverUserRemoved = _chatRoomManager.RemoveUserFromAllChatRooms((Guid)payload.UserId);
                    var clientForDisconnection = _allConnectedClients.Where(a=>a.ServerUserID == payload.UserId).FirstOrDefault();

                    
                    string messageSent = _messageDispatcher.SendMessageClientDisconnectionAccepted(_allConnectedClients, clientForDisconnection );
                    VerifyIfMessageIsNullOrContainsException(messageSent, tcpClient, serverActivityInfo);
                    CloseDisconnectedClient(clientForDisconnection, serverActivityInfo);

                    //To all remaining active clients notify that client is disconnected and they need to remove it from their lists
                    foreach(var clientInfo in _allConnectedClients)
                    {
                            if (clientInfo == null || !clientInfo.TcpClient.Connected) { continue; }
                        messageSent = _messageDispatcher.SendMessageServerUserIsDisconnected(_allConnectedClients, clientInfo, payload.ServerUserDisconnected);
                        VerifyIfMessageIsNullOrContainsException(messageSent, tcpClient, serverActivityInfo);
                    }

                }
                break;

                case MessageActionType.ManagerCreateChatRoomAndSendInvites:
                {
                    ChatRoom newChatRoomCreated = ResolveCreateChatRoom(payload.ChatRoomCreated);
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
                    Guid chatRoomId = (Guid)payload.ChatRoomCreated?.ChatRoomId;
                    Invite inviteReceivedFromGuest = payload.InviteToGuestUser;
                    ServerUser guestServerUser = inviteReceivedFromGuest.GuestServerUser;
                    ChatRoom targetChatRoom = _chatRoomManager.GetAllCreatedChatRooms().Where(a => a.ChatRoomId == chatRoomId).FirstOrDefault();
                    if (targetChatRoom == null || targetChatRoom.ChatRoomStatus == ChatRoomStatus.Closed)
                    {
                        //notify original Client that chatRoom is closed and eliminate invite.
                        ResolveSendMessageServerUserInviteRejected(guestServerUser, inviteReceivedFromGuest, tcpClient, serverActivityInfo);
                        return;
                    }

                    Guid inviteId = (Guid)payload.InviteToGuestUser?.InviteId;
                    Invite targetInvite = targetChatRoom.AllInvitesSentToGuestUsers.Where(b=>b.InviteId == inviteId ).FirstOrDefault();
                    if (targetInvite == null) 
                    {
                        //notify original client that invite does not exist in chatroom so it has not been approved to access chatRoom and eliminate invite
                        ResolveSendMessageServerUserInviteRejected(guestServerUser, inviteReceivedFromGuest, tcpClient, serverActivityInfo);
                        return;
                    }

                    _chatRoomManager.UpdateInvitedGuestServerUserInChatRoom(chatRoomId, InviteStatus.Accepted, guestServerUser);

                    ResolveSendMessageToAllActiveUserInChatRoom(chatRoomId, inviteReceivedFromGuest, tcpClient, serverActivityInfo);
                }
                break;

                case MessageActionType.ServerUserRejectInvite:
                {
                    Guid chatRoomId = (Guid)payload.ChatRoomCreated?.ChatRoomId;
                    Invite inviteReceivedFromGuest = payload.InviteToGuestUser;
                    ServerUser guestServerUser = inviteReceivedFromGuest.GuestServerUser;

                    _chatRoomManager.UpdateInvitedGuestServerUserInChatRoom(chatRoomId, InviteStatus.Rejected, guestServerUser);
                    ResolveSendMessageServerUserInviteRejected(guestServerUser, inviteReceivedFromGuest, tcpClient, serverActivityInfo);
                }
                break;

                case MessageActionType.ClientExitChatRoom:
                {
                    string messageSent = string.Empty;
                    //in server we remove the user from the list of active users and update display.
                    _chatRoomManager.RemoveUserFromAllActiveUsersInChatRoom(payload.ChatRoomCreated.ChatRoomId, (Guid)payload.UserId);
                    //If the chatRoom is still active, Notify the remaining users that one user exited the chatRoom 
                    ChatRoom selectedChatRoom = _chatRoomManager.GetAllCreatedChatRooms().Where(a => a.ChatRoomId == payload.ChatRoomCreated.ChatRoomId && a.ChatRoomStatus == ChatRoomStatus.OpenActive).FirstOrDefault();
                    if(selectedChatRoom !=null)
                    {
                        foreach (var serverUser in selectedChatRoom.AllActiveUsersInChatRoom)
                        {
                            ClientInfo clientInfo = _allConnectedClients.Where(a => a.ServerUserID == serverUser.ServerUserID).FirstOrDefault();
                            if(clientInfo != null && clientInfo.TcpClient.Connected)
                            {
                                messageSent = _messageDispatcher.SendMessageServerUserExitedChatRoom(_allConnectedClients, clientInfo, selectedChatRoom);
                                VerifyIfMessageIsNullOrContainsException(messageSent, tcpClient, serverActivityInfo);
                            }
                        }
                    }

                    //Notify the original client that its exit request is accepted and remove its chat room
                    ClientInfo clientInfoRemoteClient = _allConnectedClients.Where(a=>a.ServerUserID == payload.UserId).FirstOrDefault();
                    messageSent = _messageDispatcher.SendMessageServerUserExitedChatRoom(_allConnectedClients, clientInfoRemoteClient, payload.ChatRoomCreated);
                    VerifyIfMessageIsNullOrContainsException(messageSent, tcpClient, serverActivityInfo);
                }
                break;
            }
        }

        private bool ResolveSendMessageServerUserInviteRejected(ServerUser guestServerUser, Invite inviteReceivedFromGuest, TcpClient tcpClient,ServerActivityInfo serverActivityInfo)
        {
            ClientInfo targetClient = _allConnectedClients.Where(a => a.ServerUserID == guestServerUser.ServerUserID).FirstOrDefault();
            if (targetClient == null || !targetClient.TcpClient.Connected) 
            { 
                return false; 
            }
            //send message to REMOVE Original server user who sent the INVITE rejection
            if (targetClient.ServerUserID == inviteReceivedFromGuest.GuestServerUser.ServerUserID)
            {
                var messageSent = _messageDispatcher.SendMessageServerUserChatRoomUpdatedAndInviteRejected(_allConnectedClients, targetClient, inviteReceivedFromGuest);
                VerifyIfMessageIsNullOrContainsException(messageSent, tcpClient, serverActivityInfo);
                return true;
            }
            return false;
        }

        private void ResolveSendMessageToAllActiveUserInChatRoom(Guid targetChatRoomId, Invite inviteReceivedFromGuest, TcpClient tcpClient,ServerActivityInfo serverActivityInfo ) 
        {
            ChatRoom updatedChatRoom = _chatRoomManager.GetAllCreatedChatRooms().Where(a => a.ChatRoomId == targetChatRoomId).FirstOrDefault();
            foreach (var guestUser in updatedChatRoom.AllActiveUsersInChatRoom)
            {
                string messageSent = string.Empty;
                ClientInfo targetClient = _allConnectedClients.Where(a => a.ServerUserID == guestUser.ServerUserID).FirstOrDefault();
                if (targetClient == null || !targetClient.TcpClient.Connected) { continue; }

                if (targetClient.ServerUserID == inviteReceivedFromGuest.GuestServerUser.ServerUserID)
                {
                    messageSent = _messageDispatcher.SendMessageServerUserChatRoomUpdatedAndInviteAccepted(_allConnectedClients, targetClient, updatedChatRoom, inviteReceivedFromGuest);
                }
                else
                {
                    messageSent = _messageDispatcher.SendMessageServerUserChatRoomUpdatedAndInviteAccepted(_allConnectedClients, targetClient, updatedChatRoom, null);
                }               

                VerifyIfMessageIsNullOrContainsException(messageSent, tcpClient, serverActivityInfo);

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
