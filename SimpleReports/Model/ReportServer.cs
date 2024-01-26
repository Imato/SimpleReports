namespace SimpleReports.Model
{
    public class ReportServer
    {
        public string Name { get; set; } = null!;
        public IEnumerable<Source> Sources { get; set; } = null!;
        public IEnumerable<Report> Reports { get; set; } = null!;
    }
}