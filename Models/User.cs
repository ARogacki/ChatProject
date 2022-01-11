using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatProject.Models
{
    public class User
    {
        public User(string connectionId, string username, string state)
        {
            ConnectionId = connectionId;
            Username = username;
            State = state;
        }

        public string ConnectionId { get; set; }
        public string Username { get; set; }
        public string State { get; set; }
    }
}
