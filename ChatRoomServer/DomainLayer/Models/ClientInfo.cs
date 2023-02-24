using System.Net.Sockets;

namespace ChatRoomServer.DomainLayer.Models
{
    public class ClientInfo
    {

        public TcpClient TcpClient { get; set; }

        public Guid? ServerUserID { get; set; }

        public string Username { get; set; }
    }
}
