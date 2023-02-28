using ChatRoomServer.DomainLayer.Models;
using System.Net.Sockets;

namespace ChatRoomServer.Utils.Interfaces
{
    public interface IClientAction
    {
        void SetAllConnectedClients(List<ClientInfo> allConnectedClients);

        List<ClientInfo> GetAllConnectedClients();

        void AddNewClientConnectionToAllConnectedClients(TcpClient client);

        void ResolveCommunicationFromClient(TcpClient tcpClient, ServerActivityInfo serverActivityInfo);

        void RemoveAllCreatedChatRooms();
    }
}
