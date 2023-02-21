using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Utils.Enumerations;

namespace ChatRoomServer.Utils.Interfaces
{
    public interface IObjectCreator
    {
        Payload CreatePayload(List<ClientInfo> allConnectedClients, MessageActionType messageActionType, Guid? userId, string username);
    }
}
