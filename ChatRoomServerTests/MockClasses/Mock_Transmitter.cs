using ChatRoomServer.DomainLayer;
using ChatRoomServer.Services;
using ChatRoomServer.Utils.Interfaces;
using System.Net.Sockets;

namespace ChatRoomServerTests.MockClasses
{
    public class Mock_Transmitter : ITransmitter
    {
        public void ReceiveMessageFromClient(TcpClient tcpClient, MessageFromClientDelegate messageFromClientCallback)
        {
            string message = "this is a message";
            messageFromClientCallback(message);
        }

        public string sendMessageToClient(TcpClient tcpClient, string messageLine)
        {
            return Notification.MessageSentOk;
        }
    }
}
