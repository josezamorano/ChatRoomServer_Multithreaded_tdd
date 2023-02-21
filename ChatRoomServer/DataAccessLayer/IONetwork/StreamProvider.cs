using ChatRoomServer.Utils.Interfaces;
using System.Net.Sockets;

namespace ChatRoomServer.DataAccessLayer.IONetwork
{
    public class StreamProvider : IStreamProvider
    {

        public StreamReader CreateStreamReader(NetworkStream networkStream)
        {
            StreamReader streamReader = new StreamReader(networkStream);
            return streamReader;
        }

        public StreamWriter CreateStreamWriter(NetworkStream networkStream)
        {
            StreamWriter streamWriter = new StreamWriter(networkStream);
            return streamWriter;
        }

    }
}
