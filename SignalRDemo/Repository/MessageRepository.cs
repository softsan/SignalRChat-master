using SignalRDemo.Models;
using SignalRDemo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRDemo.Repository
{
    public class MessageRepository : BaseRepository
    {
        /// <summary>
        /// This will get latest 15 messages
        /// </summary>
        /// <returns>last 15 messages</returns>
        public List<MessageViewModel> GetLatestMessages()
        {
            var q = this.DataContext.Messages
                .Join(DataContext.Users,
                m => m.UserId,
                u => u.UserId,
                (m, u) => new  MessageViewModel { MessageId = m.MessageId, Message = m.Message1, UserName  = u.UserName })
                .Take(15)
                .OrderByDescending(m => m.MessageId)
                .ToList();

            return q;
             // return this.DataContext.Messages.Take(15).OrderByDescending(m => m.MessageId).ToList();
        }

        public void AddMessages(Message messageToAdd)
        {
            this.DataContext.Messages.Add(messageToAdd);
            this.DataContext.SaveChanges();
        }
    }

   
}