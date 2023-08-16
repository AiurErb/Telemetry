using Dapper.Contrib.Extensions;

namespace JsonParser.Models
{
    [Table("rawtelemetrydata")]
    public class TelemetryItem
    {
        [ExplicitKey]
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string KundenNr { get; set; }
        public string IP { get; set; }
        public string ClientVersion { get; set; }
        public string ServerVersion { get; set; }
        public string Content { get; set; }
    }
}
