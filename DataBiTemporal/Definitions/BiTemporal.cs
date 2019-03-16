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
        public ICollection<ITableOption> Options { get; set; } = new Collection<ITableOption>();
        public IList<Column> Columns { get; set; } = new List<Column>();
        public IList<Column> PrimaryKey { get; set; } = new List<Column>();
    }

    public interface ITableOption { }

    public class DtWithOption : ITableOption
    {
        public ICollection<IDtWithOption> Options { get; set; } = new Collection<IDtWithOption>();
    }

    public interface IDtWithOption { }

    public class BiTemporalOption : IDtWithOption
    {
        public ObjectId BtSchema { get; set; }
    }

    public class Column
    {
        public ObjectId Name { get; set; }
        public Type Type { get; set; }
        public ICollection<ColumnOption> Options { get; set; } = new Collection<ColumnOption>();
    }

    public enum Type
    {
        INT,
        VARCHAR,
        DECIMAL
    }

    public enum ColumnOption
    {
        NULL,
        NOT_NULL,
        PRIMARY_KEY
    }
}
