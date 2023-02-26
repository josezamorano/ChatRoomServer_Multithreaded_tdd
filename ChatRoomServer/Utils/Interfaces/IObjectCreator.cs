using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Utils.Enumerations;



namespace ChatRoomServer.Utils.Interfaces
{
    public interface IObjectCreator
    {
        Payload CreatePayload(List<ClientInfo> allConnectedClients, MessageActionType messageActionType, Guid? userId, string username);

        Payload CreatePayload(List<ClientInfo> allConnectedClients, MessageActionType messageActionType, ServerUser serverUser, ChatRoom chatRoom);

        Payload CreatePayload(List<ClientInfo> allConnectedClients, MessageActionType messageActionType, ServerUser serverUser, Invite invite);

        Payload CreatePayload(List<ClientInfo> allConnectedClients, MessageActionType messageActionType, ServerUser targetServerUser, ChatRoom chatRoom, string messageToChatRoom);

        Payload CreatePayload(List<ClientInfo> allConnectedClients, MessageActionType messageActionType, ServerUser targetServerUser, ChatRoom chatRoom, Invite invite);

        ChatRoom CreateChatRoom(string chatRoomName, ServerUser serverUserCreator, List<ServerUser> allActiveUsersInChatRoom, List<Invite> allInvitesSentToGuestUsers);
    }
}
