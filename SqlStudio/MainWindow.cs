using CfgDataStore;
using SqlExecute;
using SqlStudio.Converters;
using SqlStudio.CvsImport;
using SqlStudio.ScriptExecuter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;

namespace SqlStudio
{
    public partial class MainWindow : Form, IExecuteQueryCallback
    {
        private Executer _executer = null;
        private ConfigDataStore _cfgDataStore = null;
        private SyntaxHighlight.SQLSyntaxHighlight _syntaxHighLight = null;
        private BuiltInCommands _builtIn = null;
        private List<string> _cmdBuffer = null;
        private Timer _executeTimer = new Timer();
        private Timer _autoSaveConfigTimer = new Timer();
        private string _userConfigDbFile;
        public MainWindow()
        {
            InitializeComponent();

            _cmdBuffer = new List<string>();
            _userConfigDbFile = Directory.GetParent(Application.UserAppDataPath) + @"\sqlstudio.cfg";
            string defaultCfgPath = Application.StartupPath + @"\sqlstudio.cfg";
            if (!File.Exists(_userConfigDbFile))
            {
                if (File.Exists(defaultCfgPath))
                    File.Copy(defaultCfgPath, _userConfigDbFile);
            }
            _cfgDataStore = new ConfigDataStore(_userConfigDbFile);
            
            _executer = new Executer(cmdLineControl, _cfgDataStore);
            _executer.ExecutionFinished += new Executer.ExecutionFinishedDelegate(_executer_ExecutionFinished);

            _syntaxHighLight = new SyntaxHighlight.SQLSyntaxHighlight();
            _syntaxHighLight.DefaultColor = cmdLineControl.ForeColor;
            _syntaxHighLight.IdentifiersColor = cmdLineControl.ForeColor;
            _syntaxHighLight.NumbersColor = Color.Green;
            _syntaxHighLight.StringsColor = Color.Red;
            _syntaxHighLight.KeyWordsColor = Color.Blue;

            cmdLineControl.CommandReady += new CommandPrompt.CmdLineControl.CommandReadyDelegate(cmdLineControl_CommandReady);
            cmdLineControl.CommandCompletion += new CommandPrompt.CmdLineControl.CommandCompletionDelegate(cmdLineControl_CommandCompletion);
            cmdLineControl.TextInserted += new FormatTextControl.FormatTextControl.TextInsertedDelegate(cmdLineControl_TextInserted);
            cmdLineControl.SetHistoryItems(_cfgDataStore.GetHistoryItems());
            cmdLineControl.HistoryItemsIsSaved();
            cmdLineControl.InsertSnipit += CmdLineControl_InsertSnipit;

            sqlOutput.SetConfig(_cfgDataStore);
            sqlOutput.SetExecuteCallback(this);
            sqlOutput.UpdatedResults += new SqlOutputTabContainer.UpdatedResultsDelegate(sqlOutput_UpdatedResults);
            sqlOutput.VisibleRowsChanged += (s, e) => { visibleRowsToolStripStatusLabel.Text = $"{e}"; };

            tabControlMainDocs.MouseDown += new MouseEventHandler(tabControlMainDocs_MouseDown);

            _builtIn = new BuiltInCommands(cmdLineControl, _cfgDataStore);

            _executeTimer.Interval = 1000;
            _executeTimer.Tick += ExecuteTimerOnTick;

            _autoSaveConfigTimer.Interval = 10000;
            _autoSaveConfigTimer.Tick += AutoSaveConfigTimer_Tick;
            _autoSaveConfigTimer.Start();

            InitMenues();
            cmdLineControl.GetCommand();
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

        List<string> cmdLineControl_CommandCompletion(object sender, string cmd, int index)
        {
            int lastCmdSep = CommandParser.GetLastCmdSeperator(cmd, ';', index);

            string realCmd = cmd;
            int realIndex = index;
            if (lastCmdSep > 0)
            {
                realCmd = cmd.Substring(lastCmdSep + 1, cmd.Length - (lastCmdSep + 1));
                realIndex = index - (lastCmdSep + 1);
            }

            if (realCmd[0] == ':')
            {
                realCmd = realCmd.Substring(1, realCmd.Length - 1);
                realIndex--;
            }

            return _executer.GetPosibleCompletions(realCmd, realIndex);
        }

        private void InitMenues()
        {
            displayFilterRowToolStripMenuItem.Checked = _cfgDataStore.GetStringValue("display_filter_row") == "True";
            var connections = _cfgDataStore.GetConnections();
            foreach (Connection dr in connections)
            {
                AddConnectionToMenu(dr);
                AddConnectionToToolsMenu(dr);
            }
        }

        private void AddConnectionToMenu(Connection drConnection)
        {
            ConnectToolStripItem conToolItem = new ConnectToolStripItem(drConnection.description, drConnection.p_key);
            conToolItem.OnConnectionClick += new ConnectToolStripItem.OnConnectionClickDelegate(conToolItem_OnConnectionClick);
            conToolItem.OnEditConnection += new ConnectToolStripItem.OnEditConnectionDelegate(conToolItem_OnEditConnection);
            conToolItem.OnDeleteConnection += new ConnectToolStripItem.OnDeleteConnectionDelegate(conToolItem_OnDeleteConnection);
            connectToolStripMenuItem.DropDownItems.Insert(connectToolStripMenuItem.DropDownItems.Count - 2, conToolItem);
        }

        private void AddConnectionToToolsMenu(Connection drConnection)
        {
            var button = new ToolStripButton(drConnection.description);
            button.Tag = drConnection;
            button.Margin = new Padding(4, 1, 0, 2);
            button.Click += (sender, args) => conToolItem_OnConnectionClick(this, drConnection.p_key);
            toolStripMainWindow.Items.Add(button);
        }

        void conToolItem_OnDeleteConnection(object sender, long key)
        {
            _cfgDataStore.RemoveConnection(key);
            for (int i = 0; i < connectToolStripMenuItem.DropDownItems.Count; i++)
            {
                if (connectToolStripMenuItem.DropDownItems[i] is ConnectToolStripItem &&
                    ((ConnectToolStripItem)connectToolStripMenuItem.DropDownItems[i]).Key == key)
                {
                    connectToolStripMenuItem.DropDownItems.RemoveAt(i);
                    return;
                }
            }
        }

        private ToolStripButton GetToolStripButton(Connection con)
        {
            foreach (var item in toolStripMainWindow.Items)
            {
                if (!(item is ToolStripButton))
                    continue;

                var button = (ToolStripButton)item;
                if (button.Tag == con)
                    return button;
            }
            return null;
        }

        void conToolItem_OnEditConnection(object sender, long key)
        {
            Connection con = _cfgDataStore.GetConnection(key);
            var button = GetToolStripButton(con);
            
            NewDBConnectionForm dbConForm = new NewDBConnectionForm();
            dbConForm.ConnectionRow = con;
            if (dbConForm.ShowDialog() == DialogResult.OK)
            {
                if (con.default_connection)
                    _cfgDataStore.SetDefaultConnection(con.p_key);
                _cfgDataStore.Save();

                var toolStripItem = sender as ConnectToolStripItem;
                if (toolStripItem != null)
                {
                    toolStripItem.Text = con.description;
                }

                if (button != null)
                {
                    button.Text = con.description;
                }
            }
        }

        void conToolItem_OnConnectionClick(object sender, long key)
        {
            if (_executer.IsBussy)
            {
                MessageBox.Show("Bussy executing, please wait");
                return;
            }

            cmdLineControl.ExecuteCommand(_cfgDataStore.GetConnectCommand(key));
        }

        private void AutoSaveConfigTimer_Tick(object sender, EventArgs e)
        {
            _autoSaveConfigTimer.Stop();
            SaveHistoryItems();
            _autoSaveConfigTimer.Start();
        }

        private void SaveHistoryItems()
        {
           if (!cmdLineControl.HaveUnsavedHistoryItems)
            {
                return;
            }

            _cfgDataStore.SetHistoryItems(cmdLineControl.GetHistoryItems());
            _cfgDataStore.Save();
            cmdLineControl.HistoryItemsIsSaved();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _autoSaveConfigTimer.Stop();

            SaveHistoryItems();

            _cfgDataStore.SetValue("window_size_x", (long)Width);
            _cfgDataStore.SetValue("window_size_y", (long)Height);
            if (Location.X < -10000 || Location.Y < -10000)
            {
                MessageBox.Show($"Location is wrong: {Location}");
            }
            else
            {
                _cfgDataStore.SetValue("window_location_x", (long)Location.X);
                _cfgDataStore.SetValue("window_location_y", (long)Location.Y);
            }
            _cfgDataStore.Save();

            base.OnFormClosing(e);
        }

        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    _autoSaveConfigTimer.Stop();

        //    base.OnClosing(e);

        //    SaveHistoryItems();

        //    _cfgDataStore.SetValue("window_size_x", (long)Width);
        //    _cfgDataStore.SetValue("window_size_y", (long)Height);
        //    if (Location.X < -10000 || Location.Y < -10000)
        //    {
        //        MessageBox.Show($"Location is wrong: {Location}");
        //    }
        //    else
        //    {
        //        _cfgDataStore.SetValue("window_location_x", (long)Location.X);
        //        _cfgDataStore.SetValue("window_location_y", (long)Location.Y);
        //    }
        //    _cfgDataStore.Save();
        //}

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            int width = (int)_cfgDataStore.GetLongValue("window_size_x");
            int height = (int)_cfgDataStore.GetLongValue("window_size_y");
            Width = width > 500 ? width : 500;
            Height = height > 400 ? height : 400;

            var locationX = (int)_cfgDataStore.GetLongValue("window_location_x");
            var locationY = (int)_cfgDataStore.GetLongValue("window_location_y");
            if (locationX > -32000 && locationY > -32000)
            {
                Location = new Point(locationX, locationY);
            }

            Connection defaultConnection = _cfgDataStore.GetDefaultConnection();
            if (defaultConnection != null)
                conToolItem_OnConnectionClick(this, defaultConnection.p_key);
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
                        SetDatabasesOnToolsMenu(databases);
                    }
                    else if (res.ResType == SqlResult.ResultType.DISCONNECT)
                    {
                        string msg = string.Format("Disconnected OK, took {0}", ConvertMillisecondsToHumanReadableString(res.ExecutionTimeMS));
                        cmdLineControl.InsertCommandOutput("\n" + msg);
                        toolStripMessageLabel.Text = msg;
                        Text = "SqlStudio";

                        SetDatabasesOnToolsMenu(new List<string>());
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
        }

