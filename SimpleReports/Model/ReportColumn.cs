namespace SimpleReports.Model
{
    public class ReportColumn : ReportObject
    {
        public ColumnTypes Type { get; set; } = ColumnTypes.text;
        public string? Format { get; set; }
    }
}