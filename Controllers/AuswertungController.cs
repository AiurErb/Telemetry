using Microsoft.AspNetCore.Mvc;
using JsonParser.Data;
using JsonParser.Models;
using System.Linq;
using JsonParser.Classes;
using System.Text.Json;
using System.IO;
using Microsoft.Extensions.Options;
using MySqlConnector;
using Dapper;
using System.Data;

namespace JsonParser.Controllers
{
    public class AuswertungController : Controller
    {
        private readonly TelemetrieParserConfig _config;
        public AuswertungController(IOptions<TelemetrieParserConfig> conf)
        {
            this._config = conf.Value;
        }

        public async Task<ActionResult> IndexAsync()
        {
            List<TableItem> tables = _config.Tables.Select(x => new TableItem()
            {
                Count = 0,
                Database = "telemetry_data",
                Table = x.TableName
            }).ToList();
            using IDbConnection con = _config.GetConnection();
            con.Open();
            foreach (TableItem table in tables)
            {
                var cnt = await con.QueryFirstOrDefaultAsync<int>($"SELECT Count(*) " +
                    $"FROM {table.Table};");

                table.Count = cnt;
            }

            //tables = tables.Union<TableItem>(TelemetryDb.DbInfo("telemetry_data")).ToList();

            return View(tables);
        }
        public async Task<ActionResult> CustomerAsync()
        {
            using IDbConnection con = _config.GetConnection();
            con.Open();
            List<Customer> kunden = (List<Customer>)await con.QueryAsync<Customer>(
                "SELECT KundenNr, Count(TelemetryId) AS TelemetryIdCount FROM telemetrydata GROUP BY KundenNr;");
            return View(kunden);
        }
        public ActionResult ParseRawData()
        {
            TelemetryParser telemetryParser = new TelemetryParser();
            string fileName = "config.json";
            string jsonString = System.IO.File.ReadAllText(fileName);
            TelemetrieParserConfig config =
                JsonSerializer.Deserialize<TelemetrieParserConfig>(jsonString);
            telemetryParser.Parse(config);
            return RedirectToAction("Index");
        }
        [HttpGet("Auswertung/CustomerDetails/{kundenNr}")]
        public async Task<ActionResult> CustomerDetails([FromRoute] string kundenNr)
        {
            CustomerDetails kunde = new();
            kunde.KundenNr = kundenNr;
            kunde.VorgaengeneProjecte = await TableWithCustomerFilterAsync<Vppj>(
                "VorgaengeProObjektProJahre", kundenNr);
            return View(kunde);
        }

        public async Task<List<T>> TableWithCustomerFilterAsync<T>(string table, string kundenNr)
        {
            IDbConnection con = _config.GetConnection();
            con.Open();
            string sql = $"SELECT target.* FROM {table} AS target " +
                "INNER JOIN telemetrydata ON target.TelemetryId = telemetrydata.TelemetryId " +
                $"WHERE telemetrydata.KundenNr = @kundenNr";
            List<T> filteredTable = con.Query<T>(sql, 
                new { kundenNr = kundenNr }).ToList();

            List<T> filteredTable2 = await con.QueryAsync<T>(sql, new { kundenNr = kundenNr })
                .ContinueWith(x => x.Result.ToList());

            // .ToList();
            return filteredTable2;
        }
    }
}
