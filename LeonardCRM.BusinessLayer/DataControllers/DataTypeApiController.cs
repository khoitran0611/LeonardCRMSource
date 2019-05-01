using System;
using System.Collections.Generic;
using System.Web.Http;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;

namespace LeonardCRM.BusinessLayer.DataControllers
{
    public class DataTypeApiController : BaseApiController
    {
        [HttpGet]
        public IList<Eli_DataTypes> GetAllDataType()
        {
            try
            {
                return DataTypeBM.Instance.Find(r => r.IsPublic);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                return new List<Eli_DataTypes>();
            }
        }
    }
}
