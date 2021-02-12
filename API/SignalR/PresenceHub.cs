using System;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;
        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }


        // when a client connects uypdate presence tracker, send updated list of users to everyone connected
        public override async Task OnConnectedAsync()
        {
            // add user to dictionary
            var isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
            // send messages to all clients other than the one who triggered
            if (isOnline)  await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());            

            // get list of current users
            var currentUsers = await _tracker.GetOnlineUsers();
            // send list of online users to caller
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var isOffline = await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
            // notify other users when they go offline
            if (isOffline) await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

            //if exception pass up to parent
            await base.OnDisconnectedAsync(exception);
        }
    }
}