﻿using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Utils.Enumerations;
using ChatRoomServer.Utils.Interfaces;

namespace ChatRoomServer.DomainLayer
{
    public class ChatRoomManager : IChatRoomManager
    {
        private List<ChatRoom> _allCreatedChatRooms;

        IObjectCreator _objectCreator;
        public ChatRoomManager(IObjectCreator objectCreator)
        {
            _allCreatedChatRooms = new List<ChatRoom>();
            _objectCreator = objectCreator;

        }

        public ChatRoom CreateChatRoom(ChatRoom chatRoomFromServerUser)
        {
            ChatRoom chatRoomCreated = _objectCreator.CreateChatRoom(chatRoomFromServerUser.ChatRoomName, chatRoomFromServerUser.Creator, chatRoomFromServerUser.AllActiveUsersInChatRoom, chatRoomFromServerUser.AllInvitesSentToGuestUsers);
            AddChatRoomToAllChatRooms(chatRoomCreated);

            return chatRoomCreated;
        }

        public void AddChatRoomToAllChatRooms( ChatRoom chatRoom)
        {
            _allCreatedChatRooms.Add(chatRoom);
        }

        public List<ChatRoom> GetAllCreatedChatRooms()
        {
            return _allCreatedChatRooms;
        }

        public bool UpdateChatRoomStatus(Guid chatRoomId , ChatRoomStatus chatRoomStatus)
        {
            ChatRoom chatRoomForUpdate = _allCreatedChatRooms.Where(a => a.ChatRoomId == chatRoomId).FirstOrDefault();
            if (chatRoomForUpdate != null)
            {
                chatRoomForUpdate.ChatRoomStatus = chatRoomStatus;
                return true;
            }

            return false;
        }

        public bool AddActiveUserToChatRoom(Guid chatRoomId, ServerUser serverUser)
        {
            var selectedChatRoom = _allCreatedChatRooms.Where(a=>a.ChatRoomId == chatRoomId).FirstOrDefault();
            if (selectedChatRoom != null) 
            { 
                selectedChatRoom.AllActiveUsersInChatRoom.Add(serverUser);
                return true;
            }

            return false;
        }

        public bool RemoveUserFromChatRoom(Guid chatRoomId, ServerUser serverUser)
        {
            var selectedChatRoom = _allCreatedChatRooms.Where(a => a.ChatRoomId == chatRoomId).FirstOrDefault();
            if (selectedChatRoom != null)
            {
                selectedChatRoom.AllActiveUsersInChatRoom.Remove(serverUser);
                return true;
            }

            return false;
        }

        public bool UpdateInviteStatusInChatRoom(Guid chatRoomId, Guid inviteId, InviteStatus inviteStatus)
        {
            ChatRoom selectedChatRoom = _allCreatedChatRooms.Where(a =>a.ChatRoomId == chatRoomId).FirstOrDefault();
            if (selectedChatRoom != null)
            {
                Invite selectedInvite = selectedChatRoom.AllInvitesSentToGuestUsers.Where(a => a.InviteId == inviteId).FirstOrDefault(); ;
                if(selectedInvite != null) 
                {
                    selectedInvite.InviteStatus = inviteStatus;
                    return true;
                }
            }

            return false;
        }

        public bool RecordMessageInChatRoom(Guid chatRoomId, ServerUser serverUser, string message)
        {
            ChatRoom selectedChatRoom = _allCreatedChatRooms.Where(a => a.ChatRoomId == chatRoomId).FirstOrDefault();
            if (selectedChatRoom != null)
            {
                string userMessage = serverUser.Username + ": " + message;
                selectedChatRoom.ConversationRecord += userMessage;
                return true;
            }

            return false;
        }
    }
}
