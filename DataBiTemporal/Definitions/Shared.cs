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
            return id.Substring(0, 1) == "[" && id.Substring(id.Length - 1, 1) == "]";
        }
    }
}
