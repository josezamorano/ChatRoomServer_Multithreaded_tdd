using ChatRoomServer.Utils.Enumerations;

namespace ChatRoomServer.DomainLayer.Models
{
    public class ClientActionResolvedReport
    {
        public MessageActionType? MessageActionType { get; set; }
        public ServerUser? ServerUser { get; set; }
    }
}
