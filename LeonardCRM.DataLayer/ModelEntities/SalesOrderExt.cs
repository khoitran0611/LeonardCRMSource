using System;
using System.Collections;
using System.Collections.Generic;

namespace LeonardCRM.DataLayer.ModelEntities
{
    public partial class SalesOrder : CustomField
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
        public string StatusName { get; set; }
        public string StatusDescription { get; set; }

        public String CustomerSignImage { get; set; }
        public String CoCustomerSignImage { get; set; }
        public int OrginalStatus { get; set; }

        public IList<ResponsibleUserModel> UsersPicklist { get; set; }
        public int[] DeliveryUserIds { get; set; }

        public bool IsAdmin { get; set; }

        //only use the admin & client admin user
        public bool IsAdminRoleUsers { get; set; }
    }
}
