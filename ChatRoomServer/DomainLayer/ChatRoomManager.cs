using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Utils.Enumerations;
using ChatRoomServer.Utils.Interfaces;

namespace ChatRoomServer.DomainLayer
{
    public class ChatRoomManager : IChatRoomManager
    {
        private const string CRLF = "\r\n";
        private ChatRoomsUpdateDelegate _chatRoomUpdateCallback;
        private List<ControlChatRoom> _allCreatedChatRooms;

        IObjectCreator _objectCreator;
        public ChatRoomManager(IObjectCreator objectCreator)
        {
            _allCreatedChatRooms = new List<ControlChatRoom>();
            _objectCreator = objectCreator;
        }

        public void SetChatRoomUpdateCallback(ChatRoomsUpdateDelegate chatRoomUpdateCallback)
        {
            _chatRoomUpdateCallback = chatRoomUpdateCallback;
        }

        //Test
        public ChatRoom CreateChatRoom(ChatRoom chatRoomFromServerUser)
        {
            ChatRoom chatRoomCreated = _objectCreator.CreateChatRoom(chatRoomFromServerUser.ChatRoomName, chatRoomFromServerUser.Creator, chatRoomFromServerUser.AllActiveUsersInChatRoom, chatRoomFromServerUser.AllInvitesSentToGuestUsers);

            return chatRoomCreated;
        }

        public void AddChatRoomToAllChatRooms( ChatRoom chatRoom)
        {
            var existingChatRoom = _allCreatedChatRooms.Where( a=>a.ChatRoomObject.ChatRoomId == chatRoom.ChatRoomId ).FirstOrDefault();
            if (existingChatRoom == null) 
            {
                ControlChatRoom newControlChatRoom = new ControlChatRoom()
                {
                    ControlActionType = ControlActionType.Create,
                    ChatRoomObject = chatRoom,
                };
                _allCreatedChatRooms.Add(newControlChatRoom);

                _chatRoomUpdateCallback(_allCreatedChatRooms);
            }            
        }

        public List<ControlChatRoom> GetAllCreatedChatRooms()
        {
            return _allCreatedChatRooms;
        }
              

        public void RemoveAllChatRooms()
        {
            foreach(ControlChatRoom controlChatRoom in _allCreatedChatRooms)
            {
                controlChatRoom.ControlActionType = ControlActionType.Delete;
            }

            _chatRoomUpdateCallback(_allCreatedChatRooms);
        }

        public bool UpdateInvitedGuestServerUserInChatRoom(Guid chatRoomId, InviteStatus inviteStatus, ServerUser serverUser)
        {
            bool chatRoomIsUpdated = false;
            ControlChatRoom controlChatRoomForUpdate = _allCreatedChatRooms.Where(a => a.ChatRoomObject.ChatRoomId == chatRoomId).FirstOrDefault();
            if (controlChatRoomForUpdate == null) { return chatRoomIsUpdated; }

            Invite targetInfiveInfo = controlChatRoomForUpdate.ChatRoomObject.AllInvitesSentToGuestUsers.Where(a=>a.GuestServerUser.ServerUserID == serverUser.ServerUserID).FirstOrDefault();
            if (targetInfiveInfo == null) { return chatRoomIsUpdated; }
            targetInfiveInfo.InviteStatus = inviteStatus;

            var targetGuestServerUser = controlChatRoomForUpdate.ChatRoomObject.AllActiveUsersInChatRoom.Where(a => a.ServerUserID == serverUser.ServerUserID).FirstOrDefault();           
            switch (inviteStatus)
            {
                case InviteStatus.Accepted:
                    if(targetGuestServerUser == null)
                    {
                        controlChatRoomForUpdate.ChatRoomObject.AllActiveUsersInChatRoom.Add(serverUser);
                        chatRoomIsUpdated = true;
                    }
                    break;

                    case InviteStatus.Rejected:
                    if(targetGuestServerUser != null)
                    {
                        controlChatRoomForUpdate.ChatRoomObject.AllActiveUsersInChatRoom.Remove(serverUser);
                        chatRoomIsUpdated = true;
                    }
                    break;
            }
            controlChatRoomForUpdate.ControlActionType = ControlActionType.Update;
            _chatRoomUpdateCallback(_allCreatedChatRooms);

            return true;
        }

        public bool RemoveUserFromAllActiveUsersInChatRoom(Guid targetChatRoomId, Guid serverUserId)
        {
            bool actionCompleted = false;
            ControlChatRoom selectedControlChatRoom = _allCreatedChatRooms.Where(a => a.ChatRoomObject.ChatRoomId == targetChatRoomId).FirstOrDefault();
            if (selectedControlChatRoom == null) { return actionCompleted; }
            ServerUser serverUserForDeletion = selectedControlChatRoom.ChatRoomObject.AllActiveUsersInChatRoom.Where(a=>a.ServerUserID == serverUserId).FirstOrDefault();
            if(serverUserForDeletion != null)
            {
                selectedControlChatRoom.ChatRoomObject.AllActiveUsersInChatRoom.Remove(serverUserForDeletion);
                if(selectedControlChatRoom.ChatRoomObject.AllActiveUsersInChatRoom.Count <= 0)
                {
                    selectedControlChatRoom.ChatRoomObject.ChatRoomStatus = ChatRoomStatus.Closed;
                }
                actionCompleted = true;
            }
            selectedControlChatRoom.ControlActionType = ControlActionType.Update;
            _chatRoomUpdateCallback(_allCreatedChatRooms);
            return actionCompleted;
        }

        public bool RemoveUserFromAllChatRooms(Guid serverUserId)
        {
            List<ControlChatRoom> allActiveControlChatRooms = _allCreatedChatRooms.Where(a =>a.ChatRoomObject.ChatRoomStatus == ChatRoomStatus.OpenActive).ToList();
            if (allActiveControlChatRooms.Count == 0) { return false; }

            foreach(ControlChatRoom activeChatRoom in allActiveControlChatRooms)
            {
                ServerUser userForRemoval = activeChatRoom.ChatRoomObject.AllActiveUsersInChatRoom.Where(a=>a.ServerUserID == serverUserId).FirstOrDefault();
                if (userForRemoval != null) 
                {
                    activeChatRoom.ChatRoomObject.AllActiveUsersInChatRoom.Remove(userForRemoval);
                    if(activeChatRoom.ChatRoomObject.AllActiveUsersInChatRoom.Count <= 0)
                    {
                        activeChatRoom.ChatRoomObject.ChatRoomStatus= ChatRoomStatus.Closed;
                    }
                    activeChatRoom.ControlActionType = ControlActionType.Update;
                }
            }

            _chatRoomUpdateCallback (_allCreatedChatRooms);
            return true;
        }

        public bool RecordMessageInChatRoomConversation(Guid chatRoomId, string message)
        {
            ControlChatRoom selectedControlChatRoom = _allCreatedChatRooms.Where(a => a.ChatRoomObject.ChatRoomId == chatRoomId).FirstOrDefault();
            if (selectedControlChatRoom != null)
            {
                selectedControlChatRoom.ChatRoomObject.ConversationRecord += CRLF+ message;
                selectedControlChatRoom.ControlActionType = ControlActionType.Update;

                _chatRoomUpdateCallback(_allCreatedChatRooms);
                return true;
            }

            return false;
        }
    }
}
