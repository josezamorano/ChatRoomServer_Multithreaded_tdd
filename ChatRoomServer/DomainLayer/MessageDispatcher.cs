using ChatRoomServer.DomainLayer.Models;
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
