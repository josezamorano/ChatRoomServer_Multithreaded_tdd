using ChatRoomServer.DomainLayer.Models;
using System.Net.Sockets;

namespace ChatRoomServer.Utils.Interfaces
{
    public interface IClientAction
    {
        void SetAllConnectedClients(List<ClientInfo> allConnectedClients);

        List<ClientInfo> GetAllConnectedClients();

        void AddNewClientConnectionToAllConnectedClients(TcpClient client);

        void RemoveDisconnectedClientFromAllConnectedClients(ClientInfo disconnectedClient);

        string SendMessageServerStopping(TcpClient tcpClient, Guid ServerUserId, string username);

        void ResolveClientCommunication(TcpClient tcpClient, ServerActivityInfo serverActivityInfo);
    }
}
