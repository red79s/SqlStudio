using System.Collections.Generic;

namespace SqlStudio.Converters
{
    public class ConvertCvsDataToLogRecords
    {
        private readonly List<List<string>> _cvsData;

        public ConvertCvsDataToLogRecords(List<List<string>> cvsData)
        {
            _cvsData = cvsData;
        }

        private int GetColumnIndex(string column)
        {
            for(int i = 0; i < _cvsData[0].Count; i++)
            {
                if (_cvsData[0][i] == column)
                    return i;
            }
            return -1;
        }

        private string GetData(int row, int column)
        {
            if (column < 0)
            {
                return "";
            }

            return _cvsData[row][column];
        }

        private string GetDateTime(int row, int column)
        {
            var colVal = GetData(row, column);
            var index = colVal.LastIndexOf('.');
            if (index > 0)
            {
                int charsRightOfDot = colVal.Length - index;
                if (charsRightOfDot > 4)
                {
                    colVal = colVal.Substring(0, colVal.Length - (charsRightOfDot - 4));
                }
            }

            return colVal;
        }

        public List<string> CreateInsertCommands()
        {
            var res = new List<string>();

            var applicationIndex = GetColumnIndex("Application");
            var clientIdIndex = GetColumnIndex("ClientId");
            var clientLogId = GetColumnIndex("ClientLogId");
            var logLevelIndex = GetColumnIndex("LogLevel");
            var logAreaIndex = GetColumnIndex("LogArea");
            var logDateIndex = GetColumnIndex("date");
            var logDateLocalIndex = GetColumnIndex("LogDateLocal");
            var threadIdIndex = GetColumnIndex("ThreadId");
            var sessionIdIndex = GetColumnIndex("SessionId");
            var messageIndex = GetColumnIndex("message");
            var dataIndex = GetColumnIndex("Data");

            for (int i = 1; i < _cvsData.Count; i++)
            {
                var row = $"INSERT INTO ApplicationLog (TenantId,Application,ClientId,ClientLogId,LogArea,LogLevel,LogDate,LogDateLocal,ThreadId,SessionId,Message,Data) VALUES('Imported'," +
                    $"'{GetData(i, applicationIndex)}'" +
                    $",'{GetData(i, clientIdIndex)}',{GetData(i, clientLogId)},'{GetData(i, logAreaIndex)}','{GetData(i, logLevelIndex)}','{GetDateTime(i, logDateIndex)}'," +
                    $"'{GetDateTime(i, logDateLocalIndex)}','{GetData(i, threadIdIndex)}','{GetData(i, sessionIdIndex)}','{GetData(i, messageIndex)}','{GetData(i, dataIndex)}');";
                res.Add(row);
            }

            return res;
        }
    }
}
