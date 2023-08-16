using System.Collections.Generic;
using System.Text.Json;

namespace JsonParser.Models
{
    public class TelemetryTable
    {
        public string Name { get; set; }
        public List<JsonElement> Rows { get; set; }
    }
}
