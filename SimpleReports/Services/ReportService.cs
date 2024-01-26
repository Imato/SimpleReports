using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimpleReports.Model;
using System.Reflection;

namespace SimpleReports.Services
{
    public class ReportService : IReportService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<ReportService> logger;
        private readonly ISqlService sqlService;
        private Dictionary<string, Source> sources = new Dictionary<string, Source>();
        private Dictionary<string, Report> reports = new Dictionary<string, Report>();

        public ReportService(IConfiguration configuration,
            ILogger<ReportService> logger,
            ISqlService sqlService)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.sqlService = sqlService;
        }

        public ReportServer GetReportServer()
        {
            var name = nameof(ReportServer);
            return configuration
                .GetSection(name)
                .Get<ReportServer>()
                ?? throw new ArgumentException($"Not exists section {name} in configuration");
        }

        public string ReportServerName()
        {
            return configuration.GetSection("ReportServer:Name").Value ?? "";
        }

        public IEnumerable<Report> GetReports()
        {
            if (reports.Count == 0)
            {
                GetSources();
                reports = GetReportServer().Reports
                    .Select(x =>
                    {
                        x.Source = sources[x.SourceName];
                        return x;
                    })
                    .ToDictionary(x => x.Name, x => x);
            }
            return reports.Values;
        }

        public IEnumerable<Source> GetSources()
        {
            if (sources.Count == 0)
            {
                sources = GetReportServer().Sources
                    .ToDictionary(x => x.Name, x => x);
            }
            return sources.Values;
        }

        public Source GetSource(string name)
        {
            GetSources();
            return sources.ContainsKey(name)
                ? sources[name]
                : throw new ArgumentException($"Unknown source {name}");
        }

        public Report GetReport(string name)
        {
            GetReports();
            return reports.ContainsKey(name)
                ? reports[name]
                : throw new ArgumentException($"Unknown report {name}");
        }

        public ReportColumn[] GetColumns(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return obj.GetType()
                .GetProperties(BindingFlags.Public)
                .Select(x => new ReportColumn
                {
                    Name = x.Name,
                    ViewName = x.Name
                })
                .ToArray();
        }

        public async IAsyncEnumerable<object?[]> GetReportDataAsync(
            string connectionString,
            string sql,
            ReportColumn[] columns,
            Dictionary<string, object> parameters)
        {
            foreach (var d in await sqlService.GetReportDataAsync(
                connectionString,
                sql,
                parameters))
            {
                if (columns.Length == 0)
                {
                    columns = GetColumns(d);
                }

                var row = new object?[columns.Count()];
                for (int i = 0; i < columns.Count(); i++)
                {
                    var column = columns[i];
                    var value = d[column.Name];
                    value = column.Format != null && value != null
                        ? string.Format(value, column.Format)
                        : value;
                    row[i] = value;
                }
                yield return row;
            }
        }
    }
}