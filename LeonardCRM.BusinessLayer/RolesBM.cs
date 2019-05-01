using System;
using System.Collections.Generic;
using System.Linq;
using Eli.Common;
using LeonardCRM.DataLayer.CommonRepository;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;
using EntityFieldDA = LeonardCRM.DataLayer.EntityFieldRepository.EntityFieldDA;

namespace LeonardCRM.BusinessLayer
{
    public sealed class RolesBM : BusinessBase<IRepository<Eli_Roles>, Eli_Roles>
    {
        private static volatile RolesBM _instance;
        private static readonly object SyncRoot = new Object();
        public static RolesBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new RolesBM();
                    }
                }

                return _instance;
            }
        }
        private RolesBM()
            : base(RolesDA.Instance)
        { }

        public IList<Eli_Roles> GetAllRoles(int currentRoleId)
        {
            return RolesDA.Instance.GetAllRoles(currentRoleId);
        }

        public int DeleteRoles(IList<Eli_Roles> entites)
        {
            return RolesDA.Instance.DeleteRoles(entites);
        }

        public Eli_Roles GetRolebyRoleId(int roleId)
        {
            return RolesDA.Instance.GetRolebyRoleId(roleId);
        }

        public int SaveRole(Eli_Roles entity)
        {
            var rolesFields = entity.Eli_RolesPermissions.SelectMany(p => p.EntityFields).ToList();
            var list = entity.Eli_RolesPermissions.Where(item => !item.AllowDelete && !item.AllowEdit && !item.AllowRead);
            entity.Eli_RolesPermissions = entity.Eli_RolesPermissions.Except(list).ToList();
            var status = RolesDA.Instance.SaveRole(entity, rolesFields);
            if (status > 0)
            {
                var cache = new CacheManager();
                cache.Remove(Constant.ViewColumnsCacheKey);
            }
            return status;
        }

        public Select2HierarchicalData GetUsersHierachy(int currentRole, int userId)
        {
            var result = new Select2HierarchicalData();
            var roles = RolesDA.Instance.GetRolesWithUserHierachy(currentRole,userId);
            result.results = roles.Select(role => new Select2HierarchicalData.OptionGroup()
                {
                    id = role.Id.ToString(),
                    text = role.Name,
                    children = role.Eli_User.Where(u => u.Status == (int)UserStatus.Active).Select(u => new Select2HierarchicalData.Option
                        {
                            id = u.Id.ToString(),
                            text = u.Name
                        }).ToArray()
                }).ToArray();
            return result;
        }
    }
}
