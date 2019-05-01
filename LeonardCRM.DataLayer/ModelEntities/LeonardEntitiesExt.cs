namespace LeonardCRM.DataLayer.ModelEntities
{
    public partial class LeonardUSAEntities
    {
        public LeonardUSAEntities(string connnectionStr)
            : base(connnectionStr)
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }
    }
}
