using ChatRoomServer.Utils.Enumerations;

namespace ChatRoomServer.DomainLayer.Models
{
    public class Invite
    {
        public ServerUser ChatRoomCreator { get; set; }

        public ServerUser GuestServerUser { get; set; }

        public string ChatRoomName { get; set; }

        public Guid ChatRoomId { get; set; }

        public InviteStatus InviteStatus { get; set; }

    }
}
