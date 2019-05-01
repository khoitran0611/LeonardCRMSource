using System.Collections.Generic;

namespace LeonardCRM.DataLayer.ModelEntities
{
    public partial class Eli_Roles
    {
        public int ToRoleId { get; set; }
        public int ModuleId { get; set; }
        public IList<Eli_Roles> Roles { get; set; }
        public int[] ParentArray { get; set; }
        //public IList<vwModulePermission> ModulePermissions { get; set; }
    }
}
