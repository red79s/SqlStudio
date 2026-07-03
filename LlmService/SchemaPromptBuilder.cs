using System.Text;
using Common;

namespace LlmService
{
    /// <summary>
    /// Serializes an <see cref="IDatabaseSchemaInfo"/> into a compact text block suitable for
    /// including in an LLM prompt as database context.
    /// </summary>
    public static class SchemaPromptBuilder
    {
        /// <summary>
        /// Builds a human/LLM-readable description of the database schema: each table with its
        /// columns (type, nullability, primary-key marker) followed by a foreign-keys section.
        /// Returns an empty string when there is no schema information to describe.
        /// </summary>
        public static string Build(IDatabaseSchemaInfo? schema)
        {
            if (schema?.Tables == null || schema.Tables.Count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(schema.DatabaseName))
            {
                sb.Append("Database: ").AppendLine(schema.DatabaseName);
            }

            sb.AppendLine("Tables:");
            foreach (var table in schema.Tables)
            {
                if (table == null)
                {
                    continue;
                }

                sb.Append("- ").Append(table.TableName).AppendLine(" (");
                if (table.Columns != null)
                {
                    foreach (var column in table.Columns)
                    {
                        if (column == null)
                        {
                            continue;
                        }

                        sb.Append("    ").Append(column.ColumnName).Append(' ').Append(column.ColumnType);
                        if (column.IsPrimaryKey)
                        {
                            sb.Append(" PRIMARY KEY");
                        }
                        sb.Append(column.IsNullable ? " NULL" : " NOT NULL");
                        sb.AppendLine();
                    }
                }
                sb.AppendLine(")");
            }

            if (schema.ForeignKeys != null && schema.ForeignKeys.Count > 0)
            {
                sb.AppendLine("Foreign keys:");
                foreach (var fk in schema.ForeignKeys)
                {
                    if (fk == null)
                    {
                        continue;
                    }

                    sb.Append("- ")
                      .Append(fk.TableName).Append('.').Append(fk.ColumnName)
                      .Append(" -> ")
                      .Append(fk.ForeignTableName).Append('.').Append(fk.ForeignColumnName)
                      .AppendLine();
                }
            }

            return sb.ToString();
        }
    }
}
