using MySqlConnector;
using Newtonsoft.Json;
using System.Data;

namespace JsonParser.Classes
{
    public class TelemetrieParserConfig
    {
        public string? DatebaseNewConnectionString { get; set; } =
            "Server = localhost;User ID= telemetry_parser;Password= telemetry_parser;Database=telemetry_data;Allow User Variables=true";
        public string? DatebaseRawConnectionString { get; set; } =
            "Server = localhost;User ID= telemetry_parser;Password= telemetry_parser;Database=telemetry_parser;Allow User Variables=true";

        public IDbConnection GetConnection() =>
            new MySqlConnection(DatebaseNewConnectionString);
        

        public IEnumerable<Table>? Tables { get; set; }
    }
    public class Table
    {
        public string? Path { get; set; }
        public string? TableName { get; set; }
        public IEnumerable<Mapping>? Mappings { get; set; }
    }
    public class Mapping
    {
        public string? JsonName { get; set; }
        public string? DbName { get; set; }
        public string? Field { get; set; }
    }
}