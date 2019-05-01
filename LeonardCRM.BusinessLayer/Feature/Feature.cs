using System.Collections.Specialized;

namespace LeonardCRM.BusinessLayer.Feature
{
    public abstract class Feature
    {
        private string _displayTitle;
        private readonly StringCollection _parameters = null;

        protected Feature() { }


        public string DisplayTitle
        {
            get
            {
                if (_displayTitle != null)
                    return _displayTitle;
                return string.Empty;
            }

            set
            {
                _displayTitle = value;
            }
        }


        public StringCollection Parameters
        {
            get
            {
                return _parameters;
            }
        }




    }
}
