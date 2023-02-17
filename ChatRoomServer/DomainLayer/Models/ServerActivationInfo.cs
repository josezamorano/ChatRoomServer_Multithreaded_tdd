namespace ChatRoomServer.DomainLayer.Models
{
    public class ServerActivationInfo
    {
        public int Port { get; set; }

        public ServerLoggerDelegate ServerLoggerCallback { get; set; }

        public ServerStatusDelegate ServerStatusCallback { get; set; }

        public ConnectedClientsDelegate ConnectedClientsCallback { get; set; }
    }
}
