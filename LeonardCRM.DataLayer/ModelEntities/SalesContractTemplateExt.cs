using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeonardCRM.DataLayer.ModelEntities
{
    public partial class SalesContractTemplate : CustomField
    {
        public int[] StateIds { get; set; }

        public int[] UsedStates { get; set; }
    }
}
