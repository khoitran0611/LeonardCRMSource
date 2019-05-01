namespace LeonardCRM.DataLayer.ModelEntities
{
    public class BriefUser
    {
        public int Id { get; set; }
        public string LoginName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public bool Status { get; set; }
        public int RoleId { get; set; }
        public Eli_Roles Eli_Roles { get; set; }
    }
}
