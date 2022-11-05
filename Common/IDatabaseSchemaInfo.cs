using System.Collections.Generic;

namespace Common
{
    public interface IDatabaseSchemaInfo
    {
        string DatabaseName { get; set; }
        IList <TableInfo> Tables { get; set; }
    }
}
