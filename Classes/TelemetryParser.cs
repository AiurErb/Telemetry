using Dapper;
using Dapper.Contrib.Extensions;
using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Globalization;
using telemetry_parser;

namespace JsonParser.Classes
{
    public class TelemetryParser
    {
        const string OverviewTableCreateCommand =
          "CREATE TABLE IF NOT EXISTS `telemetrydata` (" +
          "`TelemetryId` int(11) NOT NULL UNIQUE," +
          "`CreateDate` datetime NOT NULL," +
          "`KundenNr` varchar(50) DEFAULT NULL," +
          "`IP` varchar(50) DEFAULT NULL," +
          "`ClientVersion` varchar(50) DEFAULT NULL," +
          "`ServerVersion` varchar(50) DEFAULT NULL," +
          "PRIMARY KEY(`TelemetryId`)" +
      ");";

        const string RawTelemetryDataCommand =
        "SELECT rawtelemetrydata.* " +
        "FROM telemetry_data.rawtelemetrydata " +
        "LEFT JOIN telemetry_parser.telemetrydata " +
        "ON rawtelemetrydata.Id = telemetrydata.TelemetryId " +
        "WHERE telemetrydata.TelemetryId IS NULL";


        public async Task Parse(TelemetrieParserConfig config)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            if (config.Tables is null)
            {
                throw new ArgumentNullException("config.Tables");
            }

            using var databaseNewConnection = new MySqlConnection(config.DatebaseNewConnectionString);
            //Log.Information("Opening MySQL Connection to new database");
            await databaseNewConnection.OpenAsync();

            using var connectionraw = new MySqlConnection(config.DatebaseRawConnectionString);
            //Log.Information("Opening MySQL Connection to raw database");
            await connectionraw.OpenAsync();
            //Log.Debug("Creating (if not exists) overview table on new database");
            using (var createOverviewCommand = new MySqlCommand(OverviewTableCreateCommand, databaseNewConnection))
            using (createOverviewCommand.ExecuteReader()) ;
            //Log.Information("Parsing new TelemetryItems");
            foreach (RawTelemetryItem CurrentTelemetryitem in databaseNewConnection.Query<RawTelemetryItem>(RawTelemetryDataCommand).Where(x => !string.IsNullOrWhiteSpace(x.Content)))
            {

                if (CurrentTelemetryitem.Content is null)
                    continue;

                //Log.Debug("Insert TelemetryItem to overview table");
                JObject o = null;
                try
                {
                    o = JObject.Parse(CurrentTelemetryitem.Content);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                //Log.Debug("Parse JSON content from TelemetryItem", CurrentTelemetryitem.Id);
                if (o is null)
                    continue;

                foreach (Table table in config.Tables.Where(t=>!string.IsNullOrWhiteSpace(t.TableName) && !string.IsNullOrWhiteSpace(t.Path)))
                {
                    if (table is null || table.Path is null || table.Mappings is null)
                        continue;   


                    //Log.Information("Parsing Table {TableName}", table.TableName);
                    IEnumerable<JToken> items = o.SelectTokens(table.Path);

                    //Log.Debug("Generating SQL commands");
                    String createCommandString = String.Format("CREATE TABLE IF NOT EXISTS {0} (TelemetryId int(10) NOT NULL", table.TableName);
                    String insertCommandString = String.Format("INSERT INTO {0} (TelemetryId", table.TableName);
                    foreach (Mapping mapping in table.Mappings)
                    {
                        string field = mapping.Field?.ToLower() switch
                        {
                            "string" => "varchar(36)",
                            "int" => "int(10)",
                            "decimal" => "decimal(10,5)",
                            "datetime" => "datetime",
                            _ => throw new ArgumentException("Wrong field name is given"),
                        };
                        createCommandString = String.Format("{0}, {1} {2} NOT NULL", createCommandString, mapping.DbName, field);
                        insertCommandString = String.Format("{0}, {1}", insertCommandString, mapping.DbName);
                    }

                    createCommandString += ");";
                    insertCommandString += ") VALUES(@TelemetryId";

                    foreach (Mapping mapping in table.Mappings)
                    {
                        insertCommandString += ", @" + mapping.DbName;
                    }

                    insertCommandString += ");";

                    //Log.Debug("Executing create SQL command");
                    using (var createCommand = new MySqlCommand(createCommandString, databaseNewConnection))
                    using (createCommand.ExecuteReader()) ;

                    foreach (JToken item in items)
                    {
                        using var insertCommand = new MySqlCommand(insertCommandString, databaseNewConnection);
                        //Log.Debug("Adding parameters");
                        insertCommand.Parameters.AddWithValue("@TelemetryId", CurrentTelemetryitem.Id);

                        foreach (Mapping mapping in table.Mappings)
                        {
                            String insert = item[mapping.DbName].ToString();
                            //Log.Debug("Adding {insert} as parameter", insert);
                            switch (mapping.Field.ToLower())
                            {
                                case "string":
                                    insertCommand.Parameters.AddWithValue("@" + mapping.DbName, insert);
                                    break;
                                case "int":
                                    insertCommand.Parameters.AddWithValue("@" + mapping.DbName, int.Parse(insert));
                                    break;
                                case "decimal":
                                    insertCommand.Parameters.AddWithValue("@" + mapping.DbName, decimal.Parse(insert));
                                    break;
                                case "datetime":
                                    insertCommand.Parameters.AddWithValue("@" + mapping.DbName, DateTime.ParseExact(insert, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None));
                                    break;

                            }
                        }

                        //Log.Debug("Executing insert SQL command");
                        await insertCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            //Log.Information("Closing MySQL connection to raw database");
            //Log.Information("Closing MySQL connection to new database");
            //Log.Information("Done.");
        }
    }
}
