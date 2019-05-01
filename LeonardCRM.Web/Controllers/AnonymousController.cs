using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Eli.Common;
using LeonardCRM.BusinessLayer;
using LeonardCRM.BusinessLayer.Common;
using LeonardCRM.DataLayer.ModelEntities;

namespace LeonardCRM.Web.Controllers
{
    public class AnonymousController : Controller
    {
        //CultureInfo currentCulture;
        private readonly Registry _registry;
        public AnonymousController()
        {
            _registry = LoadRegistry();
            ViewBag.Title = _registry.DEFAULT_TITLE;
        }      

        #region Properties
        /// <summary>
        /// Getting the server URL, eg. http://abc.com or http://localhost:8080
        /// </summary>
        protected string ServerURL
        {
            get
            {
                var serverPort = long.Parse(System.Web.HttpContext.Current.Request.ServerVariables["SERVER_PORT"]);
                var isSecure = (System.Web.HttpContext.Current.Request.ServerVariables["HTTPS"].ToLower() == "on");

                var url = new StringBuilder("http");

                if (isSecure)
                {
                    url.Append("s");
                }

                url.AppendFormat("://{0}", System.Web.HttpContext.Current.Request.ServerVariables["SERVER_NAME"]);

                if ((!isSecure && serverPort != 80) || (isSecure && serverPort != 443))
                {
                    url.AppendFormat(":{0}", serverPort.ToString());
                }

                return url.ToString();
            }
        }      

        public string CurrentCulture
        {
            get
            {
                var cache = new CacheManager();
                if (!cache.Exist(Constant.CurrentLanguage))
                {
                    var languages = ControlHelper.Languages();
                    var current = languages.SingleOrDefault(l => l.FileName == SiteSettings.DEFAULT_LANGUAGE);
                    if (current != null)
                    {
                        cache.Add(current.Code, Constant.CurrentLanguage);
                        return current.Code;
                    }
                }
                return cache.Get<string>(Constant.CurrentLanguage);

            }
            set { new CacheManager().Remove(Constant.CurrentLanguage); }
        }       

        public Registry SiteSettings
        {
            get {
                return _registry;
            }
        }

        private Registry LoadRegistry()
        {
            if (!ConfigValues.ENABLE_CACHE)
            {
                return new Registry();
            }
            var cache = new CacheManager();
            if (!cache.Exist(Constant.SiteSettings))
            {
                cache.Remove(Constant.SiteSettings);
                cache.Add(new Registry(), Constant.SiteSettings);
            }
            return cache.Get<Registry>(Constant.SiteSettings);
        }

        #endregion      

        #region Actions

        public ActionResult DeliveryRequestView(string id)
        {
            try
            {
                var model = SalesCustomerBM.Instance.GetApplicantById(int.Parse(SecurityHelper.Decrypt(id)));
                if (model != null)
                {
                    HashDeliverySignature(model);
                    LoadDeliveryPickList();
                    ViewBag.Title = LocalizeHelper.Instance.GetText("DELIVERY_REQUEST_FORM", "PUBLIC_TITLE");
                    ViewBag.Registry = this.SiteSettings;
                    return View(model);
                }
                return Redirect("/notfound");
            }
            catch(Exception ex)                
            {
                LogHelper.Log(ex.Message, ex);
                throw ex;
            }
        }

        public ActionResult CustomerAcceptanceView(string id)
        {
            try
            {
                var model = SalesCustomerBM.Instance.GetApplicantById(int.Parse(SecurityHelper.Decrypt(id)));
                if (model != null)
                {
                    HashCompleteSignature(model);
                    LoadAcceptancePickList();
                    ViewBag.Title = LocalizeHelper.Instance.GetText("CUSTOMER_ACCEPTANCE_FORM", "PUBLIC_TITLE");
                    ViewBag.Registry = this.SiteSettings;
                    return View(model);
                }
                return Redirect("/notfound");
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, ex);
                throw ex;
            }
        }

        #endregion

        #region Internal private

