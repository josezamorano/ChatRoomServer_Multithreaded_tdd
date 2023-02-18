using ChatRoomServer.DomainLayer.Models;

namespace ChatRoomServer.Utils.Interfaces
{
    public interface IServerManager
    {
        string GetLocalIP();

        void StartServer(ServerActivationInfo serverActivationInfo);

        void StopServer(ServerActivationInfo serverActivationInfo);
    }
}
