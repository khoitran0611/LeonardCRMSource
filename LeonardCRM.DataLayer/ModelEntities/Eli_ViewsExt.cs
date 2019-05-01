using System.Collections;

namespace LeonardCRM.DataLayer.ModelEntities
{
    public partial class Eli_Views
    {
        public Hashtable NameValues { get; set; }
        public int[] Parent { get; set; }
        public int CurrentModule { get; set; }
        public int[] UserRoleArray { get; set; }
        //public IList<Eli_Views> ViewList { get; set; }
    }
}
