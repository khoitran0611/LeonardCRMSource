using System;
using System.Collections.Generic;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using LeonardCRM.DataLayer.UserRepository;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class UserBM : BusinessBase<IRepository<Eli_User>, Eli_User>
    {
        private static volatile UserBM _instance;
        private static readonly object SyncRoot = new Object();

        public static UserBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new UserBM();
                    }
                }

                return _instance;
            }
        }
        private UserBM()
            : base(UserDA.Instance)
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int Insert(Eli_User entity, int roleId)
        {
            return UserDA.Instance.Insert(entity, roleId);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public Eli_User CheckLogin(string email, string pwd)
        {
            return UserDA.Instance.CheckLogin(email, pwd);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public Eli_User GetByIdWithRoles(int userid)
        {
            return UserDA.Instance.GetByIdWithRoles(userid);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="fullname"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public IList<Eli_User> Filter(int roleId, string fullname, string email, bool onlyActive)
        {
            return UserDA.Instance.Filter(roleId, fullname, email, onlyActive);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="fullname"></param>
        /// <param name="email"></param>
        /// <param name="onlyActive"></param>
        /// <param name="sortExpression"></param>
        /// <returns></returns>
        //public IList<sp_SearchUsers_Result> SearchUsers(int? roleId, string fullname, string email, bool onlyActive,
        //                                                string sortExpression, int? pageIndex, int? pageSize, out int totalRow)
        //{
        //    return UserDA.Instance.SearchUsers(roleId, fullname, email, onlyActive, sortExpression, pageIndex, pageSize, out totalRow);
        //}
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int Delete_Users(string ids, out string outNames)
        {
            return UserDA.Instance.Delete_Users(ids,out outNames);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int UpdateWithRole(Eli_User entity, int roleId)
        {
            return UserDA.Instance.UpdateWithRole(entity, roleId);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool ExistEmail(string email)
        {
            return UserDA.Instance.ExistEmail(email);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="activeCode"></param>
        /// <returns></returns>
        public int ActiveUser(Eli_User entity)
        {
            entity.Status = (int)UserStatus.Active;
            return UserDA.Instance.Update(entity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="activeCode"></param>
        /// <returns></returns>
        public Eli_User GetUserByCode(string activeCode)
        {
            return UserDA.Instance.GetUserByCode(activeCode);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Eli_User GetUserByEmail(string email)
        {
            return UserDA.Instance.Single(record => record.Email == email);
        }

        public int InsertUser(Eli_User eliUser)
        {
            return UserDA.Instance.Insert(eliUser);
        }

        public IList<Eli_User> GetUserForDropDown()
        {
            return UserDA.Instance.GetUserForDropDown();
        }

        public IList<vwUserRole> GetAllUserGroup(int currentRole, int userId)
        {
            return UserDA.Instance.GetAllUserGroup(currentRole, userId);
        }

        public IList<vwUserRole> GetAllUserGroup()
        {
            return UserDA.Instance.GetAllUserGroup();
        }

        public int GoOnline(string token, int userId)
        {
            return UserDA.Instance.GoOnline(token, userId);
        }

        public string GetStoreEmailList(int storeId, bool includeManagerEmail = false)
        {
            return UserDA.Instance.GetStoreEmailList(storeId, includeManagerEmail);
        }
        /// <summary>
        /// Getting a list of responsible users of submitted store including contract manager users
        /// </summary>
        /// <param name="storeId">The store ID which the application submitted to</param>
        /// <returns>List of user IDs</returns>
        public int[] GetResponsibleListForStore(int? storeId, bool isSoldProcess)
        {
            return UserDA.Instance.GetResponsibleListForStore(storeId, isSoldProcess);
        }

        public IList<ResponsibleUserModel> GetResponsibleUserForApp()
        {
            return UserDA.Instance.GetResponsibleUserForApp();
        }

        public string GetManagerEmail(int[] userIds)
        {
            return UserDA.Instance.GetManagerEmail(userIds);
        }

        public string GetDeliveryEmailList(int appId)
        {
            return UserDA.Instance.GetDeliveryEmailList(appId);
        }

        internal string GetDeliveryEmail(int[] userIds)
        {
            return UserDA.Instance.GetDeliveryEmail(userIds);
        }

        public Eli_User GetFirstUserByStore(int storeId)
        {
            return UserDA.Instance.GetFirstUserByStore(storeId);
        }
    }
}
