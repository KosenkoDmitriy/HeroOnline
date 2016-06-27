using LOLServices.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOLServices.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public List<int> UserRights { get; set; }

        public UserModel(int userId, string userName)
        {
            UserId = userId;
            UserName = userName;
            UserRights = new List<int>();
        }

        public static UserModel SignIn(string username, string password)
        {
            using (MainDBDataContext dbContext = new MainDBDataContext())
            {
                Account theAcc = dbContext.Accounts.FirstOrDefault(acc => acc.UserName == username);
                if (theAcc == null)
                    return new UserModel(-1, "<Invalid Account>");

                if (theAcc.Password != Common.GetHashString(password, "MD5"))
                    return new UserModel(-2, "<Invalid Password>");

                UserModel model = new UserModel(theAcc.UserId, theAcc.UserName) { NickName = theAcc.NickName };
                foreach (var auth in theAcc.Auths)
                    model.UserRights.Add(auth.ActCode);

                return model;
            }
        }
    }
}
