using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGeneratorLib
{
    class DBTypeConverter
    {
        public static string GetDotNetType(string dbType)
        {
            switch (dbType.ToLower())
            {
                case "varchar":
                case "nvarchar":
                case "char":
                case "nchar":
                case "text":
                case "character varying":
                    return "string";

                case "integer":
                case "int":
                    return "int";

                case "tinyint":
                    return "byte";

                case "float":
                    return "float";

                case "numeric":
                case "decimal":
                    return "decimal";

                case "double precision":
                    return "double";

                case "long":
                case "bigint":
                    return "long";

                case "bool":
                case "binary":
                    return "bool";

                case "blob":
                case "varbinary":
                case "oid":
                    return "byte[]";

                case "uniqueidentifier":
                    return "System.Guid";

                case "datetime":
                case "timestamp":
                case "timestamp without time zone":
                    return "System.DateTime";

                default:
                    throw new Exception("Database type \"" + dbType + "\" not recognized");
            }
        }
    }
}
