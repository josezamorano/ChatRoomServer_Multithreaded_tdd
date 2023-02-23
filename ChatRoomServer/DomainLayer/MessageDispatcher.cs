﻿using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Services;
using ChatRoomServer.Utils.Enumerations;
using ChatRoomServer.Utils.Interfaces;
using System.Net.Sockets;

namespace ChatRoomServer.DomainLayer
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

        public string SendMessageServerStopping(List<ClientInfo> allConnectedClients, TcpClient tcpClient, Guid serverUserId, string username)
        {
            Payload payloadUsernameError = _objectCreator.CreatePayload(allConnectedClients, MessageActionType.ServerStopped, serverUserId, username);
            string messageSent = SendMessage(tcpClient, payloadUsernameError);
            return messageSent;
        }

        public string SendMessageUserActivated(List<ClientInfo> allConnectedClients, Guid ServerUserID, string username)
        {
            Payload payloadUsernameOk = _objectCreator.CreatePayload(allConnectedClients, MessageActionType.UserActivated, ServerUserID, username);
            foreach (ClientInfo clientInfo in allConnectedClients)
            {
                string messageSent = SendMessage(clientInfo.tcpClient, payloadUsernameOk);
            }
            return Notification.MessageSentOk;
        }

        public string SendMessageUsernameTaken(List<ClientInfo> allConnectedClients, TcpClient tcpClient, string username)
        {
            Payload payloadUsernameError = _objectCreator.CreatePayload(allConnectedClients, MessageActionType.RetryUsernameTaken, null, username);
            string messageSent = SendMessage(tcpClient, payloadUsernameError);
            return messageSent;
        }

        public string SendMessageInviteDispatchedToUser(List<ClientInfo> allConnectedClients,ClientInfo clientInfo, Invite invite)
        {
            ServerUser targetServerUser = new ServerUser() { ServerUserID = clientInfo.ServerUserID, Username = clientInfo.Username};
            Payload payloadChatRoomCreated = _objectCreator.CreatePayload(allConnectedClients, MessageActionType.ServerInviteSent, targetServerUser, invite);
            string messageSent = SendMessage(clientInfo.tcpClient, payloadChatRoomCreated);
            return messageSent;
        }

        public string SendMessageChatRoomCreated(List<ClientInfo> allConnectedClients, ClientInfo clientInfo, ChatRoom chatRoom)
        {
            ServerUser targetServerUser = new ServerUser() { ServerUserID = clientInfo.ServerUserID, Username = clientInfo.Username };
            Payload payloadChatRoomCreated = _objectCreator.CreatePayload(allConnectedClients, MessageActionType.ServerChatRoomCreated, targetServerUser, chatRoom);
            string messageSent = SendMessage(clientInfo.tcpClient, payloadChatRoomCreated);
            return messageSent;
        }

        #region Private Methods
        private string SendMessage(TcpClient tcpClient, Payload payload)
        {
            string serializedObject = _serializationProvider.SerializeObject(payload);
            string messageSent = _transmitter.sendMessageToClient(tcpClient, serializedObject);
            return messageSent;
        }
        #endregion Private Methods

    }
}
