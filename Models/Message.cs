using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatProject.Models
{
    public class Message
    {
        public Message(string text, string author, string connectionId, DateTime dateTime)
        {
            Text = text;
            Author = author;
            ConnectionId = connectionId;
            DateTime = dateTime;
        }

        public string Text { get; set; }
        public string Author { get; set; }
        public string ConnectionId { get; set; }
        public DateTime DateTime { get; set; }
    }
}
