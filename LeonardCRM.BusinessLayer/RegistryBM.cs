using System;
using System.Collections.Generic;
using System.Linq;
using Eli.Common;
using LeonardCRM.DataLayer.CommonRepository;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.BusinessLib;
using Elinext.DataLib;

namespace LeonardCRM.BusinessLayer
{
    public sealed class RegistryBM : BusinessBase<IRepository<Eli_Registry>, Eli_Registry>
    {
        private static volatile RegistryBM _instance;
        private static readonly object SyncRoot = new Object();

        public static RegistryBM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new RegistryBM();
                    }
                }

                return _instance;
            }
        }

        private RegistryBM()
            : base(RegistryDA.Instance)
        {}

        public IList<vwRegistry> GetRegistryItems()
        {
            return RegistryDA.Instance.GetRegistryItems();
        }

        public MailServerInfo GetMailServerInfo()
        {
            var names = new[] { "SMTP_SERVER", "SMTP_CREDENTIAL_EMAIL", "SMTP_CREDENTIAL_PASSWORD", "SMTP_PORT", "SMTP_SSL" };
            var registry = Find(r=> names.Contains(r.Name));
            return new MailServerInfo
            {
                SmtpServer = registry.Single(r => r.Name == "SMTP_SERVER").Value,
                Username = registry.Single(r => r.Name == "SMTP_CREDENTIAL_EMAIL").Value,
                Password = registry.Single(r => r.Name == "SMTP_CREDENTIAL_PASSWORD").Value,
                SmtpPort = int.Parse(registry.Single(r => r.Name == "SMTP_PORT").Value),
                EnableSSL = registry.Single(r => r.Name == "SMTP_SSL").Value == "1"
            };
        }
    }
}
