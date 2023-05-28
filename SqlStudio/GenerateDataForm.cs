using SqlExecute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.Mapping;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SqlStudio
{
    public partial class GenerateDataForm : Form
    {
        private readonly SqlExecuter _sqlExecuter;

        public GenerateDataForm(SqlExecute.SqlExecuter sqlExecuter)
        {
            InitializeComponent();
            _sqlExecuter = sqlExecuter;
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            var sqlCmd = sqlScriptComponent.GetSelectedText();
            int startRowNo = StartIndex;
            int endRowNo = StartIndex + NumberOfInserts;
            int batchNumberOfRows = BatchNumberOfRows;
            if (batchNumberOfRows < 1)
            {
                batchNumberOfRows = 1;
            }
            int commitInterval = CommitInterval;
            if (commitInterval < 0)
            {
                commitInterval = 1;
            }

            if (endRowNo <= startRowNo)
            {
                MessageBox.Show($"NumberOfInserts are less or equal to 0");
                return;
            }

            btnClose.Enabled = false;
            btnRun.Enabled = false;

            await Task.Run(() =>
            {
                DbTransaction trans = null;
                try
                {

                    trans = _sqlExecuter.Connection.BeginTransaction();
                    var lastCommit = startRowNo;
                    for (int i = startRowNo; i <= endRowNo; i += batchNumberOfRows)
                    {
                        var cmd = GetSqlCommand(sqlCmd, i, i + batchNumberOfRows);
                        var res = _sqlExecuter.ExecuteSql(cmd, trans);
                        if (!res.Success)
                        {
                            throw new Exception($"Failed to execute: {res.Message}, cmd: {cmd}");
                        }
                        if (i >= (lastCommit + commitInterval))
                        {
                            trans.Commit();
                            trans = _sqlExecuter.Connection.BeginTransaction();
                            lastCommit = i;

                            WriteStatus($"Inserted {i - startRowNo} rows of {startRowNo - endRowNo} rows");
                        }
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to execute: {ex}");
                    trans.Rollback();
                }
            });

            btnClose.Enabled = true;
            btnRun.Enabled = true;
        }

        private void WriteStatus(string msg)
        {
            tbStatus.BeginInvoke(() => { tbStatus.Text = msg; });
        }

        private int StartIndex
        {
            get
            {
                int startIndex = 0;
                int.TryParse(tbStartIndex.Text, out startIndex);
                return startIndex;
            }
        }

        private int NumberOfInserts
        {
            get
            {
                int numberOfInserts = 0;
                int.TryParse(tbNumberOfInserts.Text, out numberOfInserts);
                return numberOfInserts;
            }
        }

        private int BatchNumberOfRows
        {
            get
            {
                int batchNumberOfRows = 1;
                int.TryParse(tbBatchNumberOfRows.Text, out batchNumberOfRows);
                return batchNumberOfRows;
            }
        }

        private int CommitInterval
        {
            get
            {
                int commitInterval = 1;
                int.TryParse(tbCommitInterval.Text, out commitInterval);
                return commitInterval;
            }
        }

        string ReplaceMacro(string value, int rowNo)
        {
            return Regex.Replace(value, @"{(?<exp>[^}]+)}", match =>
            {
                var p = Expression.Parameter(typeof(int), "rowNo");
                var e = System.Linq.Dynamic.Core.DynamicExpressionParser.ParseLambda(new[] { p }, null, match.Groups["exp"].Value);
                return (e.Compile().DynamicInvoke(rowNo) ?? "").ToString();
            });
        }

        private string GetValuesString(string sqlCmd)
        {
            int index = sqlCmd.IndexOf("values", StringComparison.InvariantCultureIgnoreCase);
            if (index == -1)
            {
                return null;
            }
            int openIndex = sqlCmd.IndexOf("(", index, StringComparison.CurrentCultureIgnoreCase);
            if (openIndex == -1)
            {
                return null;
            }

            return sqlCmd.Substring(openIndex);
        }

        private string GetSqlCommand(string cmd, int startRowNo, int endRowNo)
        {
            var valuesStr = GetValuesString(cmd);
            var sqlCmd = ReplaceMacro(cmd, startRowNo);
            for (int i = startRowNo + 1; i < endRowNo; i++)
            {
                var valuesStrReplaced = ReplaceMacro(valuesStr, i);
                sqlCmd += ", " + valuesStrReplaced;
            }
            return sqlCmd;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
