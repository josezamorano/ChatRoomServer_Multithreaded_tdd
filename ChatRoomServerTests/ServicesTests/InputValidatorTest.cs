using ChatRoomServer.Services;
using ChatRoomServer.Utils.Interfaces;
using NuGet.Frameworks;
using Xunit;


namespace ChatRoomServerTests.ServicesTests
{
    public class InputValidatorTest
    {
        IInputValidator _inputValidator;
        public InputValidatorTest()
        {
            _inputValidator = new InputValidator();            
        }
        [Fact]
        public void ValidateServerInputs_CorrectInputs_ReturnsOK()
        {
            //Arrange
            string port = "50000";
            //Act
            var actualResult = _inputValidator.ValidateServerInputs(port);
            //Assert
            Assert.Equal(string.Empty, actualResult);
        }
    }
}
