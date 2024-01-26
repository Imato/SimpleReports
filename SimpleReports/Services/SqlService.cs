using System.Data;
using System.Text.Json;
using System.Text.RegularExpressions;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Npgsql;
using SimpleReports.Model;

namespace SimpleReports.Services
{
    public class SqlService : ISqlService
    {
        private Regex reEnv = new Regex("\\{.*\\}",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly ILogger logger;

        public SqlService(ILogger<SqlService> logger)
        {
            this.logger = logger;
        }

        private string GetConnectionString(string connectionString)
        {
            var cs = connectionString;
            if (cs.Contains("{") && cs.Contains("}"))
            {
                foreach (var m in reEnv.Matches(cs).AsEnumerable())
                {
                    var name = m.Groups[1].Value;
                    var value = Environment.GetEnvironmentVariable(name);
                    if (value == null)
                    {
                        throw new ApplicationException($"Unknown Environment Variable {name}");
                    }
                    cs = cs.Replace(m.Groups[0].Value, value);
                }
            }

            logger?.LogDebug($"Using connection string {cs}");
            return cs;
        }

        private IDbConnection GetConnection(string connectionString)
        {
            var cs = GetConnectionString(connectionString);

            switch (Vendor(cs))
            {
                case ContextVendors.mssql:
                    return new SqlConnection(cs);

                case ContextVendors.postgres:
                    return new NpgsqlConnection(cs);

                case ContextVendors.mysql:
                    return new MySqlConnection(cs);
            }

            throw new ArgumentOutOfRangeException($"Unknown vendor in connection string {cs}");
        }

        private static ContextVendors Vendor(string connectionString)
        {
            var cs = connectionString;

            if (cs.Contains("Initial Catalog"))
            {
                return ContextVendors.mssql;
            }

            if (cs.Contains("Host")
                && cs.Contains("Database"))
            {
                return ContextVendors.postgres;
            }

            if (cs.Contains("Server")
                && cs.Contains("Database"))
            {
                return ContextVendors.mysql;
            }

            throw new InvalidOperationException($"Unknown context vendor for string: {connectionString}");
        }

        public async Task<IEnumerable<dynamic>> GetReportDataAsync(
            string connectionString,
            string sql,
            Dictionary<string, object> parameters)
        {
            using (var c = GetConnection(connectionString))
            {
                logger?.LogDebug($"Execute SQL {sql} with parameters: {JsonSerializer.Serialize(parameters)}");
                return await c.QueryAsync<dynamic>(sql, parameters);
            }
        }
    }
}