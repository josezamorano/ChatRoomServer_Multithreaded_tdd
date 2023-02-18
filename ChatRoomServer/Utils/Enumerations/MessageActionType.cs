namespace ChatRoomServer.Utils.Enumerations
{
    public enum MessageActionType
    {
        None = 0,
        ClientConnectToServer = 1,
        CreateUser = 2,
        UserActivated = 3,
        RetryUsernameTaken = 4,
        RetryUsernameError = 5,
        AssistantCreateChatRoom = 6,
        BroadcastMessageToChatRoom = 7,
        ServerStopped = 8
    }
}
