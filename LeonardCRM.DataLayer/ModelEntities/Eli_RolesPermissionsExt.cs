using System.Collections.Generic;
using System.Linq;

namespace LeonardCRM.DataLayer.ModelEntities
{
    public partial class Eli_RolesPermissions
    {
        public string Name { get; set; }

        public int ModuleParent { get; set; }

        public bool FullControl {
            get
            {
                return AllowRead && AllowEdit && AllowDelete;
            }
        }

        public bool ModuleCreateView { get; set; }
        public bool ModuleAllowImport { get; set; }
        public bool ModuleAllowExport { get; set; }
        public IEnumerable<sp_GetRolesFieldsByRoleId_Result> EntityFields { get; set; }
        //public bool ModuleAllowCreate { get; set; }
        //public bool ModuleAllowEdit { get; set; }
        //public bool ModuleAllowDelete { get; set; }
    }
}
