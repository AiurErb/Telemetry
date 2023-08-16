using Microsoft.AspNetCore.Mvc;
using JsonParser.Data;
using JsonParser.Models;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace JsonParser.Controllers
{
    

    public class TreeNode
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Name} : {Value}";
        }
    }
    public class TelemetryController : Controller
    {
        private List<TelemetryNode> _nodes { get; set; } = new List<TelemetryNode>();
        private List<TreeNode> _tree { get; set; } = new List<TreeNode>();
        public async Task<ActionResult> ListAsync()
        {
            TelemetryDb db = new();
            List<TelemetryItem> items = await db.GetItems();
            return View(items);
        }
        public ActionResult Details([FromRoute] int id)
        {
            TelemetryDb db = new();
            JsonElement content = db.GetContent(id);
            TablesAndProperties(content, 0);
            return View(_nodes);
        }
        public async Task<string> IndexAsync()
        {
            TelemetryDb db = new TelemetryDb();
            List<TelemetryItem> items = await db.GetItems();
            string output = "";
            foreach (TelemetryItem item in items)
            {
                output += item.KundenNr.ToString() + "\n";
                output += item.CreateDate.ToString() + "\n";
                _nodes.Clear();
                using JsonDocument doc = JsonDocument.Parse(item.Content);
                JsonElement root = doc.RootElement;
                ObjectTree(root, 0);

                foreach (TreeNode node in _tree)
                {
                    output += node + "\n";
                }                

            }
            return output;
        }
        public void TablesAndProperties(JsonElement element, int level)
        {
            JsonValueKind type = element.ValueKind;
            if (type == JsonValueKind.Array)
            {
                var items = element.EnumerateArray();
                int i = 1;
                int n = items.Count();
                foreach (var item in items)
                {
                    _nodes.Add(new TelemetryNode
                    {
                        Type = "Subcategory Level " +level,
                        Output = $"[{i}] of",
                        Value = n.ToString(),
                        Level = level
                    }) ;
                    i++;
                    TablesAndProperties(item, level+1);
                }
            }
            if (isTable(element))
            {
                string tableName ="";
                JsonElement rows = element.GetProperty("Rows");
                int count = rows.GetArrayLength();
                var props = element.EnumerateObject();
                while (props.MoveNext())
                {
                    if (props.Current.Name == "Name")
                    {
                        tableName = props.Current.Value.ToString();
                    }
                }
                _nodes.Add(new TelemetryNode
                {
                    Type = "Table:",
                    Output = tableName,
                    Value = count.ToString(),
                    Level = level
                });
                return;
            }
            if (type == JsonValueKind.Object)
            {
                var props = element.EnumerateObject();
                foreach (var prop in props)
                {
                    bool complexObj = prop.Value.ValueKind == JsonValueKind.Array ||
                        prop.Value.ValueKind == JsonValueKind.Object;
                    if (complexObj)
                    {
                        _nodes.Add(new TelemetryNode
                        {
                            Type = "Property",
                            Output = prop.Name,
                            Value = "Complex",
                            Level = level
                        });
                        TablesAndProperties(prop.Value, level+1);
                    }
                    else
                    {
                        _nodes.Add(new TelemetryNode
                        {
                            Type = "Property",
                            Output = prop.Name,
                            Value = prop.Value.ToString(),
                            Level = level
                        });
                    }
                    
                    
                }
            }
        }
        public bool isTable(JsonElement element)
        {
            bool flagName = false;
            bool flagRows = false;
            if (element.ValueKind == JsonValueKind.Array)
            {
                return false;
            }
            var props = element.EnumerateObject();
            foreach (var prop in props)
            {
                if (prop.Name == "Name") flagName = true;
                if (prop.Name == "Rows") flagRows = true;
            }
            if (flagName && flagRows)
            {
                return true;
            }
            return false;
        }
        public void ObjectTree (JsonElement element, int level)
        {
            string indent = "";
            string value = "";
            //TreeNode node = new TreeNode();
            for (int i = 0; i < level; i++)
            {
                indent += "\t";
            }
            JsonValueKind type = element.ValueKind;

            string name;
            if (type == JsonValueKind.Array)
            {
                var arr = element.EnumerateArray();
                int i = 0;
                while (arr.MoveNext())
                {
                    name = $"{indent}[{i}]";
                    value = "Array";
                    _tree.Add(TableDetekt(arr.Current, indent, i));
                    i++;
                    ObjectTree(arr.Current, level + 1);
                }
            }

            if (type == JsonValueKind.Object)
            {
                var obj = element.EnumerateObject();
                while (obj.MoveNext())
                {
                    if (obj.Current.Value.ValueKind == JsonValueKind.Array)
                    {
                        name = indent + obj.Current.Name;
                        Console.WriteLine(obj.Current.Name);
                        value = "Array";
                        _tree.Add(new TreeNode { Name = name, Value = value });
                        ObjectTree(obj.Current.Value, level + 1);
                        
                    }else if (obj.Current.Value.ValueKind == JsonValueKind.Object)
                    {
                        name = indent + obj.Current.Name;
                        Console.WriteLine(obj.Current.Name);
                        value = "Object";
                        _tree.Add(new TreeNode { Name = name, Value = value });
                        ObjectTree(obj.Current.Value, level + 1);

                    }
                    else {
                        name = indent + obj.Current.Name;
                        value = obj.Current.Value.ToString();
                        _tree.Add(new TreeNode { Name = name, Value = value });
                    }                    
                }
            }
        }
        public TreeNode TableDetekt(JsonElement element, string indent, int index)
        {
            string result = "";
            bool flagName = false;
            bool flagRows = false;
            if (element.ValueKind == JsonValueKind.Object)
            {
                var props = element.EnumerateObject();
                foreach (var property in props)
                {
                    if (property.Name == "Name")
                    {
                        flagName = true;
                        result = property.Value.ToString();
                    }
                    if (property.Name =="Rows") flagRows = true;
                }
            }
            if (flagName && flagRows)
            {
                return new TreeNode { Name = indent + "Table:", Value = result };
            }
            else
            {
                return new TreeNode { Name = indent+"Node", Value = $"[{index}]" };
            }            
        }
    }
}
