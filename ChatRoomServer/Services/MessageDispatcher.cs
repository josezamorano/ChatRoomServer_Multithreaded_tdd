using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Utils.Enumerations;
using ChatRoomServer.Utils.Interfaces;
using System.Net.Sockets;




namespace ChatRoomServer.Services
{
    public class MessageDispatcher : IMessageDispatcher
    {
        IObjectCreator _objectCreator;
        ISerializationProvider _serializationProvider;
        ITransmitter _transmitter;
        public MessageDispatcher(IObjectCreator objectCreator, ISerializationProvider serializationProvider, ITransmitter transmitter)
        {
            _objectCreator = objectCreator;
            _serializationProvider = serializationProvider;
            _transmitter = transmitter;
        }

        public string SendMessageServerStopping(List<ClientInfo> allConnectedClients, ClientInfo clientInfo)
        {          
            string messageSent = ResolveMessageToClient(MessageActionType.ServerStopped, allConnectedClients, clientInfo);
            return messageSent;
        }

        public string SendMessageClientDisconnectionAccepted(List<ClientInfo> allConnectedClients, ClientInfo clientInfo)
        {
            string messageSent = ResolveMessageToClient(MessageActionType.ServerClientDisconnectAccepted,allConnectedClients, clientInfo);
            return messageSent;
        }

        public string SendMessageServerUserIsDisconnected(List<ClientInfo> allConnectedClients, ClientInfo clientInfo , ServerUser serverUserDisconnected)
        {
            string messageSent = ResolveSendMessageServerUser(MessageActionType.ServerUserIsDisconnected, allConnectedClients, clientInfo, serverUserDisconnected);
        //    ServerUser activeServerUser = new ServerUser() { ServerUserID = clientInfo.ServerUserID, Username = clientInfo.Username };
        //    Payload payloadUserIsDisconnected = _objectCreator.CreatePayload(allConnectedClients, MessageActionType.ServerUserIsDisconnected, activeServerUser, serverUserDisconnected);
        //    string messageSent = SendMessage(clientInfo.TcpClient, payloadUserIsDisconnected);
            return messageSent;
        }
                
        public string SendMessageUserActivated(List<ClientInfo> allConnectedClients, Guid ServerUserID, string username)
        {
            Payload payloadUsernameOk = _objectCreator.CreatePayload(allConnectedClients, MessageActionType.UserActivated, ServerUserID, username);
            foreach (ClientInfo clientInfo in allConnectedClients)
            {
                string messageSent = SendMessage(clientInfo.TcpClient, payloadUsernameOk);
            }
            return Notification.MessageSentOk;
        }

        public string SendMessageUsernameTaken(List<ClientInfo> allConnectedClients, TcpClient tcpClient, string username)
        {
            Payload payloadUsernameError = _objectCreator.CreatePayload(allConnectedClients, MessageActionType.RetryUsernameTaken, null, username);
            string messageSent = SendMessage(tcpClient, payloadUsernameError);
            return messageSent;
        }

        public string SendMessageInviteDispatchedToUser(List<ClientInfo> allConnectedClients, ClientInfo clientInfo, Invite invite)
        {
            ServerUser targetServerUser = new ServerUser() { ServerUserID = clientInfo.ServerUserID, Username = clientInfo.Username };
            Payload payloadChatRoomCreated = _objectCreator.CreatePayload(allConnectedClients, MessageActionType.ServerInviteSent, targetServerUser, invite);
            string messageSent = SendMessage(clientInfo.TcpClient, payloadChatRoomCreated);
            return messageSent;
        }

        public string SendMessageChatRoomCreated(List<ClientInfo> allConnectedClients, ClientInfo clientInfo, ChatRoom chatRoom)
        {
            string messageSent = ResolveMessageChatRoomStatus(MessageActionType.ServerChatRoomCreated, allConnectedClients, clientInfo, chatRoom);
            return messageSent;
        }

