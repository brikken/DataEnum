using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Antlr4.StringTemplate;

namespace DataEnum
{
    class Program
    {
        static void Main(string[] args)
        {
            var tds = TableDefinition.FromFile(@"table-def02.json");
            var stg = new TemplateGroupFile(new FileInfo(@"table-def.stg").FullName);
            foreach (var td in tds)
            {
                Console.WriteLine($"Table {td.name}");
                foreach (
                    (string name, string fullname)
                    in td.members
                        .SelectMany(m => m.GetMembersPrimitive()
                        .Select(mp => (name: m.name, fullname: mp.fullname))))
                {
                    Console.WriteLine($"{name}: {fullname}");
                }
                Console.WriteLine("==========");
                var ts = stg.GetInstanceOf("tablescript");
                ts.Add("td", td);
                Console.WriteLine(ts.Render());
            }
            Stack<string> test = new Stack<string>();
            test.Push("a"); test.Push("b"); test.Push("c");
            Console.WriteLine(string.Join("", test.Reverse()));
            while (Console.ReadKey().Key != ConsoleKey.Spacebar);
        }
    }

    class TableDefinition
    {
        public IList<MemberDefinition> memberDefinitions = new List<MemberDefinition>();
        public IList<Member> members = new List<Member>();
        public string memberSeparator = ".";
        public string name;
        public ICollection<DataType> dataTypes = new List<DataType>() {
            DataType.typeSig,
            new DataType() { name = "num" },
            new DataType() { name = "str" },
            new DataType() { name = "int" },
            new DataType() { name = "varchar(max)" }
        };

        public class AlternativeDefinition
        {
            public string name;
            public IList<MemberDefinition> fieldDefinitions;

            public Alternative AsAlternative(int index, Context context)
            {
                var alternative = new Alternative()
                {
                    name = name,
                    fullname = context.GetFullName(name),
                    fieldDefinitions = fieldDefinitions
                };

                context.fullnamePrefixes.Push(name);
                {
                    alternative.index = index;
                    alternative.fields = alternative.fieldDefinitions.Select(fd => fd.AsMember(context)).ToList();
                }
                context.fullnamePrefixes.Pop();

                return alternative;
            }
        }

        public class Alternative : AlternativeDefinition
        {
            public IList<Member> fields;
            public IList<MemberPrimitive> fieldsOther;
            public string fullname;
            public int index;

            public IList<MemberPrimitive> GetMembersPrimitive()
            {
                return fields.SelectMany(f => f.GetMembersPrimitive()).ToList();
            }
        }

        public class Context
        {
            public string FullnamePrefix {
                get {
                    return string.Join(memberSeparator, fullnamePrefixes.Reverse());
                }
            }
            public Stack<string> fullnamePrefixes = new Stack<string>();
            public string memberSeparator;

            public string GetFullName(string name)
            {
                return FullnamePrefix == "" ? name : FullnamePrefix + memberSeparator + name;
            }
        }

        public class DataType
        {
            // TODO: add to syntax
            public bool addSignal = true;
            private IList<AlternativeDefinition> alternatives = new List<AlternativeDefinition>();
            public IList<AlternativeDefinition> Alternatives
            {
                get
                {
                    return alternatives;
                }
                set
                {
                    alternatives = value;
                    if (value != null)
                    {
                        Primitive = false; 
                    }
                }
            }
            public bool Multi { get { return Alternatives.Count > 1; } }
            public string name;
            private bool primitive = true;
            public bool Primitive
            {
                get
                {
                    return primitive;
                }
                set {
                    CheckValid(value, Alternatives);
                    primitive = value;
                }
            }
            /// <summary>Should signal fields be added?</summary>
            public bool Signal { get { return Multi && addSignal; } }
            public static readonly DataType typeSig = new DataType() { name = "sig" };

            /// <summary>
            /// Assumes a particular combination of 'primitive' and 'alternatives' values is valid. If not, an exception is thrown.
            /// </summary>
            /// <exception cref="ArgumentException">If the combination is not valid</exception>
            /// <param name="primitive"></param>
            /// <param name="alternatives"></param>
            private static void CheckValid(bool primitive, object alternatives)
            {
                if (!((primitive && alternatives == null) || (!primitive && alternatives != null)))
                {
                    throw new ArgumentException("Data type must be either a primitive with no alternatives or a non-primitive with at least one alternative.");
                }
            }

            public IList<Alternative> GetAlternatives(Context context)
            {
                IList<Alternative> alternatives = new List<Alternative>();
                // populate all alternative with the own member instances
                for (var index = 0; index < Alternatives.Count; index++)
                {
                    alternatives.Add(Alternatives[index].AsAlternative(index, context));
                }

                // populate all alternatives with a list of member instances in the other alternatives
                foreach (var alternative in alternatives)
                {
                    alternative.fieldsOther = alternatives
                        .Where(a => a != alternative)
                        .SelectMany(ao => ao.GetMembersPrimitive())
                        .ToList();
                }

                return alternatives;
            }

            public string GetSignalName(Context context)
            {
                return name + context.memberSeparator + "s";
            }
        }

        public class MemberDefinition
        {
            public DataType dataType;
            public string dataTypeName;
            public string name;

