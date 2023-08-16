namespace JsonParser.Models
{
    public class TableItem
    {
        public string Table { get; set; }
        public int Count { get; set; }
        public string Database { get; set; }

        public override string ToString()
        {
            return $"Table : {Table}, Amount : {Count}, Database : {Database} \n";
        }
    }
}