        private void SetDatabasesOnToolsMenu(List<string> databases)
        {
            toolStripDatabaseConnectionsDropDownButton.DropDownItems.Clear();
            foreach (string database in databases)
            {
                var button = new ToolStripButton(database);
                button.Tag = database;
                button.Margin = new Padding(4, 1, 0, 2);
                button.Click += (sender, args) => ConnectToDatabaseOnSameServer(database);
                toolStripDatabaseConnectionsDropDownButton.DropDownItems.Add(button);
            }
        }

        private void ConnectToDatabaseOnSameServer(string databaseName)
        {
            if (_executer.CurrentConnection == null)
            {
                MessageBox.Show("Not connected to server", "Error");
                return;
            }
            
            var connectionCommand = _cfgDataStore.GetConnectCommand(_executer.CurrentConnection.ProviderName, 
                _executer.CurrentConnection.Server, 
                databaseName, 
                _executer.CurrentConnection.User, 
                _executer.CurrentConnection.Password);
            cmdLineControl.ExecuteCommand(connectionCommand);

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

        public void ExecuteQuery(string query, bool inNewTab, string datatabLabel)
        {
            if (inNewTab)
            {
                sqlOutput.CreateNewDataTab(datatabLabel);
            }

            cmdLineControl.InsertCommandOutput(query);

            cmdLineControl_CommandReady(this, query);
        }

        private bool _bulkExecute = false;
        private Stopwatch _bulkTimer = new Stopwatch();
        private int _bulkErrors = 0;
        private int _bulkOk = 0;
        private DateTime _excutionStarted;

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

        private void ExecuteTimerOnTick(object sender, EventArgs e)
        {
            toolStripMessageLabel.Text = $"Executing: {DateTime.Now.Subtract(_excutionStarted).ToString(@"hh\:mm\:ss")}";
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewDBConnectionForm newConnection = new NewDBConnectionForm();
            Connection con = _cfgDataStore.CreateNewConnection(SqlExecuter.DatabaseProvider.SQLSERVER);
            newConnection.ConnectionRow = con;
            if (newConnection.ShowDialog() == DialogResult.OK)
            {
                _cfgDataStore.AddConnection(con);
                AddConnectionToMenu(con);
                _cfgDataStore.Save();
            }
        }

        private void scriptsEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void aliasesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aliasEditor = new AliasesEditor.AliasesEditor();
            var aliases = _cfgDataStore.GetAliases();
            foreach (var alias in aliases)
                aliasEditor.AddAlias(new AliasesEditor.Alias(alias.alias_name, alias.alias_value));

            aliasEditor.StartPosition = FormStartPosition.CenterParent;
            if (aliasEditor.ShowDialog() == DialogResult.OK)
            {
                List<AliasesEditor.Alias> deleted = aliasEditor.GetDeleted();
                foreach (AliasesEditor.Alias alias in deleted)
                    _cfgDataStore.RemoveAlias(alias.name);
                List<AliasesEditor.Alias> modified = aliasEditor.GetModified();
                foreach (AliasesEditor.Alias alias in modified)
                    _cfgDataStore.ModifyAlias(alias.name, alias.value);
                List<AliasesEditor.Alias> added = aliasEditor.GetNew();
                foreach (AliasesEditor.Alias alias in added)
                    _cfgDataStore.AddAlias(alias.name, alias.value);

                _cfgDataStore.Save();
            }
        }

