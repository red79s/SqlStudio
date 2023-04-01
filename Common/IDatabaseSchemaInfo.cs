using System.Collections.Generic;

namespace Common
{
    public interface IDatabaseSchemaInfo
    {
        string ConnectionString { get; }
        string DatabaseName { get; }
        IList <TableInfo> Tables { get; }
    }
}
