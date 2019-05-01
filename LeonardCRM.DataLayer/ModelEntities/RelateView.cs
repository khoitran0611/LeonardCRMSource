namespace LeonardCRM.DataLayer.ModelEntities
{
    public class RelateView
    {
        public int ViewId { get; set; }
        public int ModuleId { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public bool Collapsed { get; set; }
    }
}
