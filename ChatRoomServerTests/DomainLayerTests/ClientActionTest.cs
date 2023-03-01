using ChatRoomServer;
using ChatRoomServer.DataAccessLayer.IONetwork;
using ChatRoomServer.DomainLayer;
using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Services;
using ChatRoomServer.Utils.Interfaces;
using ChatRoomServerTests.MockClasses;
using System.Drawing.Text;
using System.Net.Sockets;
using Xunit;


namespace ChatRoomServerTests.DomainLayerTests
{
    public class ClientActionTest
    {
        ISerializationProvider _serializationProvider;
        ITransmitter _transmitter;

        IObjectCreator _objectCreator;
        IMessageDispatcher _messageDispatcher;

        IChatRoomManager _chatRoomManager;
        IClientAction _clientAction;
        public ClientActionTest()
        {
            _serializationProvider = new SerializationProvider();
            _transmitter = new Mock_Transmitter();
            _objectCreator = new ObjectCreator();
            _messageDispatcher = new MessageDispatcher(_objectCreator,_serializationProvider,_transmitter);

            _chatRoomManager = new ChatRoomManager(_objectCreator);

            _clientAction = new ClientAction(_serializationProvider,_transmitter, _messageDispatcher,_chatRoomManager);
        }


        [Fact]
        public void ResolveCommunicationFromClient_CorrectInput_ReturnsOK()
        {
            //Arrange
            void ServerLoggerReportCallback(string log)
            {
                //Arrange
                string expectedValue = "this is a message";
                //Assert
                Assert.Equal(expectedValue, log);
            }
            void ServerStatusReportCallback(bool target)
            {

            }
            void ConnectedClientsCountReportCallback( int count) 
            { 
            
            }
            void ConnectedClientsListReportCallback(List<ClientInfo> allClients)
            {

            }

            ServerActivityInfo serverActivityInfo = new ServerActivityInfo()
            {
                Port =5000,
                ServerLoggerCallback = new ServerLoggerDelegate(ServerLoggerReportCallback),
                ServerStatusCallback = new ServerStatusDelegate(ServerStatusReportCallback),
                ConnectedClientsCountCallback = new ConnectedClientsCountDelegate(ConnectedClientsCountReportCallback),
                ConnectedClientsListCallback = new ConnectedClentsListDelegate(ConnectedClientsListReportCallback)
            };
            TcpClient client = new TcpClient();
            //Act
            _clientAction.ResolveCommunicationFromClient(client, serverActivityInfo);
            //Assert
        }
    }
}
