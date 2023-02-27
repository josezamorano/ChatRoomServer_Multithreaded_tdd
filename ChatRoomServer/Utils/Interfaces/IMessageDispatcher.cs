using ChatRoomServer.DomainLayer.Models;
using System.Net.Sockets;

namespace ChatRoomServer.Utils.Interfaces
{
    public interface IMessageDispatcher
    {
        string SendMessageServerStopping(List<ClientInfo> allConnectedClients, ClientInfo clientInfo);

        string SendMessageClientDisconnectionAccepted(List<ClientInfo> allConnectedClients, ClientInfo clientInfo);

        string SendMessageServerUserIsDisconnected(List<ClientInfo> allConnectedClients, ClientInfo clientInfo, ServerUser serverUserDisconnected);

        string SendMessageUserActivated(List<ClientInfo> allConnectedClients, Guid ServerUserID, string username);

        string SendMessageUsernameTaken(List<ClientInfo> allConnectedClients, TcpClient tcpClient, string username);

        string SendMessageInviteDispatchedToUser(List<ClientInfo> allConnectedClients, ClientInfo clientInfo, Invite invite);

        string SendMessageChatRoomCreated(List<ClientInfo> allConnectedClients, ClientInfo clientInfo, ChatRoom chatRoom);

        string SendMessageServerUserExitedChatRoom(List<ClientInfo> allConnectedClients, ClientInfo clientInfo, ChatRoom chatRoom);

        string SendMessageBroadcastMessageToChatRoomActiveUser(List<ClientInfo> allConnectedClients, ClientInfo clientInfo, ChatRoom chatRoom, string messageToChatRoom);

        string SendMessageServerUserChatRoomUpdatedAndInviteAccepted(List<ClientInfo> allConnectedClients, ClientInfo clientInfo, ChatRoom chatRoom, Invite invite);

        string SendMessageServerUserChatRoomUpdatedAndInviteRejected(List<ClientInfo> allConnectedClients, ClientInfo clientInfo, Invite invite);
    }
}