            /// <summary>
            /// Returns the member definition as a Member instance
            /// </summary>
            /// <param name="fullnamePrefix"></param>
            /// <returns></returns>
            public Member AsMember(Context context)
            {
                string fullname = context.GetFullName(name);
                context.fullnamePrefixes.Push(name);
                Member member = new Member()
                {
                    dataType = dataType,
                    name = name,
                    fullname = fullname,
                    alternatives = dataType.GetAlternatives(context),
                    signal = dataType.Signal,
                    signalFullname = context.GetFullName(dataType.GetSignalName(context))
                };
                context.fullnamePrefixes.Pop();
                return member;
            }

            public MemberPrimitive AsMemberPrimitive()
            {
                return new MemberPrimitive()
                {
                    dataType = dataType,
                    name = name
                };
            }

            public MemberPrimitive AsMemberPrimitive(Context context)
            {
                var member = AsMemberPrimitive();
                member.fullname = context.GetFullName(name);
                return member;
            }
        }

        /// <summary>
        /// Instance of an abstract member, which may have underlying alternatives and primitive members
        /// </summary>
        public class Member : MemberDefinition
        {
            public IList<Alternative> alternatives = new List<Alternative>();
            public string fullname;
            public bool signal = false;
            public string signalFullname;

            public new MemberPrimitive AsMemberPrimitive()
            {
                var member = (this as MemberDefinition).AsMemberPrimitive();
                member.fullname = fullname;
                return member;
            }

            /// <summary>
            /// Returns the member itself and all submembers. Full names prefixed with provided value.
            /// </summary>
            public IList<MemberPrimitive> GetMembersPrimitive()
            {
                if (dataType.Primitive)
                {
                    return new List<MemberPrimitive>() { AsMemberPrimitive() };
                }
                else
                {
                    return alternatives.SelectMany(a => a.GetMembersPrimitive()).ToList();
                }
            }
        }

        /// <summary>
        /// Instance of a primitive member
        /// </summary>
        public class MemberPrimitive : MemberDefinition
        {
            public string fullname;
        }

        public static ICollection<TableDefinition> FromFile(string path)
        {
            var tables = new List<TableDefinition>();

            var ser = JsonSerializer.Create();
            var reader = new JsonTextReader(File.OpenText(path));
            var obj = ser.Deserialize(reader) as JObject;
            foreach (var prop in obj.Properties())
            {
                var table = new TableDefinition
                {
                    name = prop.Name
                };
                var defObj = prop.Value as JObject;
                foreach (var defProp in defObj.Properties())
                {
                    switch (defProp.Name)
                    {
                        case "types":
                            var typesObj = defProp.Value as JObject;
                            foreach (var typeProp in typesObj.Properties())
                            {
                                var type = new DataType
                                {
                                    name = typeProp.Name,
                                    Alternatives = new List<AlternativeDefinition>()
                                };
                                var altsObj = typeProp.Value as JObject;
                                foreach (var altProp in altsObj.Properties())
                                {
                                    var alt = new AlternativeDefinition
                                    {
                                        name = altProp.Name,
                                        fieldDefinitions = (altProp.Value as JObject)
                                            .Properties()
                                            .Select(elem => new MemberDefinition()
                                            {
                                                name = elem.Name,
                                                dataTypeName = (elem.Value as JValue).Value as string
                                            })
                                            .ToList()
                                    };
                                    type.Alternatives.Add(alt);
                                }
                                table.dataTypes.Add(type);
                            }
                            break;
                        case "members":
                            var memsObj = defProp.Value as JObject;
                            foreach (var memProp in memsObj.Properties())
                            {
                                var mem = new MemberDefinition
                                {
                                    name = memProp.Name,
                                    dataTypeName = (memProp.Value as JValue).Value as string
                                };
                                table.memberDefinitions.Add(mem);
                            }
                            break;
                        case "settings":
                            var setObj = defProp.Value as JObject;
                            foreach (var setProp in setObj.Properties())
                            {
                                switch (setProp.Name)
                                {
                                    case "member-separator":
                                        table.memberSeparator = (setProp.Value as JValue).Value as string;
                                        break;
                                    default:
                                        throw new NotSupportedException($"Unknown setting: {setProp.Name}");
                                }
                            }
                            break;
                        default:
                            throw new NotSupportedException("Only 'types' and 'members' are allowed in table definitions");
                    }
                }

                foreach (var dataType in table.dataTypes.Where(dt => !dt.Primitive))
                {
                    foreach (var alt in dataType.Alternatives)
                    {
                        for (var index = 0; index < alt.fieldDefinitions.Count; index++)
                        {
                            MemberDefinition field = alt.fieldDefinitions[index];
                            field.dataType = table.dataTypes.Single(dt => dt.name == field.dataTypeName);
                        }
                    }
                }

                foreach (var member in table.memberDefinitions)
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

                // create member instances from definitions
                Context context = table.GetContext();
                table.members = table.memberDefinitions
                    .Select(md => md.AsMember(context))
                    .ToList();

                tables.Add(table);
            }

            return tables;
        }

        public Context GetContext()
        {
            return new Context() { memberSeparator = memberSeparator };
        }
    }
}