        private void autoQueriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var editor = new AutoQueryEditor.AutoQueryEditor();
            editor.Init(_cfgDataStore);
            editor.StartPosition = FormStartPosition.CenterParent;
            editor.ShowDialog();
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            HelpForm hf = new HelpForm();
            hf.StartPosition = FormStartPosition.CenterParent;
            hf.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var msg = $"SqlStudio, created by Bard Hustveit{Environment.NewLine}ConfigFile: {_userConfigDbFile}{Environment.NewLine}" +
                $"Version: {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
            MessageBox.Show(msg, "SqlStudio - About");
        }

        private void closeConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _executer.Execute(new string[] { "disconnect" });
        }

        private TabPage CreateNewScriptTab(string name)
        {
            TabPage tp = new TabPage(name);

            CommandPrompt.SQLScript sqlScript = new CommandPrompt.SQLScript();
            sqlScript.Dock = DockStyle.Fill;
            tp.Controls.Add((Control)sqlScript);

            return tp;
        }

        private void newScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage tp = CreateNewScriptTab("SQL script");
            tabControlMainDocs.TabPages.Add(tp);
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

        private void toolStripButtonNewScript_Click(object sender, EventArgs e)
        {
            TabPage tp = CreateNewScriptTab("sql script");
            tabControlMainDocs.TabPages.Add(tp);
        }

        private void toolStripButtonOpenScript_Click(object sender, EventArgs e)
        {
            openScriptToolStripMenuItem_Click(this, null);
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

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlMainDocs.TabPages.Remove(tabControlMainDocs.SelectedTab);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmdLineControl.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmdLineControl.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmdLineControl.Paste();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewDBConnectionForm ndbCon = new NewDBConnectionForm();
            var connectRow = _cfgDataStore.CreateNewConnection(SqlExecuter.DatabaseProvider.SQLSERVER);
            ndbCon.ConnectionRow = connectRow;

            if (ndbCon.ShowDialog() == DialogResult.OK)
            {
                cmdLineControl.ExecuteCommand(
                    _cfgDataStore.GetConnectCommand(ndbCon.ConnectionRow.provider,
                                                         ndbCon.ConnectionRow.server,
                                                         ndbCon.ConnectionRow.db,
                                                         ndbCon.ConnectionRow.user,
                                                         ndbCon.ConnectionRow.password));
            }
        }

        private void openSQLiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                cmdLineControl.ExecuteCommand(_cfgDataStore.GetConnectCommand("SQLite", null, ofd.FileName, null, null));
            }
        }


        private void openSqlCEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                cmdLineControl.ExecuteCommand(_cfgDataStore.GetConnectCommand(SqlExecuter.GetProviderName(SqlExecuter.DatabaseProvider.SQLSERVERCE), null, ofd.FileName, null, null));
            }
        }

        private void openConfigDbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmdLineControl.ExecuteCommand(_cfgDataStore.GetConnectCommand("SQLite", null, _userConfigDbFile, null, null));
        }

        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            sqlOutput.CreateNewDataTab(null);
        }

        private void displayFilterRowToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmiDisplayFilterRow = (ToolStripMenuItem)sender;
            _cfgDataStore.SetValue("display_filter_row", tsmiDisplayFilterRow.Checked.ToString());
            sqlOutput.DisplayFilterRow = tsmiDisplayFilterRow.Checked;
        }

        private void runScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                RunScriptFile(ofd.FileName);  
            }
        }

        private void RunScriptFile(string fileName)
        {
            BackgroundWorker runScriptWorker = new BackgroundWorker();
            runScriptWorker.DoWork += runScriptWorker_DoWork;
            
            

            runScriptWorker.RunWorkerAsync(fileName);
        }

        void runScriptWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        void runScriptWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string fileName = (string)e.Argument;

            Invoke((MethodInvoker)(() => 
                {
                    _executer.LockExecuter();
                    cmdLineControl.InsertCommandOutput(string.Format("Executing script: {0}", fileName) + Environment.NewLine); 
                }));

            var scriptExecuter = ScriptExecuterFactory.GetExecuter(fileName);
            scriptExecuter.ExecuteFailed += scriptExecuter_ExecuteFailed;
            scriptExecuter.Progress += scriptExecuter_Progress;
            scriptExecuter.Execute(_executer.SqlExecuter, fileName);

            Invoke((MethodInvoker)(() =>
                {
                    cmdLineControl.InsertCommandOutput(string.Format("Script execution finished in {0}, statements: {1}, errors: {2}",
                        scriptExecuter.ElapsedTime.ToString(), scriptExecuter.NumStatementsExecuted, scriptExecuter.NumErrors));
                    _executer.UnlockExecuter();

                    cmdLineControl.GetCommand();
                }));
        }

        void scriptExecuter_Progress(object sender, int numRowsExecuted)
        {
            Invoke((MethodInvoker) (() => toolStripMessageLabel.Text = string.Format("Rows executed: {0}", numRowsExecuted)));
        }


        void scriptExecuter_ExecuteFailed(object sender, string query, string message, int lineNumber)
        {
            Invoke((MethodInvoker)(() =>
                {
                    cmdLineControl.InsertCommandOutput($"Error executing: Line {lineNumber}:{query}{Environment.NewLine}  {message}{Environment.NewLine}");
                }));
        }

        private void formatQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SqlFormatDialog formatDialog = new SqlFormatDialog();
            if (formatDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                cmdLineControl.InsertTextAtCaret(formatDialog.FormatedSql);
            }
        }

        private void uploadSBDZipFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool executionIsLocked = false;
            try
            {
                _executer.LockExecuter();
                executionIsLocked = true;

                var con = _executer.Connection as SqlConnection;
                if (con == null)
                    throw new Exception("Only works with a valid SqlServer connection");

                FileStream file = null;
                string version = null;
                DateTime created;
                long fileLength;
                var opf = new OpenFileDialog();
                if (opf.ShowDialog() == DialogResult.OK)
                {
                    version = GetSbdVersionArchive(opf.FileName);
                    if (version == null)
                    {
                        throw new Exception("Not able to get version from SBD executable");
                    }
                    file = File.OpenRead(opf.FileName);
                    created = File.GetCreationTime(opf.FileName);
                    fileLength = new FileInfo(opf.FileName).Length;
                }
                else
                {
                    return;
                }

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = string.Format("DELETE FROM SbdClientSoftware WHERE version like '{0}.%'",
                        new Version(version).Major);
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = con.CreateCommand())
                {
                    var description = "new version: " + version;

                    cmd.CommandText =
                        "INSERT INTO SbdClientSoftware (Version, Description, SbdZipfile, Uploaded, Created, SbdZipFileLength) VALUES(@version, @description, @sbdZipfile, @uploaded, @created, @sbdZipFileLength)";
                    cmd.Parameters.AddWithValue("@version", version);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.Add("@sbdZipfile", SqlDbType.Binary, -1).Value = file;
                    cmd.Parameters.AddWithValue("@uploaded", DateTime.Now);
                    cmd.Parameters.AddWithValue("@created", created);
                    cmd.Parameters.AddWithValue("@sbdZipFileLength", fileLength);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Successfully uploaded new SBD files with version: " + version);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to upload Sbd zip file: " + ex.Message);
            }
            finally
            {
                if (executionIsLocked)
                    _executer.UnlockExecuter();
            }
        }

        private string GetSbdVersionArchive(string fileName)
        {
            using (ZipArchive archive = ZipFile.OpenRead(fileName))
            {
                var entry = archive.Entries.FirstOrDefault(x => x.FullName.EndsWith("DSG.SBD.Client.exe"));
                if (entry != null)
                {
                    var extractedFile = Path.GetTempPath() + Guid.NewGuid() + ".exe";
                    entry.ExtractToFile(extractedFile);
                    var version = FileVersionInfo.GetVersionInfo(extractedFile).ProductVersion;
                    File.Delete(extractedFile);
                    return version;
                }
            }

            throw new Exception("Unable to get version info from downloaded archive");
        }

        private void cancelExecutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _executer?.Cancel();
        }

        private LogSearchParametersDialog _logSearchParametersDialog = null;
        private void logSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_logSearchParametersDialog == null)
            {
                _logSearchParametersDialog = new LogSearchParametersDialog();
                _logSearchParametersDialog.StartPosition = FormStartPosition.CenterParent;
            }
            
            if (_logSearchParametersDialog.ShowDialog() == DialogResult.OK)
            {
                var queryString = _logSearchParametersDialog.QueryString;
                ExecuteQuery(queryString, false, "");
            }
        }

        private void openCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var result = CsvImporter.ImportFromFile(ofd.FileName);
                _executer_ExecutionFinished(this, new List<SqlResult> { result });
            }
        }

        private void logImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var result = CsvImporter.ImportDataFromFile(ofd.FileName);
                var converter = new ConvertCvsDataToLogRecords(result);
                var rows = converter.CreateInsertCommands();
                TabPage tp = CreateNewScriptTab(System.IO.Path.GetFileName(ofd.FileName));
                ((CommandPrompt.SQLScript)tp.Controls[0]).Open(rows);
                tabControlMainDocs.TabPages.Add(tp);
            }
        }
    }
}