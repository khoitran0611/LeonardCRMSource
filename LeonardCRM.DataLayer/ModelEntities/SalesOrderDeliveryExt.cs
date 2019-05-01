using System;
using System.Collections;
using System.Collections.Generic;

namespace LeonardCRM.DataLayer.ModelEntities
{
    public partial class SalesOrderDelivery : CustomField
    {
        public List<string> Filenames { get; set; }
        public string UploadDirectory { get; set; }
        public int[] UserIds { get; set; }
        public IList<Eli_FieldData> FieldData { get; set; }

        /// <summary>
        /// Use for Public Api
        /// </summary>
        public int[] CustomerIds { get; set; }
        /// <summary>
        /// Use for Public Api
        /// </summary>
        public int ModuleId { get; set; }
        public IList<Eli_Notes> Notes { get; set; }
        public string StatusName { get;set; }
        public string StatusDescription { get; set; }

        public String CustomerSignImage { get; set; }
        public String DriverSignImage { get; set; }

        public String CustomerSignImageUrl { get; set; }
        public String DriverSignImageUrl { get; set; }
    }
}
