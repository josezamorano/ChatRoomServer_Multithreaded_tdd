using ChatRoomServer.DomainLayer.Models;

namespace ChatRoomServer.Utils.Interfaces
{
    public interface IServerManager
    {
        string GetLocalIP();

        void StartServer(ServerActivityInfo serverActivityInfo);

        void StopServer(ServerActivityInfo serverActivityInfo);
    }
}
