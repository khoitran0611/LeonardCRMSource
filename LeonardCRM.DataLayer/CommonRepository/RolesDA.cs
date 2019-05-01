using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.CommonRepository
{
    public sealed class RolesDA : EF5RepositoryBase<LeonardUSAEntities, Eli_Roles>
    {
        private static volatile RolesDA _instance;
        private static readonly object SyncRoot = new Object();
        public static RolesDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new RolesDA();
                    }
                }

                return _instance;
            }
        }
        private RolesDA() : base(Settings.ConnectionString) { }

        /// <summary>
        /// Get role hierachy by current role
        /// </summary>
        /// <param name="currentRoleId"></param>
        /// <returns></returns>
        public IList<Eli_Roles> GetAllRoles(int currentRoleId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var query = from r in context.sp_GetRoles(currentRoleId)
                            select new Eli_Roles
                                {
                                    Description = r.Description,
                                    Id = r.Id,
                                    IsHostAdmin = r.IsHostAdmin,
                                    Name = r.Name,
                                    Parent = r.Parent
                                };
                return query.ToList();
            }
        }

        public int SaveRole(Eli_Roles entity, IEnumerable<sp_GetRolesFieldsByRoleId_Result> rolesFields)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var rolePermissions = entity.Eli_RolesPermissions.ToList();
                var roleIds = rolePermissions.Select(r => r.Id).Distinct().ToArray();
                var deletedrole = context.Eli_RolesPermissions.Where(f => f.RoleId == entity.Id && !roleIds.Contains(f.Id)).ToList();
                var newrole = rolePermissions.Where(f => f.Id == 0).ToList();
                var updaterole = rolePermissions.Where(f => f.Id > 0 && f.RoleId > 0).ToList();
                entity.Eli_RolesPermissions.Clear();

                // Delete
                foreach (var appCountry in deletedrole)
                {
                    context.Eli_RolesPermissions.Remove(appCountry);
                }
                // Update
                foreach (var appCountry in updaterole)
                {
                    context.Eli_RolesPermissions.Attach(appCountry);
                    context.Entry(appCountry).State = System.Data.Entity.EntityState.Modified;
                }

                context.Eli_Roles.Attach(entity);
                foreach (var item in newrole)
                {
                    entity.Eli_RolesPermissions.Add(item);
                }

                if (entity.Id > 0)
                {
                    context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    context.Eli_Roles.Add(entity);
                }

                
                // insert/update rolefields
                List<sp_GetRolesFieldsByRoleId_Result> fields;
                if (entity.Id == 0)
                {
                    fields = context.sp_GetRolesFieldsByRoleId(null).ToList();
                    var fieldsIdRequest = rolesFields.Select(f => f.FId).ToList();
                    var fieldsInvisiable = fields.Where(f => !fieldsIdRequest.Contains(f.FId));
                    foreach (var rolesField in fieldsInvisiable)
                    {
                        var eliRolesFields = new Eli_RolesFields()
                        {
                            Id = rolesField.Id ?? 0,
                            CreatedBy = rolesField.CreatedBy,
                            CreatedDate = DateTime.Now,
                            FieldId = rolesField.FId,
                            RoleId = entity.Id,
                            ModifiedBy = rolesField.ModifiedBy,
                            ModifiedDate = DateTime.Now,
                            Locked = false,
                            Visible = rolesField.Display
                        };

                        context.Eli_RolesFields.Add(eliRolesFields);
                    }
                }
                else
                {
                    fields = context.sp_GetRolesFieldsByRoleId(entity.Id).ToList();
                }
                
                foreach (var rolesField in rolesFields)
                {
                    var fieldDb = fields.Single(f => f.FId == rolesField.FId);
                    var eliRolesFields = new Eli_RolesFields()
                    {
                        Id = rolesField.Id ?? 0,
                        CreatedBy = rolesField.CreatedBy,
                        CreatedDate = rolesField.CreatedDate,
                        FieldId = rolesField.FId,
                        RoleId = entity.Id,
                        ModifiedBy = rolesField.ModifiedBy,
                        ModifiedDate = rolesField.ModifiedDate,
                        Locked = rolesField.Locked ?? false,
                        Visible = rolesField.Visible ?? rolesField.Display
                    };

                    if (eliRolesFields.Id == 0)
                    {
                        if (fieldDb.Mandatory)
                        {
                            eliRolesFields.Locked = false;
                            eliRolesFields.Visible = fieldDb.Display;
                        }
                        context.Eli_RolesFields.Add(eliRolesFields);
                    }
                    else
                    {
                        if (fieldDb.Mandatory)
                        {
                            eliRolesFields.Locked = fieldDb.Locked ?? false;
                            eliRolesFields.Visible = fieldDb.Visible ?? fieldDb.Display;
                        }

                        context.Eli_RolesFields.Attach(eliRolesFields);
                        context.Entry(eliRolesFields).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                return context.SaveChanges();
            }
        }

        public int DeleteRoles(IList<Eli_Roles> entites)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                int[] roleIds = entites.Select(r => r.Id).ToArray();
                var records = context.Eli_User.Where(r => roleIds.Contains(r.RoleId));

                //Update User Role
                foreach (int roleId in roleIds)
                {
                    var role = entites.Single(r => r.Id == roleId);
                    foreach (var user in records.Where(r => r.RoleId == role.Id))
                    {
                        user.RoleId = role.ToRoleId;
                    }
                }

                //Delete Role
                foreach (var entity in entites)
                {
                    context.Eli_Roles.Attach(entity);
                    context.Eli_Roles.Remove(entity);
                }
                return context.SaveChanges();
            }
        }

        public Eli_Roles GetRolebyRoleId(int roleId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var entity = context.Eli_Roles.AsNoTracking()
                                              .SingleOrDefault(r => r.Id == roleId);
                if (entity != null)
                {
                    entity.Roles = context.Eli_Roles.AsNoTracking().Where(r => r.Id != roleId).ToList();
                    var fields = context.sp_GetRolesFieldsByRoleId(roleId).ToList();
                    entity.Eli_RolesPermissions = (from dataSource in context.sp_GetModulePermisstion(roleId)
                                                   select new Eli_RolesPermissions
                                                   {
                                                       Id = dataSource.Id,
                                                       RoleId = dataSource.RoleId,
                                                       ModuleId = dataSource.ModuleId,
                                                       AllowRead = dataSource.AllowRead,
                                                       AllowEdit = dataSource.AllowEdit,
                                                       AllowDelete = dataSource.AllowDelete,
                                                       AllowCreate = dataSource.AllowCreate,
                                                       AllowImport = dataSource.AllowImport,
                                                       AllowExport = dataSource.AllowExport,
                                                       AllowCreateView = dataSource.AllowCreateView,
                                                       Name = dataSource.Name,
                                                       ModuleParent = dataSource.ModuleParent,
                                                       ModuleAllowExport = dataSource.ModuleAllowExport,
                                                       ModuleAllowImport = dataSource.ModuleAllowImport,
                                                       ModuleCreateView = dataSource.ModuleCreateView,
                                                       EntityFields = fields.Where(f => f.ModuleId == dataSource.ModuleId).OrderBy(f=>f.SortOrder)
                                                   }).ToList();
                }
                else
                {
                    var fields = context.sp_GetRolesFieldsByRoleId(null).ToList();
                    entity = new Eli_Roles
                    {
                        Roles = context.Eli_Roles.AsNoTracking().ToList(),
                        Eli_RolesPermissions = (from dataSource in context.sp_GetModulePermisstion(roleId)
                                                select new Eli_RolesPermissions
                                                {
                                                    Id = dataSource.Id,
                                                    RoleId = dataSource.RoleId,
                                                    ModuleId = dataSource.ModuleId,
                                                    AllowRead = dataSource.Dashboard && dataSource.ModuleParent == 0,
                                                    AllowEdit = dataSource.Dashboard && dataSource.ModuleParent == 0,
                                                    AllowDelete = dataSource.Dashboard && dataSource.ModuleParent == 0,
                                                    Name = dataSource.Name,
                                                    ModuleParent = dataSource.ModuleParent,
                                                    EntityFields = fields.Where(f => f.ModuleId == dataSource.ModuleId).OrderBy(f => f.SortOrder)
                                                }).ToList()
                    };
                    var abc = entity.Eli_RolesPermissions.SelectMany(f => f.EntityFields);
                }
                return entity;
            }
        }

        public IEnumerable<Eli_Roles> GetRolesWithUserHierachy()
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var query = from r in context.Eli_Roles
                            select new
                                {
                                    r.Id,
                                    r.Name,
                                    r.Description,
                                    r.Eli_User
                                };
                return query.Where(r=>r.Eli_User.Count > 0).ToList().Select(x => new Eli_Roles
                {
                    Name = x.Name,
                    Description = x.Description,
                    Eli_User = x.Eli_User
                });
            }
        }

        public IEnumerable<Eli_Roles> GetRolesWithUserHierachy(int currentRole, int userId)
        {
            using (var context = new LeonardUSAEntities(Settings.ConnectionString))
            {
                var roles = context.fn_GetRolesHierachy(currentRole);
                var query = from r in context.Eli_Roles
                            where roles.Contains(r.Id)
                            select new
                            {
                                r.Id,
                                r.Name,
                                r.Description,
                                r.Eli_User
                            };
                if(roles.Count() > 1)
                    return query.Where(r => r.Eli_User.Count > 0).ToList().Select(x => new Eli_Roles
                    {
                        Name = x.Name,
                        Description = x.Description,
                        Eli_User = x.Eli_User
                    });

                return query.ToList().Select(x => new Eli_Roles
                {
                    Name = x.Name,
                    Description = x.Description,
                    Eli_User = x.Eli_User.Where(u=>u.Id == userId).ToList()
                });
            }
        }
    }
}