        public string SendMessageServerUserChatRoomExitAccepted(List<ClientInfo> allConnectedClients, ClientInfo clientInfo, ChatRoom chatRoom)
        {
            string message = ResolveMessageChatRoomStatus(MessageActionType.ServerExitChatRoomAccepted, allConnectedClients, clientInfo, chatRoom);
            return message;
        }

       
        public string SendMessageServerUserRemovedFromChatRoom(List<ClientInfo> allConnectedClients, ClientInfo clientInfo, ChatRoom chatRoom, ServerUser serverUserRemoved)
        {
            ServerUser activeServerUser = new ServerUser() { ServerUserID = clientInfo.ServerUserID, Username = clientInfo.Username };
            Payload payloadUserIsDisconnected = _objectCreator.CreatePayload(allConnectedClients, MessageActionType.ServerUserRemovedFromChatRoom, activeServerUser, chatRoom , serverUserRemoved);
            string messageSent = SendMessage(clientInfo.TcpClient, payloadUserIsDisconnected);
            return messageSent;
        }

        public string SendMessageBroadcastMessageToChatRoomActiveUser(List<ClientInfo> allConnectedClients, ClientInfo clientInfo, ChatRoom chatRoom, string messageToChatRoom)
        {
            ServerUser targetServerUser = new ServerUser() { ServerUserID = clientInfo.ServerUserID, Username = clientInfo.Username };
            Payload payloadMessageToActiveUser = _objectCreator.CreatePayload(allConnectedClients, MessageActionType.ServerBroadcastMessageToChatRoom, targetServerUser,chatRoom ,messageToChatRoom);
            string messageSent = SendMessage(clientInfo.TcpClient,payloadMessageToActiveUser);
            return messageSent;
        }

        public string SendMessageServerUserChatRoomUpdatedAndInviteAccepted(List<ClientInfo> allConnectedClients,ClientInfo clientInfo ,ChatRoom chatRoom , Invite invite)
        {
            ServerUser targetServerUser = new ServerUser() { ServerUserID = clientInfo.ServerUserID, Username = clientInfo.Username };
            Payload payloadInviteAccepted = _objectCreator.CreatePayload(allConnectedClients, MessageActionType.ServerUserAcceptInvite, targetServerUser, chatRoom , invite);
            string messageSent = SendMessage(clientInfo.TcpClient, payloadInviteAccepted);
            return messageSent;
        }


        public string SendMessageServerUserChatRoomUpdatedAndInviteRejected(List<ClientInfo> allConnectedClients, ClientInfo clientInfo, Invite invite)
        {
            ServerUser targetServerUser = new ServerUser() { ServerUserID = clientInfo.ServerUserID, Username = clientInfo.Username };
            Payload payloadInviteAccepted = _objectCreator.CreatePayload(allConnectedClients, MessageActionType.ServerUserRejectInvite, targetServerUser, invite);
            string messageSent = SendMessage(clientInfo.TcpClient, payloadInviteAccepted);
            return messageSent;
        }


        #region Private Methods

        private string ResolveSendMessageServerUser(MessageActionType messageActionType, List<ClientInfo> allConnectedClients, ClientInfo clientInfo, ServerUser targetServerUser)
        {
            ServerUser activeServerUser = new ServerUser() { ServerUserID = clientInfo.ServerUserID, Username = clientInfo.Username };
            Payload payloadUserIsDisconnected = _objectCreator.CreatePayload(allConnectedClients, messageActionType, activeServerUser, targetServerUser);
            string messageSent = SendMessage(clientInfo.TcpClient, payloadUserIsDisconnected);
            return messageSent;
        }

        private string ResolveMessageToClient(MessageActionType messageActionType, List<ClientInfo> allConnectedClients, ClientInfo clientInfo)
        {
            Payload payloadInfo = _objectCreator.CreatePayload(allConnectedClients, messageActionType, clientInfo.ServerUserID, clientInfo.Username);
            string messageSent = SendMessage(clientInfo.TcpClient, payloadInfo);
            return messageSent;
        }

        private string ResolveMessageChatRoomStatus(MessageActionType messageActionType, List<ClientInfo> allConnectedClients, ClientInfo clientInfo, ChatRoom chatRoom)
        {
            ServerUser targetServerUser = new ServerUser() { ServerUserID = clientInfo.ServerUserID, Username = clientInfo.Username };
            Payload payloadChatRoomCreated = _objectCreator.CreatePayload(allConnectedClients, messageActionType, targetServerUser, chatRoom);
            string messageSent = SendMessage(clientInfo.TcpClient, payloadChatRoomCreated);
            return messageSent;
        }
        private string SendMessage(TcpClient tcpClient, Payload payload)
        {
            string serializedObject = _serializationProvider.SerializeObject(payload);
            string messageSent = _transmitter.sendMessageToClient(tcpClient, serializedObject);
            return messageSent;
        }
        #endregion Private Methods

    }
}
