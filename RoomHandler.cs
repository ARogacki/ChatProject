using ChatProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatProject
{
    public class RoomHandler
    {
        public Dictionary<string, Room> rooms = new Dictionary<string, Room>();

        public void AddRoom(string name)
        {
            rooms[name] = new Room();
        }

        public void RemoveRoom(string name)
        {
            rooms.Remove(name);
        }

        public void AddUserToRoom(string room, User user)
        {
            if (!rooms.ContainsKey(room))
            {
                AddRoom(room);
            }
            rooms[room].AddToUsers(user);
        }

        public void AddUserToRoom(string room, string userId, string username, string state)
        {
            User user = new User(userId, username, state);
            if (!rooms.ContainsKey(room))
            {
                AddRoom(room);
            }
            rooms[room].AddToUsers(user);
        }

        public void RemoveUserFromRoom(string room, string userId)
        {
            User user = rooms[room].GetUserById(userId);
            if(user != null)
                rooms[room].RemoveFromUsers(user);
            if(rooms[room].GetUsers().Count < 1)
            {
                RemoveRoom(room);
            }
        }

        public void RemoveUserFromAllRooms(string userId)
        {
            foreach(var room in rooms)
            {
                User temp = room.Value.GetUserById(userId);
                if(temp != null)
                {
                    room.Value.RemoveFromUsers(temp);
                }
            }
        }

        public KeyValuePair<string, Room> GetRoomFromUser(string userId)
        {
            foreach (var room in rooms)
            {
                User temp = room.Value.GetUserById(userId);
                if (temp != null)
                {
                    return room;
                }
            }
            return new KeyValuePair<string, Room>();
        }

        public List<User> GetUserListFromRoom(string room)
        {
            return rooms[room].GetUsers();
        }

        public List<Message> GetHistoryFromRoom(string room)
        {
            return rooms[room].GetHistory();
        }

        public void ClearHistoryInRoom(string room)
        {
            rooms[room].ResetHistory();
        }

        public void ResetRoom(string room)
        {
            rooms[room].ResetHistory();
            rooms[room].ResetUsers();
        }

        public string GetUserStateFromRoomById(string room, string userId)
        {
            User user = rooms[room].GetUserById(userId);
            if (user != null)
                return user.State;
            else
                return "";
        }

        public void SetUserStateInRoom(string room, string userId, string state)
        {
            User user = rooms[room].GetUserById(userId);
            if (user != null)
                rooms[room].SetUserState(user, state);
        }

    }
}
