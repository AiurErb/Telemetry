using Dapper.Contrib.Extensions;

namespace JsonParser.Classes
{
    [Table("telemetrydata")]
    public class TelemetryItem
    {
        [ExplicitKey]
        public int TelemetryId { get; set; }
        public DateTime CreateDate { get; set; }
        public String? KundenNr { get; set; }
        public String? IP { get; set; }
        public String? ClientVersion { get; set; }
        public String? ServerVersion { get; set; }
    }
}
