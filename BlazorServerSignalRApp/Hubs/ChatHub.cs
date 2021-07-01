using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerSignalRApp.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendToCallerMessage(string user, string message)
        {
            var reply = $"System Reviced Your Message: {message}";
            await Clients.Caller.SendAsync("ReceiveMessage", "", reply);
        }

        public async Task SendToGroupMessage(string user, string message, string groupName)
        {
            var reply = $"To Group {groupName}: {message}";
            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, reply);
        }

        public async Task AddToGroup(string user, string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, $"{user} has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string user, string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, $"{user} has left the group {groupName}.");
        }
    }
}
