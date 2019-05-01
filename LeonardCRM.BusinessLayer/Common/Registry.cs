using System;
using System.Globalization;

namespace LeonardCRM.BusinessLayer.Common
{
    public sealed class Registry
    {
        private RegistryHash _mReg;

        public string DefaultLanguageFileName { get; set; }

        public Registry()
        {
            _mReg = new RegistryHash();

            // get all the registry values
            var regs = RegistryBM.Instance.GetAll();

            // get all the registry settings into our hash table
            _mReg.Clear();
            foreach (var reg in regs )
            {
                if (!_mReg.ContainsKey(reg.Name.ToLower()) || _mReg[reg.Name.ToLower()] == null)
                    _mReg.Add(reg.Name.ToLower(), reg.Value);
            }

            DefaultLanguageFileName = "default.xml";
        }

        public string DEFAULT_LANGUAGE
        {
            get
            {
                return _mReg.GetValueString("DEFAULT_LANGUAGE", DefaultLanguageFileName);
            }
            set { _mReg.SetValueString("DEFAULT_LANGUAGE", value); }
        }

        public int ITEMS_PER_PAGE
        {
            get
            {
                 return _mReg.GetValueInt("ITEMS_PER_PAGE", 20);
            }
            set { _mReg.SetValueInt("ITEMS_PER_PAGE", value); }
        }

        public string DEFAULT_THEME
        {
            get
            {
                return _mReg.GetValueString("DEFAULT_THEME", "");
            }
            set { _mReg.SetValueString("DEFAULT_THEME", value); }
        }

        public string DEFAULT_TITLE
        {
            get
            {
                return _mReg.GetValueString("DEFAULT_TITLE", "");
            }
            set { _mReg.SetValueString("DEFAULT_TITLE", value); }
        }

        public string NOTIFICATION_FROM_EMAIL
        {
            get
            {
                return _mReg.GetValueString("NOTIFICATION_FROM_EMAIL", "");
            }
            set { _mReg.SetValueString("NOTIFICATION_FROM_EMAIL", value); }
        }

        public bool ENABLE_ERROR_LOG_EMAIL
        {
            get
            {
                return _mReg.GetValueBool("ENABLE_ERROR_LOG_EMAIL", false);
            }
            set { _mReg.SetValueBool("ENABLE_ERROR_LOG_EMAIL", value); }
        }

        public string SMTP_CREDENTIAL_EMAIL
        {
            get
            {
                return _mReg.GetValueString("SMTP_CREDENTIAL_EMAIL", "");
            }
            set { _mReg.SetValueString("SMTP_CREDENTIAL_EMAIL", value); }
        }

        public string SMTP_CREDENTIAL_PASSWORD
        {
            get
            {
                return _mReg.GetValueString("SMTP_CREDENTIAL_PASSWORD", "");
            }
            set { _mReg.SetValueString("SMTP_CREDENTIAL_PASSWORD", value); }
        }

        public string SMTP_SERVER
        {
            get
            {
                return _mReg.GetValueString("SMTP_SERVER", "");
            }
            set { _mReg.SetValueString("SMTP_SERVER", value); }
        }

        public int MAX_UPLOAD_FILESIZE
        {
            get
            {
                return _mReg.GetValueInt("MAX_UPLOAD_FILESIZE", 5);
            }
            set { _mReg.SetValueInt("MAX_UPLOAD_FILESIZE", value); }
        }

        public int MAX_PAGE_NUMBERS
        {
            get
            {
                return _mReg.GetValueInt("MAX_PAGE_NUMBERS", 5);
            }
            set { _mReg.SetValueInt("MAX_PAGE_NUMBERS", value); }
        }

        public string DATE_FORMAT
        {
            get
            {
                return _mReg.GetValueString("DATE_FORMAT", "");
            }
            set { _mReg.SetValueString("DATE_FORMAT", value); }
        }

        public string MONTH_FORMAT
        {
            get
            {
                return _mReg.GetValueString("MONTH_FORMAT", "");
            }
            set { _mReg.SetValueString("MONTH_FORMAT", value); }
        }

        public string TIME_FORMAT
        {
            get
            {
                return _mReg.GetValueString("TIME_FORMAT", "");
            }
            set { _mReg.SetValueString("TIME_FORMAT", value); }
        }

        public int SMTP_PORT
        {
            get
            {
                return _mReg.GetValueInt("SMTP_PORT", 25);
            }
            set { _mReg.SetValueInt("SMTP_PORT", value); }
        }

