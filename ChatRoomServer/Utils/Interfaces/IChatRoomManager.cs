using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Utils.Enumerations;

namespace ChatRoomServer.Utils.Interfaces
{
    public interface IChatRoomManager
    {
        void SetChatRoomUpdateCallback(ChatRoomsUpdateDelegate chatRoomUpdateCallback);

        ChatRoom CreateChatRoom(ChatRoom chatRoomFromServerUser);

        void AddChatRoomToAllChatRooms(ChatRoom chatRoom);

        List<ChatRoom> GetAllCreatedChatRooms();

        bool UpdateChatRoomStatus(Guid chatRoomId, ChatRoomStatus chatRoomStatus);

        bool AddActiveUserToChatRoom(Guid chatRoomId, ServerUser serverUser);

        bool RemoveUserFromChatRoom(Guid chatRoomId, ServerUser serverUser);

        bool UpdateInviteStatusInChatRoom(Guid chatRoomId, Guid inviteId, InviteStatus inviteStatus);

        bool RecordMessageInChatRoomConversation(Guid chatRoomId, string message);
    }
}
