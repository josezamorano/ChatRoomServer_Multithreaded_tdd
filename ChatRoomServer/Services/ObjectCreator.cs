using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Utils.Enumerations;
using ChatRoomServer.Utils.Interfaces;

namespace ChatRoomServer.Services
{
    public class ObjectCreator :IObjectCreator
    {

        public Payload CreatePayload(List<ClientInfo> allConnectedClients, MessageActionType messageActionType, Guid? userId, string username)
        {
            var serverUsers = allConnectedClients.Select(a => new { a?.Username, a?.ServerUserID });
            List<ServerUser> allActiveServerUsers = new List<ServerUser>();
            foreach (var user in serverUsers)
            {
                ServerUser serverUser = new ServerUser()
                {
                    Username = user.Username,
                    ServerUserID = user.ServerUserID,
                };
                allActiveServerUsers.Add(serverUser);
            }
            Payload createdPayload = new Payload()
            {
                MessageActionType = messageActionType,
                UserGuid = userId,
                ClientUsername = username,
                ActiveServerUsers = allActiveServerUsers,
            };
            return createdPayload;
        }
    }
}
