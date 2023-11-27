using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace SqlStudio.EnumImporter
{
    public class EfEnumImporter
    {
        public List<ColumnDescription> Import(string fileName)
        {
            var res = new List<ColumnDescription>();

            var ass = Assembly.LoadFrom(fileName);
            var types = ass.GetTypes();

            foreach (var type in types) 
            {
                if (type.IsEnum)
                    continue;

                if (!type.IsClass)
                    continue;

                var enumColumns = GetEnumColumns(type);
                res.AddRange(enumColumns);
            }

            return res;
        }

        private List<ColumnDescription> GetEnumColumns(Type type)
        {
            var enumColumns = new List<ColumnDescription>();

            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (!property.PropertyType.IsEnum)
                {
                    continue;
                }

                var colDesc = new ColumnDescription
                {
                    AssemblyName = type.Assembly.GetName().Name,
                    TableName = GetTableName(type),
                    ColumnName = GetColumnName(property),
                    EnumValues = GetEnumValues(property.PropertyType)
                };

                if (colDesc.EnumValues.Count > 0)
                {
                    enumColumns.Add(colDesc);
                }
            }

            return enumColumns;
        }

        private string GetTableName(Type type)
        {
            var tableAttribute = type.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "TableAttribute");
            if (tableAttribute != null && tableAttribute.ConstructorArguments.Count > 0)
            {
                return tableAttribute.ConstructorArguments[0].Value.ToString();
            }
            return type.Name;
        }

        private string GetColumnName(PropertyInfo propType)
        {
            var columnAttribute = propType.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "ColumnAttribute");
            if (columnAttribute != null)
            {
                if (columnAttribute.ConstructorArguments.Count == 1) 
                    return columnAttribute.ConstructorArguments[0].ToString();
            }
            return propType.Name;
        }

        private List<EnumValue> GetEnumValues(Type type)
        {
            var enumValues = new List<EnumValue>();
            System.Type enumUnderlyingType = System.Enum.GetUnderlyingType(type);
            var values = Enum.GetValues(type);

            for (int i = 0; i < values.Length; i++)
            {
                // Retrieve the value of the ith enum item.
                object value = values.GetValue(i);

                // Convert the value to its underlying type (int, byte, long, ...)
                var underlyingValue = Convert.ChangeType(value, typeof(int));

                enumValues.Add(new EnumValue { Name = value.ToString(), Value = (int)underlyingValue });
            }

            return enumValues;
        }
    }
}
