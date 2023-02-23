using ChatRoomServer.Utils.Enumerations;
using System.Security.Permissions;

namespace ChatRoomServer.DomainLayer.Models
{
    public class Payload
    {
        public MessageActionType MessageActionType { get; set; }

        public string ClientUsername { get; set; }

        public Guid? UserId { get; set; }

        public List<ServerUser> ActiveServerUsers { get; set; }

        public ChatRoom ChatRoomCreated { get; set; }

        public Invite InviteToGuestUser { get; set; }
    }
}
