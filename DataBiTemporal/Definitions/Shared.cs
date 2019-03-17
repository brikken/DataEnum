using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBiTemporal.Definitions
{
    public class ObjectId
    {
        public string Raw { get; set; }
        public string Name { get
            {
                if (Delimited(Raw))
                    return Raw.Substring(1, Raw.Length - 2);
                else
                    return Raw;
            }
        }

        private static bool Delimited(string id)
        {
            if (id == null) return false;
            return id.Substring(0, 1) == "[" && id.Substring(id.Length - 1, 1) == "]";
        }

        public override string ToString()
        {
            return Name;
        }

        public static ObjectId FromDef(string input)
        {
            return new ObjectId() { Raw = input };
        }
    }

    public class Defined<T>
    {
        public T Content { get; set; }
        public string Raw { get; set; }

        public static implicit operator T(Defined<T> d)
        {
            return d.Content;
        }

        public static implicit operator Defined<T>(T d)
        {
            return new Defined<T>() { Content = d };
        }
    }
}
