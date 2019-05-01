using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.UserRepository
{
    public sealed class UserDA : EF5RepositoryBase<LeonardUSAEntities, Eli_User>
    {
        private static volatile UserDA _instance;
        private static readonly object SyncRoot = new Object();

        public static UserDA Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (SyncRoot)
                {
                    if (_instance == null)
                        _instance = new UserDA();
                }
                return _instance;
            }
        }

        private UserDA() : base(Settings.ConnectionString) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Eli_User GetByIdWithRoles(int userId)
        {
            return SingleLoadWithReferences(record => record.Id == userId, "Eli_Roles");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Eli_User CheckLogin(string username, string password)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var hdUser = context.Eli_User.Include("Eli_Roles").SingleOrDefault(record => record.Email.Equals(username) &&
                                                                    record.Password.Equals(password));
                if (hdUser != null && hdUser.ActivationCode == null)
                {
                    hdUser.ActivationCode = Guid.NewGuid();
                    context.SaveChanges();
                }
                return hdUser;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="fullname"></param>
        /// <param name="email"></param>
        /// <param name="onlyActive"></param>
        /// <returns></returns>
        public IList<Eli_User> Filter(int roleId, string fullname, string email, bool onlyActive)
        {
            const int status = (int)UserStatus.Active;
            return onlyActive
                       ? SelectWithPaging(records => records.Eli_Roles.Id == roleId &&
                                                     records.Name.Contains(fullname) &&
                                                     records.Status == status &&
                                                     records.Email.Contains(email), null, null,
                                          record => record.CreatedDate, false)
                       : SelectWithPaging(records => records.Eli_Roles.Id == roleId &&
                                                     records.Name.Contains(fullname) &&
                                                     records.Email.Contains(email), null, null,
                                          record => record.CreatedDate, false);
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
        //public IList<sp_SearchUsers_Result> SearchUsers(int? roleId, string fullname, string email, bool onlyActive, string sortExpression, int? pageIndex, int? pageSize, out int totalRow)
        //{
        //    using (_context = new LeonardEntities(Settings.ConnectionString))
        //    {
        //        var total = new ObjectParameter("totalRow", typeof(int));
        //        var entities = _context.sp_SearchUsers(roleId, fullname, email, onlyActive,
        //                                                                        sortExpression, pageIndex, pageSize, total).ToList();
        //        totalRow = int.Parse(total.Value.ToString());
        //        return entities;
        //    }
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int Insert(Eli_User entity, int roleId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.sp_CreateUser(entity.Name, entity.Email, entity.LoginName, entity.Password, entity.Status, entity.Phone, entity.CreatedBy, entity.ModifiedBy, roleId, entity.ActivationCode, entity.DateOfBirth);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int UpdateWithRole(Eli_User entity, int roleId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var dbSet = context.Set<Eli_User>();
                dbSet.Attach(entity);

                var entityGroup = entity.Eli_Roles;
                if (entityGroup.Id != roleId)
                {
                    entityGroup = context.Eli_Roles.SingleOrDefault(record => record.Id == roleId);
                    entity.Eli_Roles = entityGroup;
                }
                context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                return context.SaveChanges();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int DeleteUsers(int userId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var entity = context.Eli_User.SingleOrDefault(record => record.Id == userId);
                context.Eli_User.Attach(entity);
                context.Eli_User.Remove(entity);
                return context.SaveChanges();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int DeleteUsers(IList<Eli_User> entities)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                foreach (var entity in entities)
                {
                    context.Eli_User.Attach(entity);
                    context.Eli_User.Remove(entity);
                }
                return context.SaveChanges();
            }
        }

        public int Delete_Users(string ids, out string outNames)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var denyString = new ObjectParameter("denyStr", typeof(string));
                var status = context.sp_DeleteUsers(ids, denyString);
                outNames = denyString.Value.ToString();
                return status;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>

        public bool ExistEmail(string email)
        {
            var hdUser = Single(record => record.Email == email);
            return hdUser != null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="activeCode"></param>
        /// <returns></returns>

        public Eli_User GetUserByCode(string activeCode)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var entity = context.Eli_User.Include("Eli_Roles")
                                        .SingleOrDefault(record => record.ActivationCode == new Guid(activeCode));
                return entity;
            }
        }

        public IList<Eli_User> GetUserForDropDown()
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var query = from u in context.Eli_User
                            select new
                                {
                                    u.Id,
                                    u.Name
                                };
                return query.ToList().Select(x => new Eli_User() { Id = x.Id, Name = x.Name }).ToList();
            }
        }

        public string GetStoreEmailList(int storeId, bool includeManagerEmail)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var storeNumber = storeId.ToString();
                var query = from u in context.Eli_User
                            join l in context.Eli_ListValues on u.StoreId equals l.Id
                            into a
                            from b in a.DefaultIfEmpty()
                            where (b.AdditionalInfo == storeNumber && u.RoleId == (int)UserRoles.Store) || (includeManagerEmail && u.RoleId == (int)UserRoles.ContractManager)
                            select u.Email;
                var emails = String.Join(",", query.ToList());
                return emails;
            }
        }

        public string GetManagerEmail(int[] userIds)
        {
            return GetUserEmailByRole(userIds, UserRoles.ContractManager.GetHashCode());
        }
        
        public string GetDeliveryEmail(int[] userIds)
        {
            return GetUserEmailByRole(userIds, UserRoles.DeliveryStaff.GetHashCode());
        }

        private string GetUserEmailByRole(int[] userIds, int role)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var query = from u in context.Eli_User
                            join l in context.Eli_ListValues on u.StoreId equals l.Id
                            into a
                            from b in a.DefaultIfEmpty()
                            where u.RoleId == role && userIds.Contains(u.Id)
                            select u.Email;
                var emails = string.Join(",", query.ToList());
                return emails;
            }
        }


        public string GetDeliveryEmailList(int appId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var app = context.SalesOrders.SingleOrDefault(x => x.Id == appId);
                var responsibleUsers = (!string.IsNullOrEmpty(app.ResponsibleUsers) ? app.ResponsibleUsers : "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x=> int.Parse(x)).ToList();
                var deliveryRole = UserRoles.DeliveryStaff.GetHashCode();

                var deliveryEmails = context.Eli_User.Where(x => responsibleUsers.Contains(x.Id) && x.RoleId == deliveryRole).Select(user => user.Email).ToArray();

                return string.Join(",", deliveryEmails);
            }
        }

        /// <summary>
        /// Getting a list of responsible users of submitted store including contract manager users
        /// </summary>
        /// <param name="storeId">The store ID which the application submitted to</param>
        /// <returns>List of user IDs</returns>
        public int[] GetResponsibleListForStore(int? storeId, bool isSoldProcess)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var storeNumber = storeId.ToString();
                return (from u in context.Eli_User.AsNoTracking()
                        join l in context.Eli_ListValues on u.StoreId equals l.Id
                        into a
                        from b in a.DefaultIfEmpty()
                        where (storeNumber != null && storeNumber != "" && b.AdditionalInfo == storeNumber && u.RoleId == (int)UserRoles.Store) || (!isSoldProcess && u.RoleId == (int)UserRoles.ContractManager)
                        select u.Id).ToArray();
            }
        }

        public IList<vwUserRole> GetAllUserGroup(int currentRole, int userId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var roles = context.fn_GetRolesHierachy(currentRole);
                if (roles.Count() > 1)
                    return context.vwUserRoles.AsNoTracking().Where(r => roles.Contains(r.RoleId)).ToList();
                return context.vwUserRoles.AsNoTracking().Where(r => r.Id == userId).ToList();
            }
        }

        public IList<vwUserRole> GetAllUserGroup()
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                return context.vwUserRoles.AsNoTracking().ToList();
            }
        }

        public int GoOnline(string token, int userId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                //Case: Token existed User Online table
                var loginInfo = context.Eli_OnlineUsers.FirstOrDefault(r => r.AccessToken == token);
                if (loginInfo != null)
                {
                    loginInfo.LoggedTime = DateTime.Now;
                    context.Entry(loginInfo).State = System.Data.Entity.EntityState.Modified;
                    return context.SaveChanges();
                }

                //Case: Token not exist User Online table
                var userInformation = context.Eli_User.SingleOrDefault(r => r.Id == userId);
                if (userInformation == null) return 0;
                loginInfo = new Eli_OnlineUsers
                {
                    LoggedTime = DateTime.Now,
                    AccessToken = token,
                    UserID = userId
                };
                context.Eli_OnlineUsers.Add(loginInfo);
                return context.SaveChanges();
            }
        }

        public IList<ResponsibleUserModel> GetResponsibleUserForApp()
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var customerRole = UserRoles.Customer.GetHashCode();
                var users = context.Eli_User.Include("Eli_Roles")
                                            .Where(x => x.RoleId != customerRole)
                                            .Select(u => new ResponsibleUserModel()
                {
                    Id = u.Id,
                    Description = u.Name,
                    Role = u.Eli_Roles.Name,
                    RoleId = u.RoleId
                });
                return users.ToList();
            }
        }

        public Eli_User GetFirstUserByStore(int storeId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var storeNumber = storeId.ToString();

                var query = from user in context.Eli_User
                            join l in context.Eli_ListValues on user.StoreId equals l.Id
                            into a
                            from b in a.DefaultIfEmpty()
                            where b.AdditionalInfo == storeNumber && (user.RoleId == (int)UserRoles.Store || user.RoleId == (int)UserRoles.ContractManager || user.RoleId == (int)UserRoles.Administrator)
                            select user;

                return query.FirstOrDefault();
            }
        }
    }
}
