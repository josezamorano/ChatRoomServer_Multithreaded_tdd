using ChatRoomServer.DomainLayer.Models;
using System.Net.Sockets;

namespace ChatRoomServer.Utils.Interfaces
{
    public interface IMessageDispatcher
    {
        string SendMessageServerStopping(List<ClientInfo> allConnectedClients, TcpClient tcpClient, Guid serverUserId, string username);

        string SendMessageUserActivated(List<ClientInfo> allConnectedClients, Guid ServerUserID, string username);

        string SendMessageUsernameTaken(List<ClientInfo> allConnectedClients, TcpClient tcpClient, string username);

        string SendMessageInviteDispatchedToUser(List<ClientInfo> allConnectedClients, ClientInfo clientInfo, Invite invite);

        string SendMessageChatRoomCreated(List<ClientInfo> allConnectedClients, ClientInfo clientInfo, ChatRoom chatRoom);
                
        string SendMessageBroadcastMessageToChatRoomActiveUser(List<ClientInfo> allConnectedClients, ClientInfo clientInfo, ChatRoom chatRoom, string messageToChatRoom);
    }
}