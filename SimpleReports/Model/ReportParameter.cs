namespace SimpleReports.Model
{
    public class ReportParameter : ReportObject
    {
        public ColumnTypes Type { get; set; } = ColumnTypes.text;
        public string? Format { get; set; }
        public object? DefaultValue { get; set; }
    }
}