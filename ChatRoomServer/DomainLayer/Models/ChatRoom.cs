using ChatRoomServer.Utils.Enumerations;

namespace ChatRoomServer.DomainLayer.Models
{
    public class ChatRoom
    {
        public Guid ChatRoomId { get; set; }

        public string ChatRoomName { get; set; }

        public string ChatRoomIdentifierNameId { get; set; }

        public ChatRoomStatus ChatRoomStatus { get; set; }

        public ServerUser Creator { get; set; }

        public string ConversationRecord { get; set; }

        public List<ServerUser> AllActiveUsersInChatRoom { get; set; }

        public List<Invite> AllInvitesSentToGuestUsers { get; set; }
    }
}
