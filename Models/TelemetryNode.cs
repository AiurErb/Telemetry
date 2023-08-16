namespace JsonParser.Models
{
    public class TelemetryNode
    {
        public string Type { get; set; }
        public string Output { get; set; }
        public string Value { get; set; }
        public int Level { get; set; }

        public override string ToString()
        {
            return $"Type: {Type}, Description {Output}, Value {Value}";
        }
    }
}
