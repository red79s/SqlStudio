using CfgDataStore;
using CommandPrompt;
using Common;
using Common.Model;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SqlStudio.Converters;
using SqlStudio.CvsImport;
using SqlStudio.EnumImporter;
using SqlStudio.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;
using static Org.BouncyCastle.Math.Primes;

namespace SqlStudio
{
    public partial class MainWindow : Form, ILogger
    {
        private ConfigDataStore _cfgDataStore = null;
        private string _userConfigDbFile;
        private IColumnValueDescriptionProvider _columnMetadataInfo = null;

        public MainWindow()
        {
            InitializeComponent();

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
            button.ToolTipText = $"{drConnection.db} - {drConnection.server}";
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
                    break;
                }
            }

            foreach (var item in toolStripMainWindow.Items)
            {
                if (item is ToolStripButton)
                {
                    var con = ((ToolStripButton)item).Tag as Connection;
                    if (con != null && con.p_key == key)
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
            var server = _cfgDataStore.GetConnection(key).server;
            var databaseConnection = tabControlDatabaseConnections.CreateNewDatabaseConnectionTab(server);
            databaseConnection.SetDislayFilterRow(DisplayFilterRow);
            databaseConnection.Connect(_cfgDataStore.GetConnection(key));
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
                conToolItem_OnConnectionClick(this, defaultConnection.p_key);
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //cmdLineControl.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //cmdLineControl.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //cmdLineControl.Paste();
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

        }

        private void openSQLiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var con = tabControlDatabaseConnections.CreateNewDatabaseConnectionTab("sqlite");
            con.SetDislayFilterRow(DisplayFilterRow);
            con.OpenSqlite();
        }


        private void openSqlCEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var con = tabControlDatabaseConnections.CreateNewDatabaseConnectionTab("sqlCET");
            con.SetDislayFilterRow(DisplayFilterRow);
            con.OpenSqlCet();
        }

        private void openConfigDbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var con = tabControlDatabaseConnections.CreateNewDatabaseConnectionTab("config");
            con.SetDislayFilterRow(DisplayFilterRow);
            con.OpenConfigDb();
        }

        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {
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

        }

        private void RunScriptFile(string fileName)
        {

        }

        void runScriptWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        void runScriptWorker_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        void scriptExecuter_Progress(object sender, int numRowsExecuted)
        {
        }


        void scriptExecuter_ExecuteFailed(object sender, string query, string message, int lineNumber)
        {

        }

        private void formatQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SqlFormatDialog formatDialog = new SqlFormatDialog();
            if (formatDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

            }
        }

        private void uploadSBDZipFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
                //ExecuteQuery(queryString, false, "");
            }
        }

        private void openCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var result = CsvImporter.ImportFromFile(ofd.FileName);
                //_executer_ExecutionFinished(this, new List<SqlResult> { result });
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
                //tabControlMainDocs.TabPages.Add(tp);
            }
        }

        public void Log(LogLevel logLevel, string message)
        {

        }

        public void Log(LogLevel logLevel, string message, Exception ex)
        {
            Console.WriteLine($"{logLevel}: {message}, {ex.Message}");
        }

        private void copyConnectionStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (_executer?.SqlExecuter?.ConnectionString == null)
            //{
            //    MessageBox.Show("Not able to get connection string", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            //Clipboard.SetText(_executer.SqlExecuter.ConnectionString);
        }

        private void generatePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var generatePasswordDialog = new GeneratePasswordForm();
            generatePasswordDialog.ShowDialog();
        }

        private void generateDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var gdf = new GenerateDataForm(_executer.SqlExecuter);
            //gdf.ShowDialog();
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
    }
}