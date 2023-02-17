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

        void SetAllActiveServerUsers(List<ServerUser> allActiveServerUsers);

        List<ServerUser> GetAllActiveServerUsers();

        void PollClientConnection(TcpClient client);

        void ResolveClientCommunication(TcpClient tcpClient, ServerActivationInfo serverActivationInfo);
    }
}