        public bool SMTP_SSL
        {
            get
            {
                return _mReg.GetValueBool("SMTP_SSL", false);
            }
            set { _mReg.SetValueBool("SMTP_SSL", value); }
        }

        public string CURRENCY
        {
            get
            {
                return _mReg.GetValueString("CURRENCY", "$");
            }
            set { _mReg.SetValueString("CURRENCY", value); }
        }

        public string DOMAIN_URL
        {
            get
            {
                return _mReg.GetValueString("DOMAIN_URL", "");
            }
            set { _mReg.SetValueString("DOMAIN_URL", value); }
        }      

        public int TOTAL_POINTS
        {
            get
            {
                return _mReg.GetValueInt("TOTAL_POINTS", 0);
            }
            set { _mReg.SetValueInt("TOTAL_POINTS", value); }
        }

        public decimal BLOCK_UNIT_PRICE
        {
            get
            {
                return _mReg.GetValueDecimal("BLOCK_UNIT_PRICE", 0);
            }
            set { _mReg.SetValueDecimal("BLOCK_UNIT_PRICE", value); }
        }

        public decimal MAN_HOUR_UNIT_PRICE
        {
            get
            {
                return _mReg.GetValueDecimal("MAN_HOUR_UNIT_PRICE", 0);
            }
            set { _mReg.SetValueDecimal("MAN_HOUR_UNIT_PRICE", value); }
        }

        public decimal LOADED_MILE_UNIT_PRICE
        {
            get
            {
                return _mReg.GetValueDecimal("LOADED_MILE_UNIT_PRICE", 0);
            }
            set { _mReg.SetValueDecimal("LOADED_MILE_UNIT_PRICE", value); }
        }

        public int FORMAT_DECIMAL_PLACES
        {
            get
            {
                return _mReg.GetValueInt("FORMAT_DECIMAL_PLACES", 0);
            }
            set { _mReg.SetValueInt("FORMAT_DECIMAL_PLACES", value); }
        }

        public string ACCOUNTING_USER_EMAIL
        {
            get
            {
                return _mReg.GetValueString("ACCOUNTING_USER_EMAIL", "");
            }
            set { _mReg.SetValueString("ACCOUNTING_USER_EMAIL", value); }
        }

        public string REPAIR_USER_EMAIL
        {
            get
            {
                return _mReg.GetValueString("REPAIR_USER_EMAIL", "");
            }
            set { _mReg.SetValueString("REPAIR_USER_EMAIL", value); }
        }

        public string NO_USER_EMAIL
        {
            get
            {
                return _mReg.GetValueString("NO_USER_EMAIL", "");
            }
            set { _mReg.SetValueString("NO_USER_EMAIL", value); }
        }

        /// <summary>
        /// Saves the whole setting registry to the database.
        /// </summary>
        public int SaveRegistry()
        {
            var regs = RegistryBM.Instance.GetAll();

            // loop through all values and commit them to the DB
            foreach (var reg in regs)
                reg.Value = _mReg[reg.Name.ToLower()].ToString();

            return RegistryBM.Instance.Update(regs);
        }
    }

    public class RegistryHash : System.Collections.Hashtable
    {
        // helper class functions
        public int GetValueInt(string name, int Default)
        {
            if (this[name.ToLower()] == null) return Default;
            return Convert.ToInt32(this[name.ToLower()]);
        }
        public void SetValueInt(string name, int value)
        {
            this[name.ToLower()] = Convert.ToString(value);
        }
        public bool GetValueBool(string name, bool Default)
        {
            if (this[name.ToLower()] == null) return Default;
            return Convert.ToBoolean(Convert.ToInt32(this[name.ToLower()]));
        }
        public void SetValueBool(string name, bool value)
        {
            this[name.ToLower()] = Convert.ToString(Convert.ToInt32(value));
        }
        public string GetValueString(string name, string Default)
        {
            if (this[name.ToLower()] == null) return Default;
            return Convert.ToString(this[name.ToLower()]);
        }
        public void SetValueString(string name, string value)
        {
            this[name.ToLower()] = value;
        }
        public decimal GetValueDecimal(string name, decimal Default)
        {
            if (this[name.ToLower()] == null) return Default;
            return Convert.ToDecimal(this[name.ToLower()]);
        }
        public void SetValueDecimal(string name, decimal value)
        {
            this[name.ToLower()] = Convert.ToString(value);
        }
    }
}
