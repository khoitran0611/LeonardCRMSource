using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Security;

namespace LeonardCRM.BusinessLayer.Security
{
    public class RequireAuthorize : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            string username;
            string password;
            if (GetUserNameAndPassword(actionContext, out username, out password))
            {
                if (Membership.ValidateUser(username, password))
                {
                    if (!isUserAuthorized(username))
                        actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
                }
                else
                {
                    actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                }
            }
            else
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }

        private bool isUserAuthorized(string username)
        {
            throw new NotImplementedException();
        }

        private bool GetUserNameAndPassword(HttpActionContext actionContext, out string username, out string password)
        {
            throw new NotImplementedException();
        }
    }
}
