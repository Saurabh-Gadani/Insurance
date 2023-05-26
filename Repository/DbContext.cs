using Microsoft.Extensions.Options;
using System.Data;
using Microsoft.Data.SqlClient;
using MySqlConnector;

namespace MVCWebApplication.Repository
{
    public class ConnectionStringOptions
    {
        public const string Position = "ConnectionStrings";
        public string SqlConnection { get; set; }
    }
    public class DbContext
    {
        private ConnectionStringOptions connectionStringOptions;
        public DbContext(IOptionsMonitor<ConnectionStringOptions> optionsMonitor)
        {
            connectionStringOptions = optionsMonitor.CurrentValue;
        }
        public IDbConnection CreateConnection() => new MySqlConnection(connectionStringOptions.SqlConnection);
    }
}
