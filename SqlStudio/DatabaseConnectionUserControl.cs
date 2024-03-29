﻿using CfgDataStore;
using Common;
using SqlCommandCompleter;
using SqlExecute;
using SqlStudio.ColumnMetaDataInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlStudio
{
    public partial class DatabaseConnectionUserControl : UserControl, IExecuteQueryCallback
    {
        private Executer _executer = null;
        private ConfigDataStore _cfgDataStore = null;
        private SyntaxHighlight.SQLSyntaxHighlight _syntaxHighLight = null;
        private BuiltInCommands _builtIn = null;
        private List<string> _cmdBuffer = null;
        private Timer _executeTimer = new Timer();
        private bool _bulkExecute = false;
        private Stopwatch _bulkTimer = new Stopwatch();
        private int _bulkErrors = 0;
        private int _bulkOk = 0;
        private DateTime _excutionStarted;
        private string _userConfigDbFile;
        private ISqlCompleter _sqlCompleter = null;
        private IDatabaseKeywordEscape _databaseKeywordEscape = null;
        private IColumnValueDescriptionProvider _columnMetadataInfo = null;
        private readonly ILogger _logger;

        public DatabaseConnectionUserControl(ILogger logger)
        {
            InitializeComponent();

            _databaseKeywordEscape = new DatabaseKeywordEscapeManager();
            _columnMetadataInfo = new ColumnValueDescriptionManager();
            _columnMetadataInfo.Load();

            _cmdBuffer = new List<string>();
            _userConfigDbFile = Directory.GetParent(Application.UserAppDataPath) + @"\sqlstudio.cfg";
            string defaultCfgPath = Application.StartupPath + @"\sqlstudio.cfg";
            if (!File.Exists(_userConfigDbFile))
            {
                if (File.Exists(defaultCfgPath))
                    File.Copy(defaultCfgPath, _userConfigDbFile);
                else
                {
                    MessageBox.Show($"Config file is missing: {_userConfigDbFile} and default config file is not found: {defaultCfgPath}");
                    Application.Exit();
                    return;
                }
            }
            _cfgDataStore = new ConfigDataStore(_userConfigDbFile);

            _executer = new Executer(cmdLineControl, _cfgDataStore);
            _executer.ExecutionFinished += new Executer.ExecutionFinishedDelegate(_executer_ExecutionFinished);

            _sqlCompleter = new SqlCompleter(_logger, _executer.SqlExecuter, _databaseKeywordEscape);

            _syntaxHighLight = new SyntaxHighlight.SQLSyntaxHighlight();
            _syntaxHighLight.DefaultColor = cmdLineControl.ForeColor;
            _syntaxHighLight.IdentifiersColor = cmdLineControl.ForeColor;
            _syntaxHighLight.NumbersColor = Color.Green;
            _syntaxHighLight.StringsColor = Color.Red;
            _syntaxHighLight.KeyWordsColor = Color.Blue;

            cmdLineControl.CommandReady += new CommandPrompt.CmdLineControl.CommandReadyDelegate(cmdLineControl_CommandReady);
            cmdLineControl.CommandCompletion += new CommandPrompt.CmdLineControl.CommandCompletionDelegate(cmdLineControl_CommandCompletion);
            cmdLineControl.TextInserted += new FormatTextControl.FormatTextControl.TextInsertedDelegate(cmdLineControl_TextInserted);
            cmdLineControl.SetHistoryItems(_cfgDataStore);
            cmdLineControl.InsertSnipit += CmdLineControl_InsertSnipit;

            sqlOutput.SetDependencyObjects(_cfgDataStore, this, _executer.SqlExecuter, _databaseKeywordEscape, _columnMetadataInfo);
            sqlOutput.UpdatedResults += new SqlOutputTabContainer.UpdatedResultsDelegate(sqlOutput_UpdatedResults);
            sqlOutput.VisibleRowsChanged += (s, e) => { visibleRowsToolStripStatusLabel.Text = $"{e}"; };

            tabControlMainDocs.MouseDown += new MouseEventHandler(tabControlMainDocs_MouseDown);

            _builtIn = new BuiltInCommands(cmdLineControl, _cfgDataStore);

            _executeTimer.Interval = 1000;
            _executeTimer.Tick += ExecuteTimerOnTick;

            cmdLineControl.GetCommand();
            _logger = logger;
        }

        public void ExecuteQuery(string query, bool inNewTab, string datatabLabel)
        {
            if (inNewTab)
            {
                sqlOutput.CreateNewDataTab(datatabLabel);
            }

            cmdLineControl.InsertCommandOutput(query);

            cmdLineControl_CommandReady(this, query);
        }

        private void CmdLineControl_InsertSnipit(object sender, CommandPrompt.InsertSnipitEventArgs e)
        {
            if (!e.Args.Control)
                return;

            var aliasName = "CTR+" + Char.ToUpper((char)e.Args.KeyValue);
            var alias = _cfgDataStore.GetAlias(aliasName);
            if (string.IsNullOrEmpty(alias))
                return;

            e.InsertText = alias;
            e.Handled = true;
        }

        void tabControlMainDocs_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 1; i < tabControlMainDocs.TabPages.Count; i++)
                {
                    if (tabControlMainDocs.GetTabRect(i).Contains(e.Location))
                    {
                        tabControlMainDocs.SelectedTab = tabControlMainDocs.TabPages[i];

                        cmScriptTabs.Show(tabControlMainDocs, e.Location);

                        return;
                    }
                }
            }
        }

        void sqlOutput_UpdatedResults(object sender, int rows, string message)
        {
            toolStripMessageLabel.Text = message;
        }

        void cmdLineControl_TextInserted(object sender, FormatTextControl.TextPos start, FormatTextControl.TextPos end, string text)
        {
            for (int line = start.Line; line <= end.Line; line++)
            {
                List<SyntaxHighlight.Token> syntaxTokens = _syntaxHighLight.GetTokens(cmdLineControl.GetText(line));
                cmdLineControl.RemoveFormating(line);
                foreach (SyntaxHighlight.Token token in syntaxTokens)
                {
                    if (token.Color != cmdLineControl.ForeColor)
                        cmdLineControl.AddFormating(line, token.LineIndex, token.Value.Length, token.Color);
                }
                cmdLineControl.InvalidateLine(line);
            }
        }

        void cmdLineControl_CommandReady(object sender, string cmd)
        {
            try
            {
                CommandParser commandParser = new CommandParser();
                List<string> cmdLines = commandParser.GetCommands(cmd);

                if (!_bulkExecute)
                {
                    _bulkExecute = _cmdBuffer.Count > 0;
                    if (_bulkExecute)
                    {
                        _bulkTimer.Reset();
                        _bulkTimer.Start();
                        _bulkOk = 0;
                        _bulkErrors = 0;
                    }
                }

                var haveBuiltIn = _builtIn.HandleBuiltIn(ref cmdLines);

                if (cmdLines.Count > 0)
                {
                    if (haveBuiltIn)
                    {
                        foreach (var cmdLine in cmdLines)
                        {
                            cmdLineControl.InsertCommandOutput(Environment.NewLine + cmdLine);
                        }
                    }
                    _executer.Execute(cmdLines.ToArray());
                    _excutionStarted = DateTime.Now;
                    _executeTimer.Start();
                }
                else
                    cmdLineControl.GetCommand();
            }
            catch (Exception ex)
            {
                cmdLineControl.InsertCommandOutput(Environment.NewLine + ex.Message);
                cmdLineControl.GetCommand();
            }
        }

        CommandCompletionResult cmdLineControl_CommandCompletion(object sender, string cmd, int index)
        {
            int lastCmdSep = CommandParser.GetLastCmdSeperator(cmd, ';', index);

            string realCmd = cmd;
            int realIndex = index;
            if (lastCmdSep > 0)
            {
                realCmd = cmd.Substring(lastCmdSep + 1, cmd.Length - (lastCmdSep + 1));
                realIndex = index - (lastCmdSep + 1);
            }

            if (realCmd.Length > 0 && realCmd[0] == ':')
            {
                realCmd = realCmd.Substring(1, realCmd.Length - 1);
                realIndex--;
            }

            return _sqlCompleter.GetPossibleCompletions(realCmd, realIndex);
        }

        private void ExecuteTimerOnTick(object sender, EventArgs e)
        {
            toolStripMessageLabel.Text = $"Executing: {DateTime.Now.Subtract(_excutionStarted).ToString(@"hh\:mm\:ss")}";
        }

        void _executer_ExecutionFinished(object sender, List<SqlResult> results)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Executer.ExecutionFinishedDelegate(_executer_ExecutionFinished), new object[] { sender, results });
                return;
            }

            _executeTimer.Stop();

            if (results == null)
            {
                toolStripMessageLabel.Text = "Invalid result, not processing";

                cmdLineControl.ClearUndoHistory();
                return;
            }

            foreach (SqlResult res in results)
            {
                if (res.Success)
                {
                    if (res.ResType == SqlResult.ResultType.CONNECT)
                    {
                        string msg = string.Format("Connected OK, took {0}", ConvertMillisecondsToHumanReadableString(res.ExecutionTimeMS));
                        cmdLineControl.InsertCommandOutput("\n" + msg);
                        toolStripMessageLabel.Text = msg;
                        Text = "SqlStudio - " + res.ServerName + ":" + res.DataBaseName;

                        var timeout = _cfgDataStore.GetLongValue("timeout");
                        if (timeout > 0)
                        {
                            _cmdBuffer.Add($"set timeout {timeout}");
                        }

                        var databases = _executer.GetDatabases();
                    }
                    else if (res.ResType == SqlResult.ResultType.DISCONNECT)
                    {
                        string msg = string.Format("Disconnected OK, took {0}", ConvertMillisecondsToHumanReadableString(res.ExecutionTimeMS));
                        cmdLineControl.InsertCommandOutput("\n" + msg);
                        toolStripMessageLabel.Text = msg;
                        Text = "SqlStudio";
                    }
                    else if (res.ResType == SqlResult.ResultType.INFO)
                    {
                        string msg = string.Format("{0}, took {1}", res.Message, ConvertMillisecondsToHumanReadableString(res.ExecutionTimeMS));
                        cmdLineControl.InsertCommandOutput("\n" + msg);
                        toolStripMessageLabel.Text = msg;
                    }
                    else if (res.ResType == SqlResult.ResultType.BACKGROUND_INFO)
                    {
                        toolStripMessageLabel.Text = string.Format("{0}, took {1}", res.Message, ConvertMillisecondsToHumanReadableString(res.ExecutionTimeMS));
                        toolStripStatusLabelMetaData.Text = "Cache";
                        return;
                    }
                    else
                    {
                        string msg = string.Format("Executed OK, affected {0} rows in {1}", res.RowsAffected, ConvertMillisecondsToHumanReadableString(res.ExecutionTimeMS));
                        cmdLineControl.InsertCommandOutput("\n" + msg);
                        toolStripMessageLabel.Text = msg;
                    }
                }
                else
                {
                    string msg = "Error: " + res.Message;
                    cmdLineControl.InsertCommandOutput("\n" + msg);

                    if (!string.IsNullOrEmpty(res.SqlQuery))
                        msg += " (" + res.SqlQuery + ")";
                    toolStripMessageLabel.Text = msg;
                }

                if (_bulkExecute)
                {
                    if (res.Success)
                    {
                        _bulkOk++;
                    }
                    else
                    {
                        _bulkErrors++;
                    }
                }
            }


            sqlOutput.DisplayResults(results);

            if (_bulkExecute && _cmdBuffer.Count < 1)
            {
                _bulkExecute = false;
                _bulkTimer.Stop();
                string msg = "";

                if (_bulkErrors > 0)
                {
                    msg = string.Format("Script executed with {0} ERRORS, {1} statements in {2}", _bulkErrors, _bulkErrors + _bulkOk,
                    ConvertMillisecondsToHumanReadableString(_bulkTimer.ElapsedMilliseconds));
                }
                else
                {
                    msg = string.Format("Script executed {0} statements in {1}", _bulkErrors + _bulkOk,
                    ConvertMillisecondsToHumanReadableString(_bulkTimer.ElapsedMilliseconds));
                }
                cmdLineControl.InsertCommandOutput(Environment.NewLine + "    ********************************" + Environment.NewLine + msg);
            }

            cmdLineControl.GetCommand();

            if (_cmdBuffer.Count > 0)
            {
                string cmd = _cmdBuffer[0];
                _cmdBuffer.RemoveAt(0);
                while (_executer.IsBussy)
                    System.Threading.Thread.Sleep(1);
                cmdLineControl.ExecuteCommand(cmd);
            }

            cmdLineControl.ClearUndoHistory();
        }

        private TabPage CreateNewScriptTab(string name)
        {
            TabPage tp = new TabPage(name);

            CommandPrompt.SQLScript sqlScript = new CommandPrompt.SQLScript();
            sqlScript.Dock = DockStyle.Fill;
            tp.Controls.Add((Control)sqlScript);

            return tp;
        }

        private void toolStripButtonNewScript_Click(object sender, EventArgs e)
        {
            TabPage tp = CreateNewScriptTab("sql script");
            tabControlMainDocs.TabPages.Add(tp);
        }

        private void toolStripButtonOpenScript_Click(object sender, EventArgs e)
        {
            openScriptToolStripMenuItem_Click(this, null);
        }

        private void openScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TabPage tp = CreateNewScriptTab(System.IO.Path.GetFileName(ofd.FileName));
                ((CommandPrompt.SQLScript)tp.Controls[0]).Open(ofd.FileName);
                tabControlMainDocs.TabPages.Add(tp);
            }
        }

        private void toolStripButtoSaveScript_Click(object sender, EventArgs e)
        {
            object obj = tabControlMainDocs.SelectedTab.Controls[0];
            if (obj is CommandPrompt.SQLScript)
            {
                ((CommandPrompt.SQLScript)obj).Save();
            }
        }

        private void toolStripButtonRunScript_Click(object sender, EventArgs e)
        {
            CommandPrompt.SQLScript script = tabControlMainDocs.SelectedTab.Controls[0] as CommandPrompt.SQLScript;
            if (script != null)
            {
                var text = script.GetSelectedText();
                var parser = new CommandParser();
                List<string> commands = parser.GetCommands(text);

                if (script.FileName != null)
                {
                    _executer.CurrentScriptPath = new FileInfo(script.FileName).DirectoryName + @"\";
                }

                tabControlMainDocs.SelectedTab = tabControlMainDocs.TabPages[0]; //set input window as selected
                for (int i = 1; i < commands.Count; i++)
                    _cmdBuffer.Add(commands[i]);
                if (commands.Count > 0)
                    cmdLineControl.ExecuteCommand(commands[0]);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlMainDocs.TabPages.Remove(tabControlMainDocs.SelectedTab);
        }

        private void saveScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButtoSaveScript_Click(this, null);
        }

        private void saveScriptAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            object obj = tabControlMainDocs.SelectedTab.Controls[0];
            if (obj is CommandPrompt.SQLScript)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ((CommandPrompt.SQLScript)obj).Save(sfd.FileName);
                    tabControlMainDocs.SelectedTab.Text = System.IO.Path.GetFileName(sfd.FileName);
                }
            }
        }

        private string ConvertMillisecondsToHumanReadableString(long ms)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(ms);
            if (t.Hours > 0)
            {
                return string.Format("{0:D2}h {1:D2}m {2:D2}s {3:D3}ms",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds,
                                    t.Milliseconds);
            }
            else if (t.Minutes > 0)
            {
                return string.Format("{0:D2}m {1:D2}s {2:D3}ms",
                                    t.Minutes,
                                    t.Seconds,
                                    t.Milliseconds);
            }
            else if (t.Seconds > 0)
            {
                return string.Format("{0:D2}s {1:D3}ms",
                                    t.Seconds,
                                    t.Milliseconds);
            }

            return string.Format("{0}ms",
                                    t.Milliseconds);
        }
    }
}
