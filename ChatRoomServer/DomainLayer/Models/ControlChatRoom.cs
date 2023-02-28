using ChatRoomServer.Utils.Enumerations;

namespace ChatRoomServer.DomainLayer.Models
{
    public class ControlChatRoom
    {
        public ControlActionType ControlActionType { get; set; }
        public ChatRoom ChatRoomObject { get; set; }
    }
}
