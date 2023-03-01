using System.Net;

namespace ChatRoomServer.Utils.Interfaces
{
    public interface IDnsProvider
    {
        IPHostEntry GetDnsHostEntry();
    }
}
