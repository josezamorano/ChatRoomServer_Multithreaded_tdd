
using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Utils.Interfaces;
using System.Net.Sockets;

namespace ChatRoomServerTests.MockClasses
{
    public class Mock_ClientAction : IClientAction
    {

        List<ClientInfo> _allConnectedClients;

        public Mock_ClientAction()
        {
            _allConnectedClients = new List<ClientInfo>();  
        }
        public void AddNewClientConnectionToAllConnectedClients(TcpClient client)
        {
            throw new NotImplementedException();
        }

        public List<ClientInfo> GetAllConnectedClients()
        {
            throw new NotImplementedException();
        }

        public void RemoveAllCreatedChatRooms()
        {
            throw new NotImplementedException();
        }

        public void ResolveCommunicationFromClient(TcpClient tcpClient, ServerActivityInfo serverActivityInfo)
        {
            throw new NotImplementedException();
        }

        public void SetAllConnectedClients(List<ClientInfo> allConnectedClients)
        {
            _allConnectedClients = allConnectedClients; 
        }
    }
}
