using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SignalRDemo
{
    using SignalRDemo.Models;
    using SignalRDemo.Repository;
    using SignalRDemo.ViewModel;

    public class ChatHub : Hub
    {
        private UserRepository userRepository = new UserRepository();
        private MessageRepository messageRepository = new MessageRepository();

        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }


        #region Data Members

        static List<UserDetail> ConnectedUsers = new List<UserDetail>();
        static List<MessageDetail> CurrentMessage = new List<MessageDetail>();

        #endregion

        #region Methods

        public void Connect(string userName)
        {
            var id = Context.ConnectionId;


            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {
                // Get list of online users
                var onlineUsers = this.userRepository.GetAvailableUser();
                foreach (var userDetail in onlineUsers)
                {
                    if (!ConnectedUsers.Any(c => c.UserName.ToLowerInvariant() == userDetail.UserName.ToLowerInvariant()))
                    {
                        ConnectedUsers.Add(new UserDetail { ConnectionId = id, UserName = userDetail.UserName });
                    }
                }

                ConnectedUsers.Where(p => p.UserName.ToLowerInvariant() == userName.ToLowerInvariant()).Select(u => { u.ConnectionId = id; return u; }).ToList();

                if (!ConnectedUsers.Any(c => c.UserName.ToLowerInvariant() == userName.ToLowerInvariant()))
                {
                    ConnectedUsers.Add(new UserDetail { ConnectionId = id, UserName = userName });
                }

                var latestMessages = this.messageRepository.GetLatestMessages();
                CurrentMessage.Clear();
                foreach (var msg in latestMessages)
                {
                    //if (!CurrentMessage.Any(m => m.MessageId == msg.MessageId))
                    //{
                        CurrentMessage.Add(new MessageDetail { Message = msg.Message, UserName = msg.UserName, MessageId = msg.MessageId });
                    //}
                }

                // send to caller
                Clients.Caller.onConnected(id, userName, ConnectedUsers, CurrentMessage);

                // send to all except caller client
                Clients.AllExcept(id).onNewUserConnected(id, userName);

                if (!this.userRepository.IsUserExist(userName))
                {
                    var user = new User { UserName = userName, ConnectionId = id, isOnline = true };
                    this.userRepository.AddUser(user);
                }
                else
                {
                    this.userRepository.UpdateUserStatus(userName, id, true);
                }
            }

        }

        public void Disconnect()
        {
            this.OnDisconnected(true);
        }

        public void SendMessageToAll(string userName, string message)
        {
            // store last 100 messages in cache
            AddMessageinCache(userName, message);

            // Broad cast message
            Clients.All.messageReceived(userName, message);
            Clients.All.broadcastMessage(userName, message);

            var user = this.userRepository.FindByUserbyName(userName);
            if (user != null)
            {
                Message newMessage = new Message();
                newMessage.Message1 = message;
                newMessage.UserId = user.UserId;
                this.messageRepository.AddMessages(newMessage);
            }
        }

        public void SayWhoIsTyping(string html)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            context.Clients.All.sayWhoIsTyping(html);
        }

        public void IsTyping(string html)
        {
            //call the function to send the html to the other clients
            SayWhoIsTyping(html);
        }



        public void SendPrivateMessage(string toUserId, string message)
        {

            string fromUserId = Context.ConnectionId;

            var toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == toUserId);
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

            if (toUser != null && fromUser != null)
            {
                // send to 
                Clients.Client(toUserId).sendPrivateMessage(fromUserId, fromUser.UserName, message);

                // send to caller user
                Clients.Caller.sendPrivateMessage(toUserId, fromUser.UserName, message);
            }

        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                ConnectedUsers.Remove(item);

                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.UserName);

                this.userRepository.UpdateUserStatus(item.UserName, id, false);
            }

            return base.OnDisconnected(stopCalled);
        }


        #endregion

        #region private Messages

        private void AddMessageinCache(string userName, string message)
        {
            CurrentMessage.Add(new MessageDetail { UserName = userName, Message = message });

            if (CurrentMessage.Count > 100)
                CurrentMessage.RemoveAt(0);
        }

        #endregion
    }

    

    
}