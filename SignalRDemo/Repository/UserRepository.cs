using System;
using System.Collections.Generic;
 

namespace SignalRDemo.Repository
{
    using System.Linq;
    using SignalRDemo.Models;
    using User = System.Web.Providers.Entities.User;

    public class UserRepository : BaseRepository
    {
        public List<SignalRDemo.Models.User> GetAvailableUser()
        {
            return this.DataContext.Users.Where(t => t.isOnline == true).ToList();
        }

        public bool IsUserExist(string userName)
        {
            return this.DataContext.Users.Any(t => t.UserName == userName);
        }

        public Models.User FindByUserbyName(string userName)
        {
            return this.DataContext.Users.FirstOrDefault(u => u.UserName.Trim() == userName.Trim());
        }

        public bool AddUser(Models.User newUser)
        {
            this.DataContext.Users.Add(newUser);
            this.SaveData();

            return true;

        }

        public bool UpdateUserStatus(string userName,  string connectionId, bool isOnline)
        {
            var existingUser = this.DataContext.Users.FirstOrDefault(u => u.UserName == userName);
            if (existingUser != null)
            {
                existingUser.isOnline = isOnline;
                existingUser.ConnectionId = connectionId;
                this.SaveData();
            }
            
            return true;

        }


        private void SaveData()
        {
            try
            {
                // save data
                this.DataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    
}