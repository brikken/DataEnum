using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Antlr4.StringTemplate;

namespace DataEnum
{
    class Program
    {
        static void Main(string[] args)
        {
            var tds = TableDefinition.FromFile(@"table-def01.json");
            foreach (var td in tds)
            {
                Console.WriteLine($"Table {td.name}");
                foreach (
                    (string name, string fullname)
                    in td.members
                        .SelectMany(m => m.GetFullNames()
                        .Select(fn => (m.name, fn))))
                {
                    Console.WriteLine($"{name}: {fullname}");
                }
            }
            while (Console.ReadKey().Key != ConsoleKey.Spacebar);
        }
    }

    class TableDefinition
    {
        public IList<Member> members = new List<Member>();
        public string name;
        public ICollection<DataType> dataTypes = new List<DataType>() {
            new DataType() { name = "num", primitive = true },
            new DataType() { name = "str", primitive = true }
        };

        public class DataType
        {
            public IList<Alternative> alternatives;
            public string name;
            public bool primitive = false;

            public class Alternative
            {
                public string name;
                public IList<Member> fields;
            }

            public List<string> GetFullNames()
            {
                if (primitive)
                {
                    return new List<string>() { name };
                }
                else
                {
                    return alternatives.SelectMany(a => a.fields.SelectMany(f => f.GetFullNames())).ToList();
                }
            }
        }

        public class Member
        {
            public DataType dataType;
            public string dataTypeName;
            public string name;

            public List<string> GetFullNames()
            {
                return dataType.GetFullNames().Select(n => name + '.' + n).ToList();
            }
        }

        public static ICollection<TableDefinition> FromFile(string path)
        {
            var tables = new List<TableDefinition>();

            var ser = JsonSerializer.Create();
            var reader = new JsonTextReader(File.OpenText(path));
            var obj = ser.Deserialize(reader) as JObject;
            foreach (var prop in obj.Properties())
            {
                var table = new TableDefinition();
                table.name = prop.Name;
                var defObj = prop.Value as JObject;
                foreach (var defProp in defObj.Properties())
                {
                    switch (defProp.Name)
                    {
                        case "types":
                            var typesObj = defProp.Value as JObject;
                            foreach (var typeProp in typesObj.Properties())
                            {
                                var type = new DataType();
                                type.name = typeProp.Name;
                                type.alternatives = new List<DataType.Alternative>();
                                var altsObj = typeProp.Value as JObject;
                                foreach (var altProp in altsObj.Properties())
                                {
                                    var alt = new DataType.Alternative();
                                    alt.name = altProp.Name;
                                    alt.fields = (altProp.Value as JArray)
                                        .Select(elem => new Member() { dataTypeName = (elem as JValue).Value as string })
                                        .ToList();
                                    type.alternatives.Add(alt);
                                }
                                table.dataTypes.Add(type);
                            }
                            break;
                        case "members":
                            var memsObj = defProp.Value as JObject;
                            foreach (var memProp in memsObj.Properties())
                            {
                                var mem = new Member();
                                mem.name = memProp.Name;
                                mem.dataTypeName = (memProp.Value as JValue).Value as string;
                                table.members.Add(mem);
                            }
                            break;
                        default:
                            throw new NotSupportedException("Only 'types' and 'members' are allowed in table definitions");
                    }
                }

                foreach (var dataType in table.dataTypes.Where(dt => !dt.primitive))
                {
                    foreach (var alt in dataType.alternatives)
                    {
                        for (var index = 0; index < alt.fields.Count; index++)
                        {
                            Member field = alt.fields[index];
                            field.dataType = table.dataTypes.Single(dt => dt.name == field.dataTypeName);
                            field.name = alt.name + ':' + index.ToString();
                        }
                    }
                }

                foreach (var member in table.members)
                {
                    try
                    {
                        member.dataType = table.dataTypes.Single(dt => dt.name == member.dataTypeName);
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine($"Unrecognized type {member.dataTypeName} for member {member.name} in table {table.name}");
                        throw;
                    }
                }

                tables.Add(table);
            }

            return tables;
        }
    }
}
