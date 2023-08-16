using MySqlConnector;
using Dapper;
using Dapper.Contrib.Extensions;
using JsonParser.Models;
using System.Text.Json;
using System.Runtime.CompilerServices;

namespace JsonParser.Data
{
    public class TelemetryDb
    {
        public static string Server { get; set; } = "localhost;";
        public static string UserID { get; set; } = "telemetry_parser;";
        public static string Password { get; set; } = "telemetry_parser;";
        public static string Database { get; set; } = "telemetry_parser";
        public static string RawDatabase { get; set; } = "telemetry_parser";
        public static string NewDatabase { get; set; } = "telemetry_data";
        public MySqlConnection TelemetryCon;

        /*
        public TelemetryDb()
        {
            TelemetryCon = GetCon();
        }*/

        public static MySqlConnection GetCon(string database)
        {
            string connectionString = $"Server ={Server}"
                + $"User ID={UserID}"
                + $"Password={Password}"
                + $"Database={database}";
            //Console.WriteLine(connectionString);
            return new MySqlConnection(connectionString) ;
        }

        public static List<TableItem> DbInfo (string database)
        {
            List<TableItem> tableItems = new List<TableItem>();
            using (MySqlConnection connection = GetCon(database))
            {
                connection.Open();
                MySqlCommand getTables = new MySqlCommand("SHOW TABLES;", connection);
                MySqlDataReader tables = getTables.ExecuteReader();
                
                while (tables.Read())
                {
                    tableItems.Add(new TableItem { 
                        Table = tables.GetString(0), 
                        Count = 0,
                        Database = database});
                }
                
            };
            using (MySqlConnection connection = GetCon(database))
            {
                connection.Open();
                foreach (TableItem tableItem in tableItems)
                {
                    MySqlCommand getAmount = new MySqlCommand(
                        $"SELECT Count(*) FROM {tableItem.Table};", connection);
                    
                    tableItem.Count = (int)(long)getAmount.ExecuteScalar();
                    
                }
                
            }
            
            return tableItems;
        }

        
        public async Task<List<TelemetryItem>> GetItems()
        {
            string sql = "SELECT * FROM rawtelemetrydata;";
            return await TelemetryCon.QueryAsync<TelemetryItem>(sql)
                .ContinueWith(x => x.Result.ToList());
        }

        public JsonElement GetContent(int id)
        {
            TelemetryItem item = TelemetryCon.Get<TelemetryItem>(id);
            JsonElement content = JsonDocument.Parse(item.Content).RootElement;
            return content;
        }
    }
}