        private void LoadDeliveryPickList()
        {
            var values = ListNameBM.Instance.GetListNameValuesByModules(new int[] { Constant.ModuleOrderDelivery, Constant.ModuleCustomer});
            ViewBag.DeliveryTimes = values.Where(x => x.FieldName == "DeliveryTime").ToList();
            var deliveryTypes = values.Where(x => x.FieldName == "DeliveryType").ToList();
            ViewBag.DeliveryTypes = deliveryTypes.Take(deliveryTypes.Count() - 1).ToList();
            ViewBag.States = values.Where(x => x.FieldName == "PhysicalState").ToList();
            ViewBag.LoadDoorFacings = values.Where(x => x.FieldName == "LoadDoorFacing").ToList();
        }

        private void LoadAcceptancePickList()
        {
            var values = ListNameBM.Instance.GetListNameValuesByModules(new int[] { Constant.ModuleOrderDelivery, Constant.ModuleCustomer, Constant.ModuleSaleComplete });            
            ViewBag.DeliveryTypes = values.Where(x => x.FieldName == "DeliveryType" && x.ModuleId == Constant.ModuleSaleComplete.GetHashCode()).ToList();
            ViewBag.States = values.Where(x => x.FieldName == "PhysicalState").ToList();
            ViewBag.PayementTypes = values.Where(x => x.FieldName == "PaymentType").ToList();
            ViewBag.Ratings = values.Where(x => x.FieldName == "Rating").ToList();
        }

        private void HashDeliverySignature(SalesCustomer model)
        {
            var saleOrder = model.SalesOrders.FirstOrDefault();
            if (saleOrder != null &&
                saleOrder.SalesOrderDeliveries != null &&
                saleOrder.SalesOrderDeliveries.Any())
            {
                var delivery = saleOrder.SalesOrderDeliveries.First();
                if (!string.IsNullOrEmpty(delivery.CustomerSignImageUrl))
                {
                    var physicalPath = Server.MapPath("~" + delivery.CustomerSignImageUrl.Split(new string[] { "?" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    if (System.IO.File.Exists(physicalPath))
                    {
                        delivery.CustomerSignImageUrl = ImageHelper.GetImageBase64(physicalPath, "data:image/png;base64,");
                    }
                }
            }
        }
        
        private void HashCompleteSignature(SalesCustomer model)
        {
            var saleOrder = model.SalesOrders.FirstOrDefault();
            if (saleOrder != null &&
                saleOrder.SalesOrderCompletes != null &&
                saleOrder.SalesOrderCompletes.Any())
            {
                var complete = saleOrder.SalesOrderCompletes.First();
                var physicalPath = "";
                if (!string.IsNullOrEmpty(complete.CustomerSignatureUrl))
                {
                    physicalPath = Server.MapPath("~" + complete.CustomerSignatureUrl.Split(new string[] { "?" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    if (System.IO.File.Exists(physicalPath))
                    {
                        complete.CustomerSignatureUrl = ImageHelper.GetImageBase64(Server.MapPath("~" + complete.CustomerSignatureUrl.Split(new string[] { "?" }, StringSplitOptions.RemoveEmptyEntries)[0]), "data:image/png;base64,");
                    }
                }
                if (!string.IsNullOrEmpty(complete.DeliverySignatureUrl))
                {
                    physicalPath = Server.MapPath("~" + complete.DeliverySignatureUrl.Split(new string[] { "?" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    if (System.IO.File.Exists(physicalPath))
                    {
                        complete.DeliverySignatureUrl = ImageHelper.GetImageBase64(Server.MapPath("~" + complete.DeliverySignatureUrl.Split(new string[] { "?" }, StringSplitOptions.RemoveEmptyEntries)[0]), "data:image/png;base64,");
                    }
                }
                if (!string.IsNullOrEmpty(complete.ManagerSignatureUrl))
                {
                    physicalPath = Server.MapPath("~" + complete.ManagerSignatureUrl.Split(new string[] { "?" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    if (System.IO.File.Exists(physicalPath))
                    {
                        complete.ManagerSignatureUrl = ImageHelper.GetImageBase64(Server.MapPath("~" + complete.ManagerSignatureUrl.Split(new string[] { "?" }, StringSplitOptions.RemoveEmptyEntries)[0]), "data:image/png;base64,");
                    }
                }
            }
        }
        #endregion
    }
}
