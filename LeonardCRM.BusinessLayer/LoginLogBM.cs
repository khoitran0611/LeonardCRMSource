using System;
using LeonardCRM.DataLayer.CommonRepository;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class LoginLogBM : BusinessBase<IRepository<Eli_LoginLog>, Eli_LoginLog>
    {
        private static volatile LoginLogBM _instance;
        private static readonly object SyncRoot = new Object();
        public static LoginLogBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new LoginLogBM();
                    }
                }

                return _instance;
            }
        }
        private LoginLogBM()
            : base(LoginLogDA.Instance)
        { }

        public int Insert(Eli_OnlineUsers user)
        {
            return LoginLogDA.Instance.Insert(user);
        }

        public Eli_OnlineUsers GetLoggedInUser(int id)
        {
            return LoginLogDA.Instance.GetLoggedInUser(id);
        }

        public Eli_OnlineUsers GetLoggedInLogByToken(string headerToken)
        {
            return LoginLogDA.Instance.GetLoggedInLogByToken(headerToken);
        }
    }
}
