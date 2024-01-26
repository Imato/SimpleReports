
namespace SimpleReports.Services
{
    public interface ISqlService
    {
        Task<IEnumerable<dynamic>> GetReportDataAsync(string connectionString, string sql, Dictionary<string, object> parameters);
    }
}