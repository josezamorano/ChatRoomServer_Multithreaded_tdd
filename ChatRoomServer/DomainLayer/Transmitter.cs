using ChatRoomServer.Services;
using ChatRoomServer.Utils.Interfaces;
using System.Net.Sockets;

namespace ChatRoomServer.DomainLayer
{
    
    public class Transmitter : ITransmitter
    {
        IStreamProvider _streamProvider;
        public Transmitter(IStreamProvider streamProvider)
        {
            _streamProvider = streamProvider;
        }


        public string sendMessageToClient(TcpClient tcpClient, string messageLine)
        {
            try
            {
                if (tcpClient == null) { return string.Empty; }
                StreamWriter streamWriter = _streamProvider.CreateStreamWriter(tcpClient.GetStream());
                streamWriter.WriteLine(Notification.ServerPayload + messageLine);
                streamWriter.Flush();

                return Notification.MessageSentOk;
            }
            catch(Exception ex)
            {
                string log = Notification.CRLF + Notification.Exception + "Problem Sending message to the Client..." + Notification.CRLF + ex.ToString();
                return log;
            }
        }

        public void ReceiveMessageFromClient(TcpClient tcpClient , MessageFromClientDelegate messageFromClientCallback)
        {
            try
            {
                StreamReader streamReader = _streamProvider.CreateStreamReader(tcpClient.GetStream());               
                while (tcpClient.Connected)
                {
                    var input = streamReader.ReadLine(); // blocks here until something is received from client
                    messageFromClientCallback( input);
                    if(input == null)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                string log = Notification.CRLF + Notification.Exception + "Problem Reading message from the Client..." + Notification.CRLF + ex.ToString();
                messageFromClientCallback( log );
            }
        }
    }
}
