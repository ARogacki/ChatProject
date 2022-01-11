using ChatProject.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatProject.Hubs
{

    public class RoomContainer
    {
        private RoomContainer() { }

        private static RoomContainer _instance;

        private static readonly object _lock = new object();

        public RoomHandler roomHandler;

        public static RoomContainer GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new RoomContainer();
                        _instance.roomHandler = new RoomHandler();
                    }
                }
            }
            return _instance;
        }
    }

    public class ChatHub : Hub<IChatHub>
    {
        RoomHandler roomHandler;
        public async Task BroadcastAsync(Message message)
        {
            await Clients.All.MessageReceivedFromHub(message);
        }

        public async Task SendMessageToGroup(Message message)
        {
            try
            {
                PrepareHandler();
                //await Clients.All.NewUserConnected("info: " + roomHandler.rooms.Count);
                KeyValuePair<string, Room> room = roomHandler.GetRoomFromUser(Context.ConnectionId);
                room.Value.AddToHistory(message);
                await Clients.Group(room.Key).SendAsync(message);
            }
            catch (Exception e)
            {
                await Clients.Client(Context.ConnectionId).SendError(e.Message);
            }
        }

        public async Task JoinRoom(string room, string username, Message message)
        {
            try
            {
                PrepareHandler();
                User user = new User(Context.ConnectionId, username, "Just joined.");
                await JoinRoomByUser(room, user, message);
            }
            catch (Exception e)
            {
                await Clients.Client(Context.ConnectionId).SendError(e.Message);
            }
        }

        public async Task JoinRoomByUser(string room, User user, Message message)
        {
            try
            {
                roomHandler.AddUserToRoom(room, user);
                await Groups.AddToGroupAsync(Context.ConnectionId, room);

                await Clients.Client(Context.ConnectionId).ReceiveHistory(roomHandler.rooms[room].GetHistory());
                roomHandler.rooms[room].AddToHistory(message);
                await Clients.Group(room).SendAsync(message);
                await Clients.Group(room).ReceiveUsers(roomHandler.rooms[room].GetUsers());
                await Clients.Group(room).NewUserConnected(message.Text);
            }
            catch (Exception e)
            {
                await Clients.Client(Context.ConnectionId).SendError(e.Message);
            }
        }


        public async Task ChangeState(string state)
        {
            try
            {
                PrepareHandler();
                KeyValuePair<string, Room> room = roomHandler.GetRoomFromUser(Context.ConnectionId);
                roomHandler.SetUserStateInRoom(room.Key, Context.ConnectionId, state);
                await Clients.Group(room.Key).ChangeState(state);
                await Clients.Group(room.Key).ReceiveUsers(roomHandler.rooms[room.Key].GetUsers());
            }
            catch (Exception e)
            {
                await Clients.Client(Context.ConnectionId).SendError(e.Message);
            }
        }

        public async Task LeaveRoom()
        {
            try
            {
                PrepareHandler();
                KeyValuePair<string, Room> room = roomHandler.GetRoomFromUser(Context.ConnectionId);
                User user = room.Value.GetUserById(Context.ConnectionId);
                roomHandler.RemoveUserFromRoom(room.Key, Context.ConnectionId);
                if (roomHandler.rooms.ContainsKey(room.Key))
                {
                    await Clients.Group(room.Key).ReceiveUsers(roomHandler.rooms[room.Key].GetUsers());
                    await Clients.Group(room.Key).SendAsync(new Message(user.Username + " disconnected.", user.Username, user.ConnectionId, DateTime.Now));
                    await Clients.Group(room.Key).UserDisconnected("disconnected");
                }
            }
            catch (Exception e)
            {
                await Clients.Client(Context.ConnectionId).SendError(e.Message);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            PrepareHandler();
            await LeaveRoom();
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                await Clients.Client(Context.ConnectionId).SuccessfullyConnected("Successfully connected");
            }
            catch (Exception e)
            {
                await Clients.Client(Context.ConnectionId).SendError(e.Message);
            }
        }

        private void PrepareHandler()
        {
            RoomContainer roomContainer = RoomContainer.GetInstance();
            roomHandler = roomContainer.roomHandler;
        }
    }

    public interface IChatHub
    {
        Task ReceiveHistory(List<Message> messages);

        Task MessageReceivedFromHub(Message message);

        Task ChangeState(string message);

        Task NewUserConnected(string message);

        Task UserDisconnected(string message);

        Task SuccessfullyConnected(string message);

        Task SendAsync(Message message);

        Task ReceiveUsers(List<User> users);

        Task SendError(string message);
    }
}
