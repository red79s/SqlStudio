using CfgDataStore;
using Common;
using Common.Model;
using SqlStudio.ColumnMetaDataInfo;
using SqlStudio.EnumImporter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SqlStudio
{
    public partial class MainWindow : Form, ILogger
    {
        private IConfigDataStore _cfgDataStore = null;
        private string _userConfigDbFile;
        private IColumnValueDescriptionProvider _columnMetadataInfo = null;

        public MainWindow()
        {
            InitializeComponent();

            _columnMetadataInfo = new ColumnValueDescriptionManager();
            _columnMetadataInfo.Load();

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
            tabControlDatabaseConnections.ConfigDataStore = _cfgDataStore;

            InitMenues();
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

        private void AddConnectionToMenu(Connection connection)
        {
            ConnectToolStripItem conToolItem = new ConnectToolStripItem(connection);
            conToolItem.OnConnectionClick += new ConnectToolStripItem.OnConnectionClickDelegate(conToolItem_OnConnectionClick);
            conToolItem.OnEditConnection += new ConnectToolStripItem.OnEditConnectionDelegate(conToolItem_OnEditConnection);
            conToolItem.OnDeleteConnection += new ConnectToolStripItem.OnDeleteConnectionDelegate(conToolItem_OnDeleteConnection);
            connectToolStripMenuItem.DropDownItems.Insert(connectToolStripMenuItem.DropDownItems.Count - 2, conToolItem);
        }

        private void AddConnectionToToolsMenu(Connection connection)
        {
            var button = new ToolStripButton(connection.description);
            button.Tag = connection;
            button.ToolTipText = $"{connection.db} - {connection.server}";
            button.Margin = new Padding(4, 1, 0, 2);
            button.Click += (sender, args) => conToolItem_OnConnectionClick(this, connection);
            toolStripMainWindow.Items.Add(button);
        }

        void conToolItem_OnDeleteConnection(object sender, Connection connection)
        {
            _cfgDataStore.RemoveConnection(connection);
            for (int i = 0; i < connectToolStripMenuItem.DropDownItems.Count; i++)
            {
                if (connectToolStripMenuItem.DropDownItems[i] is ConnectToolStripItem &&
                    ((ConnectToolStripItem)connectToolStripMenuItem.DropDownItems[i]).Connection == connection)
                {
                    connectToolStripMenuItem.DropDownItems.RemoveAt(i);
                    break;
                }
            }

            foreach (var item in toolStripMainWindow.Items)
            {
                if (item is ToolStripButton)
                {
                    var con = ((ToolStripButton)item).Tag as Connection;
                    if (con != null && con == connection)
                    {
                        toolStripMainWindow.Items.Remove((ToolStripItem)item);
                        break;
                    }
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

        void conToolItem_OnEditConnection(object sender, Connection connection)
        {
            var button = GetToolStripButton(connection);

            NewDBConnectionForm dbConForm = new NewDBConnectionForm();
            dbConForm.ConnectionRow = connection;
            if (dbConForm.ShowDialog() == DialogResult.OK)
            {
                if (connection.default_connection)
                    _cfgDataStore.SetDefaultConnection(connection);
                _cfgDataStore.Save();

                var toolStripItem = sender as ConnectToolStripItem;
                if (toolStripItem != null)
                {
                    toolStripItem.Text = connection.description;
                }

                if (button != null)
                {
                    button.Text = connection.description;
                }
            }
        }

        void conToolItem_OnConnectionClick(object sender, Connection connection)
        {
            var databaseConnection = tabControlDatabaseConnections.CreateNewDatabaseConnectionTab(connection.server, _columnMetadataInfo);
            databaseConnection.SetDislayFilterRow(DisplayFilterRow);
            databaseConnection.Connect(connection);
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
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

            e.Cancel = false;

            base.OnFormClosing(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (_cfgDataStore == null)
            {
                Application.Exit();
                return;
            }

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
                conToolItem_OnConnectionClick(this, defaultConnection);
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl?.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl?.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl?.Paste();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewDBConnectionForm newConnection = new NewDBConnectionForm();
            Connection con = _cfgDataStore.CreateNewConnection("");
            newConnection.ConnectionRow = con;
            if (newConnection.ShowDialog() == DialogResult.OK)
            {
                _cfgDataStore.AddConnection(con);
                AddConnectionToMenu(con);
                AddConnectionToToolsMenu(con);
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
                    _cfgDataStore.RemoveAlias(alias.Name);
                List<AliasesEditor.Alias> modified = aliasEditor.GetModified();
                foreach (AliasesEditor.Alias alias in modified)
                    _cfgDataStore.ModifyAlias(alias.Name, alias.Value);
                List<AliasesEditor.Alias> added = aliasEditor.GetNew();
                foreach (AliasesEditor.Alias alias in added)
                    _cfgDataStore.AddAlias(alias.Name, alias.Value);

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
            tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl?.CloseConnection();
        }

        private TabPage CreateNewScriptTab(string name)
        {
            var tp = new TabPage(name);

            CommandPrompt.SQLScript sqlScript = new CommandPrompt.SQLScript();
            sqlScript.Dock = DockStyle.Fill;
            tp.Controls.Add(sqlScript);

            return tp;
        }

        private void newScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl?.CreateNewScriptTab();
        }

        private void openScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl?.OpenScriptFile();
        }

        private void toolStripButtonNewScript_Click(object sender, EventArgs e)
        {
            newScriptToolStripMenuItem_Click(this, e);
        }

        private void toolStripButtonOpenScript_Click(object sender, EventArgs e)
        {
            openScriptToolStripMenuItem_Click(this, null);
        }

        private void toolStripButtoSaveScript_Click(object sender, EventArgs e)
        {
            tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl.SaveScript();
        }

        private void toolStripButtonRunScript_Click(object sender, EventArgs e)
        {
            tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl?.RunScript();
        }

        private void saveScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButtoSaveScript_Click(this, null);
        }

        private void saveScriptAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl?.SaveScriptAs();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl?.CloseScriptTab();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openScriptToolStripMenuItem_Click(sender, e);
        }

        private void openSQLiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var con = tabControlDatabaseConnections.CreateNewDatabaseConnectionTab("sqlite", _columnMetadataInfo);
            con.SetDislayFilterRow(DisplayFilterRow);
            con.OpenSqlite();
        }


        private void openSqlCEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var con = tabControlDatabaseConnections.CreateNewDatabaseConnectionTab("sqlCET", _columnMetadataInfo);
            con.SetDislayFilterRow(DisplayFilterRow);
            con.OpenSqlCet();
        }

        private void openConfigDbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var con = tabControlDatabaseConnections.CreateNewDatabaseConnectionTab("config", _columnMetadataInfo);
            con.SetDislayFilterRow(DisplayFilterRow);
            con.OpenConfigDb();
        }

        private bool DisplayFilterRow
        {
            get
            {
                return displayFilterRowToolStripMenuItem.Checked;
            }
        }

        private void displayFilterRowToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmiDisplayFilterRow = (ToolStripMenuItem)sender;
            _cfgDataStore.SetValue("display_filter_row", tsmiDisplayFilterRow.Checked.ToString());
            foreach (var dbConCntr in tabControlDatabaseConnections.DatabaseConnectionUIControls)
            {
                dbConCntr.SetDislayFilterRow(tsmiDisplayFilterRow.Checked);
            }
        }

        private void runScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl?.RunScript();
        }

        private void formatQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SqlFormatDialog formatDialog = new SqlFormatDialog();
            formatDialog.ShowDialog();
        }


        private void cancelExecutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl?.CancelExecution();
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
                tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl?.ExecuteQueryAndDisplay(queryString, false, "");
            }
        }

        private void openCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl?.OpenCvsFile();
        }

        private void NewDataTabToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl.InsertNewDataTab();
        }

        public void Log(LogLevel logLevel, string message)
        {
            Console.WriteLine($"{logLevel}: {message}");
        }

        public void Log(LogLevel logLevel, string message, Exception ex)
        {
            Console.WriteLine($"{logLevel}: {message}, {ex.Message}");
        }

        private void copyConnectionStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl?.CopyConnectionStringToClipboard();
        }

        private void generatePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var generatePasswordDialog = new GeneratePasswordForm();
            if (generatePasswordDialog.ShowDialog() == DialogResult.OK)
            {
                tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl?.WriteToOutput($"Password: {generatePasswordDialog.PasswordClearText}");
                tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl?.WriteToOutput($"Password hash: {generatePasswordDialog.PasswordHash}");
                Clipboard.SetText(generatePasswordDialog.PasswordHash);
            }
        }

        private void generateDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl?.OpenGenerateDataTool();
        }

        private void importEnumValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            var importer = new EfEnumImporter();
            var colDesc = importer.Import(ofd.FileName);

            if (colDesc != null && colDesc.Count > 0)
            {
                var source = colDesc[0].AssemblyName;
                var cvd = new List<ColumnValueDescription>();
                foreach (var cd in colDesc)
                {
                    foreach (var enumVal in cd.EnumValues)
                    {
                        cvd.Add(
                            new ColumnValueDescription
                            {
                                Source = source,
                                TableName = cd.TableName,
                                ColumnName = cd.ColumnName,
                                Value = enumVal.Value.ToString(),
                                Description = enumVal.Name
                            });
                    }
                }
                _columnMetadataInfo.AddColumnMetadataInfo(source, cvd);
                _columnMetadataInfo.Save();
            }
        }

        private void formatTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var textOutputDialog = new TextOutputDialog("");
            textOutputDialog.Owner = this;
            textOutputDialog.StartPosition = FormStartPosition.CenterParent;
            textOutputDialog.ShowDialog();
        }

        private void setUserPermissionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlDatabaseConnections.SelectedDatabaseConnectionUIControl?.OpenSetUserPermissionsTool();
        }
    }
}