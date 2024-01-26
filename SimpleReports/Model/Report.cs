namespace SimpleReports.Model
{
    public class Report : ReportObject
    {
        public string Path { get; set; } = null!;
        public ReportColumn[] Columns { get; set; } = null!;
        public ReportParameter[] Parameters { get; set; } = null!;
        public string SourceName { get; set; } = null!;
        public Source Source { get; set; } = null!;
        public string Sql { get; set; } = null!;
    }
}