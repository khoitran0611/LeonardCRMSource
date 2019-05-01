using System;
using System.Linq;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.CommonRepository
{
    public sealed class LoginLogDA : EF5RepositoryBase<LeonardUSAEntities, Eli_LoginLog>
    {

        private static volatile LoginLogDA _instance;
        private static readonly object SyncRoot = new Object();
        public static LoginLogDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new LoginLogDA();
                    }
                }

                return _instance;
            }
        }

        private LoginLogDA() : base(Settings.ConnectionString) { }

        public int Insert(Eli_OnlineUsers user)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                context.Eli_OnlineUsers.Add(user);
                return context.SaveChanges();
            }
        }

        public Eli_OnlineUsers GetLoggedInUser(int id)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.Eli_OnlineUsers.FirstOrDefault(u => u.UserID == id);
            }
        }

        public Eli_OnlineUsers GetLoggedInLogByToken(string headerToken)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.Eli_OnlineUsers.FirstOrDefault(u => u.AccessToken == headerToken);
            }
        }
    }
}
