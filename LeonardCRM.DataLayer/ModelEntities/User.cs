using System.Collections.Generic;
using System.Security.Principal;

namespace LeonardCRM.DataLayer.ModelEntities
{
    public partial class Eli_User : IIdentity
    {
        #region IIdentity Members
        public string AuthenticationType
        {
            get
            {
                return "Elinext_Authen";
            }
        }

        private bool _isAuthenticated;
        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
            set { _isAuthenticated = value; }
        }

        #endregion

        private string _passwordConfirm;
        public string PasswordConfirm {
            get { return _passwordConfirm; }
            set { _passwordConfirm = value; }
        }

        public string Captcha { get; set; }
    }
}