using System;
using Eli.Common;
using LeonardCRM.DataLayer.ModelEntities;
using Elinext.DataLib;

namespace LeonardCRM.DataLayer.CommonRepository
{
    public sealed class MailTemplateDA : EF5RepositoryBase<LeonardUSAEntities, Eli_MailTemplates>
    {
        private static volatile MailTemplateDA _instance;
        private static readonly object SyncRoot = new Object();
        public static MailTemplateDA Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new MailTemplateDA();
                    }
                }

                return _instance;
            }
        }

        private MailTemplateDA() : base(Settings.ConnectionString) { }
    }
}
