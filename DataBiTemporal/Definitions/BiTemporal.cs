using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBiTemporal.Definitions;

namespace DataBiTemporal.Definitions
{
    public class BiTemporal
    {
        public ObjectId Table { get; set; }
        public ObjectId Database { get; set; }
        public ObjectId Schema { get; set; }
        public ObjectId BtSchema { get; set; }
        public IList<Column> Columns { get; set; } = new List<Column>();
        public IList<Column> PrimaryKey { get; set; } = new List<Column>();
    }

    public class Column
    {
        public ObjectId Id { get; set; }
        public string Type { get; set; }
        public Defined<ICollection<ColumnOption>> Options { get; set; } = new Collection<ColumnOption>();
    }

    public enum ColumnOption
    {
        NULL,
        NOT_NULL,
        PRIMARY_KEY
    }
}
