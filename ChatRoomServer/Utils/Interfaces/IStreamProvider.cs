using System.Net.Sockets;

namespace ChatRoomServer.Utils.Interfaces
{
    public interface IStreamProvider
    {
        StreamReader CreateStreamReader(NetworkStream networkStream);

        StreamWriter CreateStreamWriter(NetworkStream networkStream);
    }
}
