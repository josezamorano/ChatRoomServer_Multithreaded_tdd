
using ChatRoomServer.DataAccessLayer.IONetwork;
using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Services;
using ChatRoomServer.Utils.Interfaces;
using Xunit;


namespace ChatRoomServerTests.DataAccessLayerTests
{
    public class SerializationProviderTest
    {
        ISerializationProvider _serializationProvider;
        public SerializationProviderTest()
        {
            _serializationProvider = new SerializationProvider();
        }


        [Fact]
        public void SerializeObject_CorrectInputObject_ReturnsOk()
        {
            //Arrange
            string expectedSubstring = "username";
            Payload payload = new Payload() 
            {
                ClientUsername = expectedSubstring,
            };
            //Act
            var actualResult = _serializationProvider.SerializeObject(payload);
            //Assert
            Assert.Contains(expectedSubstring, actualResult);
            
        }

        [Fact]
        public void SerializeObject_StringInputObject_ReturnsOK()
        {
            //Arrange
            string expectedSubstring = "username";
           
            //Act
            var actualResult = _serializationProvider.SerializeObject(expectedSubstring);
            //Assert
            Assert.Contains(expectedSubstring, actualResult);

        }

        [Fact]
        public void SerializeObject_NullInputObject_ReturnsOk()
        {
            //Arrange
            Payload payload = null;

            //Act
            var actualResult = _serializationProvider.SerializeObject(payload);
            //Assert
            Assert.Contains("null", actualResult);

        }


        [Fact]
        public void DeserializeObject_CorrectInputString_ReturnsOk()
        {
            //arrange
            string serializedObject = "{\"MessageActionType\":0,\"ClientUsername\":\"username\",\"UserId\":null,\"ActiveServerUsers\":null,\"ChatRoomCreated\":null,\"InviteToGuestUser\":null,\"MessageToChatRoom\":null,\"ServerUserDisconnected\":null,\"ServerUserRemovedFromChatRoom\":null}";
            //Act
            var actualResult = _serializationProvider.DeserializeObject<Payload>(serializedObject);
            //Assert
            Assert.IsType<Payload>(actualResult);
        }


        [Fact]
        public void DeserializeObject_BadInputString_ReturnsERROR()
        {
            //arrange
            string serializedObject = "this is a string";
            //Act
            var actualResult = _serializationProvider.DeserializeObject<Payload>(serializedObject);
            //Assert
            Assert.Null(actualResult);
        }
    }
}
