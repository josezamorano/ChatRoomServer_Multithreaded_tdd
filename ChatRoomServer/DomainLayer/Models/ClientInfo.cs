using System.Net.Sockets;

namespace ChatRoomServer.DomainLayer.Models
{
    public class ClientInfo
    {

        public TcpClient tcpClient { get; set; }

        public Guid? ServerUserID { get; set; }

        public string Username { get; set; }
    }
}
