using Common;
using Common.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SqlStudio.ColumnMetaDataInfo
{
    public class ColumnValueDescriptionManager : IColumnValueDescriptionProvider
    {
        private const string CasheFileName = "ColumnValueDescriptionCache.json";
        private List<ColumnValueDescription> _columnValueDescriptions = new List<ColumnValueDescription>();
        private Dictionary<string, string> _columnValueDescriptionsDict = new Dictionary<string, string>();
        public void AddColumnMetadataInfo(string source, List<ColumnValueDescription> columnValues)
        {
            _columnValueDescriptions.RemoveAll(x => x.Source == source);
            foreach (var columnValueDescription in columnValues)
            {
                columnValueDescription.Source = source;
                _columnValueDescriptions.Add(columnValueDescription);
            }
            ReloadDict();
        }

        public string GetDescriptionForValue(string tableName, string columnName, string value)
        {
            string key = $"{tableName}|{columnName}|{value}";
            if (_columnValueDescriptionsDict.ContainsKey(key))
            {
                return _columnValueDescriptionsDict[key];
            }

            return value;
        }

        public List<string> GetDescriptionForColumn(string tableName, string columnName)
        {
            var descriptions = new List<string>();
            var posibleValues = _columnValueDescriptions.Where(x => x.TableName == tableName && x.ColumnName == columnName).OrderBy(x => x.Value).ToList();
            foreach (var val in posibleValues)
            {
                descriptions.Add($"{val.Value} - {val.Description}");
            }
            return descriptions;
        }

        public void Load()
        {
            if (!File.Exists(CasheFileName))
                return;
            var fileContent = File.ReadAllText(CasheFileName);
            _columnValueDescriptions = JsonSerializer.Deserialize<List<ColumnValueDescription>>(fileContent);

            ReloadDict();
        }

        private void ReloadDict()
        {
            _columnValueDescriptionsDict.Clear();
            foreach (var columnValueDescription in _columnValueDescriptions)
            {
                _columnValueDescriptionsDict[$"{columnValueDescription.TableName}|{columnValueDescription.ColumnName}|{columnValueDescription.Value}"] = columnValueDescription.Description;
            }
        }

        public void Save()
        {
            var fileContent = JsonSerializer.Serialize( _columnValueDescriptions );
            File.WriteAllText(CasheFileName, fileContent);
        }
    }
}
