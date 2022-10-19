using System.IO;

namespace SqlStudio.ScriptExecuter
{
    public delegate void SqlCommandExecutionFailedDelegate(object sender, string query, string message, int lineNumber);
    public delegate void SqlExecuterProgressDelegate(object sender, int numRowsExecuted);

    public enum SqlScriptFileType
    {
        MsSql,
        Internal
    }

    public class ScriptExecuterFactory
    {
        public static IScriptExecuter GetExecuter(string filename)
        {
            switch (GetScriptType(filename))
            {
                case SqlScriptFileType.MsSql: return new MsSqlScriptExecuter(GetScriptReader(SqlScriptFileType.MsSql));
                default: return new InternalScriptExecuter(GetScriptReader(SqlScriptFileType.Internal));
            }
        }

        public static IScriptReader GetScriptReader(SqlScriptFileType fileType)
        {
            switch (fileType)
            {
                case SqlScriptFileType.MsSql: return new ScriptReaderMsSqlFormat();
                default:
                    return new ScriptReaderInternalFormat();
            }
        }

        private static SqlScriptFileType GetScriptType(string filename)
        {
            int lineNo = 0;
            using (var reader = new StreamReader(filename))
            {
                while (lineNo < 100)
                {
                    var line = reader.ReadLine();
                    if (line == "GO")
                    {
                        return SqlScriptFileType.MsSql;
                    }
                    lineNo++;

                    if (line == null)
                    {
                        return SqlScriptFileType.Internal;
                    }
                }

                return SqlScriptFileType.Internal;
            }
        }
    }
}
