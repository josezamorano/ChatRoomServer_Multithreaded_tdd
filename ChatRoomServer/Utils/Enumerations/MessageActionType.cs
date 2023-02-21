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
        AssistantCreateChatRoomAndSendInvites = 6,
        ServerChatRoomCreatedAndInvitesSent = 7,
        ServerUserAcceptInvite = 8,
        ServerUserDeclineInvite = 9,
        BroadcastMessageToChatRoom = 10,
        ServerStopped = 11
    }
}
