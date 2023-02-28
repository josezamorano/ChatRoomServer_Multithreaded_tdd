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

        void RemoveAllChatRooms();

        bool UpdateChatRoomStatus(Guid chatRoomId, ChatRoomStatus chatRoomStatus);

        bool UpdateInvitedGuestServerUserInChatRoom(Guid chatRoomId, InviteStatus inviteStatus, ServerUser serverUser);

        bool RemoveUserFromAllActiveUsersInChatRoom(Guid targetChatRoomId, Guid serverUserId);

        bool RemoveUserFromAllChatRooms(Guid serverUserId);

        bool RecordMessageInChatRoomConversation(Guid chatRoomId, string message);       
    }
}
