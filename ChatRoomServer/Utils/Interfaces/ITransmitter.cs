using ChatRoomServer.DomainLayer;
using System.Net.Sockets;

namespace ChatRoomServer.Utils.Interfaces
{
    public interface ITransmitter
    {
        string sendMessageToClient(TcpClient tcpClient, string messageLine);

        void ReceiveMessageFromClient(TcpClient tcpClient , MessageFromClientDelegate messageFromClientCallback);
    }
}
