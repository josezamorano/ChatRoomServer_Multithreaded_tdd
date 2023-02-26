﻿namespace ChatRoomServer.Utils.Enumerations
{
    public enum MessageActionType
    {
        None = 0,
        ClientConnectToServer = 1,
        CreateUser = 2,
        UserActivated = 3,
        RetryUsernameTaken = 4,
        RetryUsernameError = 5,
        ManagerCreateChatRoomAndSendInvites = 6,
        ServerChatRoomCreated = 7,
        ServerInviteSent = 8,
        ServerUserAcceptInvite = 9,
        ServerUserRejectInvite = 10,
        ClientSendMessageToChatRoom = 11,
        ServerBroadcastMessageToChatRoom = 12,
        ServerStopped = 13
    }
}
