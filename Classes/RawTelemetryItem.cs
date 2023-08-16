using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemetry_parser
{
    [Table("rawtelemetrydata")]
    public class RawTelemetryItem
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public String? KundenNr { get; set; }
        public String? IP { get; set; }
        public String? ClientVersion { get; set; }
        public String? ServerVersion { get; set; }
        public String? Content { get; set; }
    }
}