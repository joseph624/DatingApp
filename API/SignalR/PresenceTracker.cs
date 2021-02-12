using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    // shared with service so all users have access
    public class PresenceTracker
    {
        // <username, List<ConnectionIds>
        // track who is connected - get connection information rather than getting from other server
        private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();

        // add users who connected
        public Task<bool> UserConnected(string username, string connectionId)
        {

            bool isOnline = false;
            // lock dictionary to prevent multiple logins 
            lock (OnlineUsers) 
            {
                // check if there is already a user in dictionary
                if (OnlineUsers.ContainsKey(username))
                {
                    // access dicitonary with key of username & add connection ID to list
                    OnlineUsers[username].Add(connectionId);
                }
                else 
                {
                    // if no user
                    OnlineUsers.Add(username, new List<string>{connectionId});
                    isOnline = true;
                }
            }
            return Task.FromResult(isOnline);
        }

        // when user is disconected 
        public Task<bool> UserDisconnected(string username, string connectionId)
        {
            bool isOffline = false;
            lock(OnlineUsers){
                // check if dictionary element is in dictionary
                if (!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

                OnlineUsers[username].Remove(connectionId);
                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                    isOffline = true;
                }
            }

            return Task.FromResult(isOffline);
        }

        // get list of users connected
        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock(OnlineUsers)
            {
                // if user is connected from one of their devices then they are online
                // store in memory not database
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }

        // get list of connections for a paticular user
        public Task<List<string>> GetConnectionsForUser(string username) 
        {
            // lock dictionary
            List<string> connectionIds;
            lock(OnlineUsers)
            {   
                // if we have a connection id for that user this will return a list of connection ids
                connectionIds = OnlineUsers.GetValueOrDefault(username);
            }

            return Task.FromResult(connectionIds);
        }
    }
}