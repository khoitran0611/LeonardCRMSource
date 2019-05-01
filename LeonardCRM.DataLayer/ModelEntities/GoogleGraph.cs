﻿using System.Collections.Generic;

namespace LeonardCRM.DataLayer.ModelEntities
{
    public class GoogleGraph
    {
        public ColInfo[] cols { get; set; }
        public DataPointSet[] rows { get; set; }
        public Dictionary<string, string> p { get; set; }
    }

    public class ColInfo
    {
        public string id { get; set; }
        public string label { get; set; }
        public string type { get; set; }
    }

    public class DataPointSet
    {
        public DataPoint[] c { get; set; }
    }

    public class DataPoint
    {
        public string v { get; set; } // value
        public string f { get; set; } // format
    }
}
