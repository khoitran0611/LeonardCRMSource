namespace LeonardCRM.DataLayer.ModelEntities
{
    public class ColumnExport
    {
        public string ColumnName { get; set; }
        public string ModuleName { get; set; }
        public int ModuleId { get; set; }
        public int ViewId { get; set; }
        public string LabelDisplay { get; set; }
        public bool Display { get; set; }
        public bool IsInteger { get; set; }
        public bool IsDate { get; set; }
        public bool IsDecimal { get; set; }
        public bool IsText { get; set; }
        public bool IsList { get; set; }
        public bool IsTextArea { get; set; }
        public bool IsTime { get; set; }
        public bool IsEmail { get; set; }
        public bool IsUrl { get; set; }
        public bool IsMultiSelecttBox { get; set; }
        public bool IsCheckBox { get; set; }
        public bool IsImage { get; set; }
        public bool IsDateTime { get; set; }
        public bool IsCurrency { get; set; }
        public int Id { get; set; }
        public int SortOrder { get; set; }
        public string Width { get; set; }
        public int FieldId { get; set; }
        public bool Sortable { get; set; }
    }
}
