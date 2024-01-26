using SimpleReports.Model;

namespace SimpleReports.Services
{
    public interface IReportService
    {
        Report GetReport(string name);

        IAsyncEnumerable<object?[]> GetReportDataAsync(
           string connectionString,
           string sql,
           ReportColumn[] columns,
           Dictionary<string, object> parameters);

        IEnumerable<Report> GetReports();

        Source GetSource(string name);

        IEnumerable<Source> GetSources();

        string ReportServerName();

        ReportColumn[] GetColumns(object obj);
    }
}