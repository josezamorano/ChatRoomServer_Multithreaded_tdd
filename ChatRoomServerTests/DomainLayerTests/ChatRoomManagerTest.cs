using ChatRoomServer.DomainLayer;
using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Services;
using ChatRoomServer.Utils.Interfaces;
using Xunit;

namespace ChatRoomServerTests.DomainLayerTests
{
    public class ChatRoomManagerTest
    {

        IObjectCreator _objectCreator;
        IChatRoomManager _chatRoomManager;

        public ChatRoomManagerTest()
        {
            _objectCreator = new ObjectCreator();
            _chatRoomManager = new ChatRoomManager(_objectCreator);

            
        }

        [Fact]
        public void CreateChatRoom_CorrectInputs_ReturnOK()
        {
            //Arrange
            var id = Guid.NewGuid();
            var name = "test";
            var identifier = name + "_" + id;
            ChatRoom chatRoom = new ChatRoom()
            {
                ChatRoomId = id,
                ChatRoomName = name,
                ChatRoomIdentifierNameId = identifier,
                ChatRoomStatus = ChatRoomServer.Utils.Enumerations.ChatRoomStatus.Created,
                Creator = new ServerUser() { Username = "Test", ServerUserID = Guid.NewGuid() },
                AllInvitesSentToGuestUsers = new List<Invite>(),
                AllActiveUsersInChatRoom = new List<ServerUser>()
                
            };
            //Act
            var actualResult = _chatRoomManager.CreateChatRoom(chatRoom);
            //Assert

            Assert.Equal(actualResult.ChatRoomName, name);

        }

    }
}
