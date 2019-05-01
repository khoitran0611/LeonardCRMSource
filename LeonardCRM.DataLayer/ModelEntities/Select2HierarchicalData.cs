namespace LeonardCRM.DataLayer.ModelEntities
{
    public class Select2HierarchicalData
    {
        public bool more { get; set; }
        public OptionGroup[] results { get; set; }

        public class OptionGroup
        {
            public string id { get; set; }
            public string text { get; set; }
            public Option[] children { get; set; }
        }

        public class Option
        {
            public string id { get; set; }
            public string text { get; set; }
        }
    }
}
