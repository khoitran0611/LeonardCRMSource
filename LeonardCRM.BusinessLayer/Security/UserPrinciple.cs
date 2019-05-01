using System.Linq;
using System.Security.Principal;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;

namespace LeonardCRM.BusinessLayer.Security
{
    public class UserPrinciple : IPrincipal 
    {
        private Eli_User _user { get; set; }

        public UserPrinciple(Eli_User user)
        {
            _user = user;
        }

        #region IPrincipal Members

        public IIdentity Identity
        {
            get { return _user; }
        }

        public bool IsInRole(string role)
        {
            throw new System.NotImplementedException();
        }

        //public bool IsInRole(params Role[] roles)
        //{
        //    if (_user.Eli_Roles != null)
        //    {
        //        var role = (Role)System.Enum.Parse(typeof(Role), _user.Eli_Roles.Id.ToString());
        //        return roles.Contains(role);
        //    }
        //    return false;
        //}

        public bool HasPermission(Permission permission)
        {
            //return BaseUserBM.IsUserInPermission(permission);
            return true;
        }

        //public bool HasRole(Role role)
        //{
        //    return _user.Eli_Roles.Id == (int)role;
        //}

        #endregion
    }

}
