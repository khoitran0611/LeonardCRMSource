using System.Configuration;
using System.Collections.Specialized;

namespace Eli.Common
{
    public class Config
    {
        public Config() { }

        public static NameValueCollection GetConfiguration()
        {
            return ConfigurationSettings.GetConfig("ElinextSettings") as NameValueCollection;
        }
    }

    public class ElinextSectionHandler : NameValueSectionHandler
    {
        protected override string KeyAttributeName
        {
            get
            {
                return base.KeyAttributeName;
            }
        }

        protected override string ValueAttributeName
        {
            get
            {
                return base.ValueAttributeName;
            }
        }
    }
}
