namespace JsonParser.Models
{
    public class Customer
    {
        public string KundenNr { get; set; }
        public int TelemetryIdCount { get; set; }
    }

    public class CustomerDetails
    {
        public string KundenNr { get; set; }
        public List<Vppj> VorgaengeneProjecte { get; set; }
    }

    public class Vppj
    {
        public int TelemetryId { get; set; }
        public int ObjNr { get; set; }
        public int Jahr { get; set; }
        public float VorgaengeProObjektProJahre { get; set; }
    }
}
