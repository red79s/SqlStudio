using System.Collections.Generic;

namespace Common
{
    public interface IDatabaseSchemaInfo
    {
        string DatabaseName { get; }
        IList <TableInfo> Tables { get; }
    }
}
