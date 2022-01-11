using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatProject.Models
{
    public class Room
    {
        List<Message> History = new List<Message>();
        List<User> Users = new List<User>();

        public void AddToHistory(Message message)
        {
            History.Add(message);
        }

        public void RemoveFromHistory(Message message)
        {
            History.Remove(message);
        }

        public void ResetHistory()
        {
            History = new List<Message>();
        }

        public List<Message> GetHistory()
        {
            return History;
        }

        public void AddToUsers(User user)
        {
            Users.Add(user);
        }

        public void RemoveFromUsers(User user)
        {
            Users.Remove(user);
        }

        public List<User> GetUsers()
        {
            return Users;
        }

        private User GetUserByIdString(string id)
        {
            foreach(User user in Users)
            {
                if(user.ConnectionId == id)
                {
                    return user;
                }
            }
            return null;
        }

        public void SetUserState(User user, string state)
        {
            Users.First(temp => temp == user).State = state;
        }

        public User GetUserById(string id)
        {
            return GetUserByIdString(id);
        }

        public void ResetUsers()
        {
            Users = new List<User>();
        }
    }
}
