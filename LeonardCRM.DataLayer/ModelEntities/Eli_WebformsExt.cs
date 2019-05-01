namespace LeonardCRM.DataLayer.ModelEntities
{
    using System.Collections.Generic;
    public partial class Eli_Webforms
    {
        public IEnumerable<Eli_EntityFields> EntityFields { get; set; }
        public int CurrentModuleId { get; set; }
    }
}
