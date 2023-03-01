using ChatRoomServer.DataAccessLayer.IONetwork;
using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Services;
using ChatRoomServer.Utils.Interfaces;
using ChatRoomServerTests.MockClasses;
using Xunit;

namespace ChatRoomServerTests.ServicesTests
{
    public class MessageDispatcherTest
    {
        IObjectCreator _objectCreator;
        ISerializationProvider _serializationProvider;
        ITransmitter _transmitter;
        IMessageDispatcher _messageDispatcher;

        public MessageDispatcherTest()
        {
            _objectCreator = new ObjectCreator();
            _serializationProvider = new SerializationProvider();
            _transmitter = new Mock_Transmitter();
            _messageDispatcher = new MessageDispatcher(_objectCreator, _serializationProvider, _transmitter);
        }
        [Fact]
        public void SendMessageServerStopping_CorrectInput_ReturnsOK()
        {
            //Arrange
            List<ClientInfo> allClients = new List<ClientInfo>() { new ClientInfo() { Username="test1", ServerUserID= Guid.NewGuid() } };
            ClientInfo clientInfo = new ClientInfo() { Username="client1", ServerUserID= Guid.NewGuid()};
            //Act
            var actualResult = _messageDispatcher.SendMessageServerStopping(allClients, clientInfo);
            //Assert
            Assert.Equal(Notification.MessageSentOk, actualResult);
        }
    }
}
