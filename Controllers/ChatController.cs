using ChatProject.Hubs;
using ChatProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> hubContext;

        public ChatController(IHubContext<ChatHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        [HttpPost]
        public async Task SendMessage(Message message)
        {
            //additional business logic 

            await this.hubContext.Clients.All.SendAsync("messageReceivedFromApi", message);

            //additional business logic 
        }
    }
}
