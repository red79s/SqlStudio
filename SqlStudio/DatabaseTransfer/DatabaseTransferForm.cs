using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CfgDataStore;

namespace SqlStudio.DatabaseTransfer
{
    /// <summary>
    /// Manage preconfigured prod -> test database pairs and start a transfer.
    /// The source list only offers connections flagged as production and the target list only
    /// non-production ones; TransferGuard re-validates on save and again at run time.
    /// </summary>
    public class DatabaseTransferForm : Form
    {
        private readonly IConfigDataStore _cfg;
        private TransferSettings _settings;
        private List<Connection> _connections;
        private bool _loading;
        private bool _dirty;

        private ListBox _pairList;
        private Button _addButton, _duplicateButton, _deleteButton;
        private TextBox _nameText;
        private ComboBox _sourceCombo, _targetCombo;
        private EndpointEditor _sourceSsh, _targetSsh;
        private TextBox _scriptText;
        private TextBox _sourceDbText, _targetDbText;
        private TextBox _relinkText;
        private CheckBox _relinkOwnerCheck;
        private Label _validationLabel;
        private TextBox _localFolderText;
        private Button _saveButton, _runButton, _testRunButton, _closeButton;
        private Panel _editorPanel;

        private class ConnectionItem
        {
            public Connection Connection { get; }
            public ConnectionItem(Connection c) { Connection = c; }
            public override string ToString() => $"{Connection.description}   ({Connection.server} / {Connection.db})";
        }

        public DatabaseTransferForm(IConfigDataStore cfg)
        {
            _cfg = cfg ?? throw new ArgumentNullException(nameof(cfg));
            BuildUi();
            LoadAll();
        }

        #region UI construction

        private void BuildUi()
        {
            Text = "Database Transfer (production → test)";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(900, 640);
            Size = new Size(1000, 700);
            Font = new Font("Segoe UI", 9F);
            ShowInTaskbar = false;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Padding = new Padding(8) };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Controls.Add(root);

            // Left: pair list
            var left = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
            left.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            left.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            left.Controls.Add(new Label { Text = "Transfers", AutoSize = true, Font = new Font(Font, FontStyle.Bold), Margin = new Padding(3, 3, 3, 6) }, 0, 0);
            _pairList = new ListBox { Dock = DockStyle.Fill, IntegralHeight = false };
            _pairList.SelectedIndexChanged += (s, e) => { if (!_loading) ShowSelectedPair(); };
            _pairList.DoubleClick += (s, e) => RunSelected(testRun: false);
            left.Controls.Add(_pairList, 0, 1);
            var leftButtons = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, WrapContents = false };
            _addButton = new Button { Text = "Add", AutoSize = true };
            _duplicateButton = new Button { Text = "Duplicate", AutoSize = true };
            _deleteButton = new Button { Text = "Delete", AutoSize = true };
            _addButton.Click += (s, e) => AddPair(null);
            _duplicateButton.Click += (s, e) => { if (SelectedPair != null) AddPair(SelectedPair); };
            _deleteButton.Click += (s, e) => DeleteSelected();
            leftButtons.Controls.AddRange(new Control[] { _addButton, _duplicateButton, _deleteButton });
            left.Controls.Add(leftButtons, 0, 2);
            root.Controls.Add(left, 0, 0);

            // Right: editor
            _editorPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            var grid = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 3, Padding = new Padding(8, 0, 0, 0) };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            int row = 0;
            void AddRow(string label, Control c, Control extra = null)
            {
                grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                grid.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 7, 8, 3) }, 0, row);
                c.Dock = DockStyle.Fill;
                grid.Controls.Add(c, 1, row);
                if (extra != null)
                    grid.Controls.Add(extra, 2, row);
                else
                    grid.SetColumnSpan(c, 2);
                row++;
            }
            void AddHeader(string text, Color color)
            {
                grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                var l = new Label { Text = text, AutoSize = true, Font = new Font(Font, FontStyle.Bold), ForeColor = color, Margin = new Padding(3, 14, 3, 2) };
                grid.Controls.Add(l, 0, row);
                grid.SetColumnSpan(l, 3);
                row++;
            }

            _nameText = new TextBox();
            _nameText.TextChanged += (s, e) => { if (UpdateFromEditor()) RefreshListText(); };
            AddRow("Name:", _nameText);

            AddHeader("Source — PRODUCTION (backup is taken here)", Color.DarkRed);
            _sourceCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            _sourceCombo.SelectedIndexChanged += (s, e) => { UpdateFromEditor(); RefreshHostHints(); };
            AddRow("SQL connection:", _sourceCombo);
            _sourceDbText = new TextBox();
            _sourceDbText.TextChanged += (s, e) => UpdateFromEditor();
            AddRow("Database:", _sourceDbText, CreateDatabasePickerButton(_sourceCombo, _sourceDbText));
            _sourceSsh = new EndpointEditor();
            _sourceSsh.Changed += (s, e) => UpdateFromEditor();
            AddRow("ssh / folder:", _sourceSsh);

            AddHeader("Target — TEST (database is overwritten here)", Color.DarkGreen);
            _targetCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            _targetCombo.SelectedIndexChanged += (s, e) => { UpdateFromEditor(); RefreshHostHints(); };
            AddRow("SQL connection:", _targetCombo);
            _targetDbText = new TextBox();
            _targetDbText.TextChanged += (s, e) => UpdateFromEditor();
            AddRow("Database:", _targetDbText, CreateDatabasePickerButton(_targetCombo, _targetDbText));
            _targetSsh = new EndpointEditor();
            _targetSsh.Changed += (s, e) => UpdateFromEditor();
            AddRow("ssh / folder:", _targetSsh);

            AddHeader("After restore", SystemColors.ControlText);
            _scriptText = new TextBox();
            _scriptText.TextChanged += (s, e) => UpdateFromEditor();
            var scriptButtons = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = Padding.Empty };
            var browseScript = new Button { Text = "Browse...", AutoSize = true };
            browseScript.Click += (s, e) => BrowseScript();
            var clearScript = new Button { Text = "Clear", AutoSize = true };
            clearScript.Click += (s, e) => _scriptText.Text = "";
            scriptButtons.Controls.AddRange(new Control[] { browseScript, clearScript });
            AddRow("Optional SQL script:", _scriptText, scriptButtons);

            _relinkText = new TextBox { PlaceholderText = @"e.g. apx, reporting=DOMAIN\svc_report" };
            _relinkText.TextChanged += (s, e) => UpdateFromEditor();
            _relinkOwnerCheck = new CheckBox { Text = "make db_owner", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(6, 5, 3, 3) };
            _relinkOwnerCheck.CheckedChanged += (s, e) => UpdateFromEditor();
            new ToolTip().SetToolTip(_relinkOwnerCheck, "Add the users to db_owner, creating a user for the login if the database has none.");
            AddRow("Re-link users:", _relinkText, _relinkOwnerCheck);

            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var help = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(640, 0),
                ForeColor = SystemColors.GrayText,
                Margin = new Padding(3, 10, 3, 3),
                Text = "Source must be a connection marked 'Is production DB'; target must not be, and must not be on a server that hosts any production connection. " +
                       "ssh host defaults to the machine part of the SQL server name. Folders are local paths on each server (e.g. D:\\Backups) that the SQL Server service can read/write. " +
                       "Re-link users runs right after the restore (before the script) and links users that came from prod to this server's logins " +
                "(users are matched to logins by SID, which differs between servers): list user names, or user=login when the names differ. " +
                "The script runs against the restored test database and stops at the first failing batch."
            };
            grid.Controls.Add(help, 0, row);
            grid.SetColumnSpan(help, 3);
            row++;

            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _validationLabel = new Label { AutoSize = true, MaximumSize = new Size(640, 0), ForeColor = Color.Firebrick, Margin = new Padding(3, 10, 3, 3) };
            grid.Controls.Add(_validationLabel, 0, row);
            grid.SetColumnSpan(_validationLabel, 3);

            _editorPanel.Controls.Add(grid);
            root.Controls.Add(_editorPanel, 1, 0);

            // Bottom bar
            var bottom = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 4, Margin = new Padding(0, 8, 0, 0) };
            bottom.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            bottom.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            bottom.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            bottom.Controls.Add(new Label { Text = "Local staging folder:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 7, 8, 3) }, 0, 0);
            _localFolderText = new TextBox { Dock = DockStyle.Fill };
            _localFolderText.TextChanged += (s, e) => { if (!_loading) { _settings.LocalFolder = _localFolderText.Text; _dirty = true; } };
            bottom.Controls.Add(_localFolderText, 1, 0);
            var browseLocal = new Button { Text = "Browse...", AutoSize = true };
            browseLocal.Click += (s, e) => BrowseLocalFolder();
            bottom.Controls.Add(browseLocal, 2, 0);

            var buttons = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = new Padding(16, 0, 0, 0) };
            _runButton = new Button { Text = "Run transfer...", AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
            _runButton.Click += (s, e) => RunSelected(testRun: false);
            _testRunButton = new Button { Text = "Test run...", AutoSize = true };
            _testRunButton.Click += (s, e) => RunSelected(testRun: true);
            new ToolTip().SetToolTip(_testRunButton, "Back up prod and copy the backup to the test server, then stop and print the restore command instead of running it.");
            _saveButton = new Button { Text = "Save", AutoSize = true };
            _saveButton.Click += (s, e) => TrySave(showSuccess: true);
            _closeButton = new Button { Text = "Close", AutoSize = true, DialogResult = DialogResult.Cancel };
            buttons.Controls.AddRange(new Control[] { _runButton, _testRunButton, _saveButton, _closeButton });
            bottom.Controls.Add(buttons, 3, 0);

            root.Controls.Add(bottom, 0, 1);
            root.SetColumnSpan(bottom, 2);

            CancelButton = _closeButton;
            FormClosing += OnFormClosing;
        }

        #endregion

        #region Load / bind

        private TransferPair SelectedPair => _pairList.SelectedItem as TransferPair;

        private void LoadAll()
        {
            _loading = true;
            try
            {
                _connections = _cfg.GetConnections();
                _settings = TransferSettingsStore.Load(_cfg);

                _sourceCombo.Items.Clear();
                _targetCombo.Items.Clear();
                foreach (var c in _connections.Where(c => c.provider == TransferGuard.SqlServerProvider).OrderBy(c => c.description))
                {
                    if (c.IsProduction == true)
                        _sourceCombo.Items.Add(new ConnectionItem(c));
                    else
                        _targetCombo.Items.Add(new ConnectionItem(c));
                }

                _localFolderText.Text = _settings.LocalFolder;

                _pairList.Items.Clear();
                foreach (var p in _settings.Pairs)
                    _pairList.Items.Add(p);
            }
            finally
            {
                _loading = false;
            }

            if (_pairList.Items.Count > 0)
                _pairList.SelectedIndex = 0;
            else
                ShowSelectedPair();
            _dirty = false;
        }

        private void ShowSelectedPair()
        {
            var p = SelectedPair;
            _loading = true;
            try
            {
                _editorPanel.Enabled = p != null;
                _duplicateButton.Enabled = _deleteButton.Enabled = _runButton.Enabled = _testRunButton.Enabled = p != null;

                _nameText.Text = p?.Name ?? "";
                SelectConnection(_sourceCombo, p?.SourceConnectionId);
                SelectConnection(_targetCombo, p?.TargetConnectionId);
                _sourceDbText.Text = p?.SourceDatabase ?? "";
                _targetDbText.Text = p?.TargetDatabase ?? "";
                _sourceSsh.SetValue(p?.SourceSsh);
                _targetSsh.SetValue(p?.TargetSsh);
                _scriptText.Text = p?.PostRestoreScriptPath ?? "";
                _relinkText.Text = p?.RelinkUsers ?? "";
                _relinkOwnerCheck.Checked = p?.RelinkUsersAsDbOwner ?? false;
            }
            finally
            {
                _loading = false;
            }
            RefreshHostHints();
            RefreshValidation();
        }

        private static void SelectConnection(ComboBox combo, long? id)
        {
            combo.SelectedItem = id == null ? null
                : combo.Items.Cast<ConnectionItem>().FirstOrDefault(i => i.Connection.p_key == id.Value);
            if (id != null && combo.SelectedItem == null)
                combo.SelectedIndex = -1;
        }

        /// <summary>Copies the editor into the selected pair. Returns true if something was updated.</summary>
        private bool UpdateFromEditor()
        {
            if (_loading)
                return false;
            var p = SelectedPair;
            if (p == null)
                return false;

            p.Name = _nameText.Text.Trim();
            p.SourceConnectionId = (_sourceCombo.SelectedItem as ConnectionItem)?.Connection.p_key ?? 0;
            p.TargetConnectionId = (_targetCombo.SelectedItem as ConnectionItem)?.Connection.p_key ?? 0;
            p.SourceDatabase = _sourceDbText.Text.Trim();
            p.TargetDatabase = _targetDbText.Text.Trim();
            p.SourceSsh = _sourceSsh.GetValue();
            p.TargetSsh = _targetSsh.GetValue();
            p.PostRestoreScriptPath = _scriptText.Text.Trim();
            p.RelinkUsers = _relinkText.Text.Trim();
            p.RelinkUsersAsDbOwner = _relinkOwnerCheck.Checked;
            _dirty = true;
            RefreshValidation();
            return true;
        }

        private void RefreshListText()
        {
            // Force the ListBox to re-read ToString() of the selected item.
            int i = _pairList.SelectedIndex;
            if (i >= 0)
            {
                _loading = true;
                try { _pairList.Items[i] = _pairList.Items[i]; _pairList.SelectedIndex = i; }
                finally { _loading = false; }
            }
        }

        private void RefreshHostHints()
        {
            _sourceSsh.SetHostHint(TransferGuard.HostFromServerName((_sourceCombo.SelectedItem as ConnectionItem)?.Connection.server));
            _targetSsh.SetHostHint(TransferGuard.HostFromServerName((_targetCombo.SelectedItem as ConnectionItem)?.Connection.server));
            _sourceDbText.PlaceholderText = DefaultDbHint(_sourceCombo);
            _targetDbText.PlaceholderText = DefaultDbHint(_targetCombo);
        }

        private static string DefaultDbHint(ComboBox combo)
        {
            var db = (combo.SelectedItem as ConnectionItem)?.Connection.db;
            return string.IsNullOrWhiteSpace(db) ? "(connection has no default database: enter one)" : $"(connection default: {db})";
        }

        /// <summary>"..." button that lists the databases on the selected connection's server.</summary>
        private Button CreateDatabasePickerButton(ComboBox connectionCombo, TextBox dbText)
        {
            var button = new Button { Text = "...", AutoSize = true, MinimumSize = new Size(32, 0) };
            new ToolTip().SetToolTip(button, "List databases on the server");
            button.Click += (s, e) =>
            {
                var con = (connectionCombo.SelectedItem as ConnectionItem)?.Connection;
                if (con == null)
                {
                    MessageBox.Show(this, "Select a SQL connection first.", "Database Transfer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                List<string> names;
                Cursor = Cursors.WaitCursor;
                try
                {
                    names = ListDatabases(con);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Could not list databases on {con.server}:\n\n{ex.Message}", "Database Transfer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    Cursor = Cursors.Default;
                }

                var menu = new ContextMenuStrip();
                var defaultItem = menu.Items.Add(string.IsNullOrWhiteSpace(con.db) ? "(connection default: none)" : $"(connection default: {con.db})");
                defaultItem.Click += (s2, e2) => dbText.Text = "";
                menu.Items.Add(new ToolStripSeparator());
                foreach (var name in names)
                {
                    var item = (ToolStripMenuItem)menu.Items.Add(name);
                    item.Checked = string.Equals(name, string.IsNullOrWhiteSpace(dbText.Text) ? con.db : dbText.Text.Trim(), StringComparison.OrdinalIgnoreCase);
                    item.Click += (s2, e2) => dbText.Text = string.Equals(name, con.db, StringComparison.OrdinalIgnoreCase) ? "" : name;
                }
                menu.Closed += (s2, e2) => BeginInvoke(new Action(menu.Dispose));
                menu.Show(button, new Point(0, button.Height));
            };
            return button;
        }

        private static List<string> ListDatabases(Connection con)
        {
            var csb = new System.Data.SqlClient.SqlConnectionStringBuilder(DatabaseTransferRunner.BuildConnectionString(con, "master")) { ConnectTimeout = 10 };
            using var sql = new System.Data.SqlClient.SqlConnection(csb.ConnectionString);
            sql.Open();
            using var cmd = new System.Data.SqlClient.SqlCommand("SELECT name FROM sys.databases WHERE database_id > 4 ORDER BY name", sql) { CommandTimeout = 15 };
            using var reader = cmd.ExecuteReader();
            var names = new List<string>();
            while (reader.Read())
                names.Add(reader.GetString(0));
            return names;
        }

        private void RefreshValidation()
        {
            var p = SelectedPair;
            if (p == null)
            {
                _validationLabel.Text = _pairList.Items.Count == 0 ? "No transfers configured. Click Add to create one." : "";
                return;
            }
            var errors = TransferGuard.Validate(p, _connections);
            if (!string.IsNullOrWhiteSpace(p.PostRestoreScriptPath) && !File.Exists(p.PostRestoreScriptPath))
                errors.Add("Script file does not exist.");
            _validationLabel.ForeColor = errors.Count == 0 ? Color.DarkGreen : Color.Firebrick;
            _validationLabel.Text = errors.Count == 0 ? "✓ Configuration is valid." : string.Join(Environment.NewLine, errors.Select(e => "• " + e));
        }

        #endregion

        #region Actions

        private void AddPair(TransferPair template)
        {
            var p = template?.Clone() ?? new TransferPair();
            if (template != null)
                p.Name = template.Name + " (copy)";
            _settings.Pairs.Add(p);
            _pairList.Items.Add(p);
            _pairList.SelectedItem = p;
            _dirty = true;
            _nameText.Focus();
            _nameText.SelectAll();
        }

        private void DeleteSelected()
        {
            var p = SelectedPair;
            if (p == null)
                return;
            if (MessageBox.Show(this, $"Delete transfer '{p.Name}'?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            int i = _pairList.SelectedIndex;
            _settings.Pairs.Remove(p);
            _pairList.Items.Remove(p);
            _dirty = true;
            if (_pairList.Items.Count > 0)
                _pairList.SelectedIndex = Math.Min(i, _pairList.Items.Count - 1);
            else
                ShowSelectedPair();
        }

        private bool TrySave(bool showSuccess)
        {
            var problems = new List<string>();
            foreach (var p in _settings.Pairs)
            {
                var errors = TransferGuard.Validate(p, _connections);
                if (errors.Count > 0)
                    problems.Add($"'{p.Name}':" + Environment.NewLine + string.Join(Environment.NewLine, errors.Select(e => "   • " + e)));
            }
            if (problems.Count > 0)
            {
                MessageBox.Show(this, "Cannot save, these transfers are not valid:" + Environment.NewLine + Environment.NewLine +
                    string.Join(Environment.NewLine + Environment.NewLine, problems),
                    "Database Transfer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(_settings.LocalFolder))
                _settings.LocalFolder = TransferSettingsStore.DefaultLocalFolder;

            TransferSettingsStore.Save(_cfg, _settings);
            _dirty = false;
            if (showSuccess)
                MessageBox.Show(this, "Saved.", "Database Transfer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }

        private void RunSelected(bool testRun)
        {
            var p = SelectedPair;
            if (p == null)
                return;

            if (_dirty && !TrySave(showSuccess: false))
                return;

            // Re-read connections: a flag may have been changed since this window opened.
            _connections = _cfg.GetConnections();
            var errors = TransferGuard.Validate(p, _connections);
            if (errors.Count > 0)
            {
                MessageBox.Show(this, "This transfer is not allowed:" + Environment.NewLine + Environment.NewLine +
                    string.Join(Environment.NewLine, errors.Select(e => "• " + e)), "Database Transfer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RefreshValidation();
                return;
            }

            var source = TransferGuard.ResolveSource(p, _connections);
            var target = TransferGuard.ResolveTarget(p, _connections);
            var script = string.IsNullOrWhiteSpace(p.PostRestoreScriptPath) ? "(none)" : p.PostRestoreScriptPath;
            var msg = testRun
                ? $"TEST RUN\n\nBack up PRODUCTION database\n\n    {source.server} / {source.db}\n\n" +
                  $"and copy the backup to {TransferGuard.ResolveSshHost(p.TargetSsh, target)}:{p.TargetSsh.Folder}, then stop.\n\n" +
                  $"The test database {target.server} / {target.db} will NOT be changed and the script will not run. " +
                  "The restore command is printed in the log, and the backup is left on the test server so it can be run manually. Continue?"
                : $"Overwrite TEST database\n\n    {target.server} / {target.db}\n\nwith a copy of PRODUCTION database\n\n    {source.server} / {source.db}\n\n" +
                      $"Post-restore script: {script}\n\nAll data in {target.server} / {target.db} will be replaced. Continue?";
            if (MessageBox.Show(this, msg, testRun ? "Confirm test run" : "Confirm database transfer", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            using var dlg = new TransferProgressForm(p, _connections, _settings.LocalFolder, source, target, testRun);
            dlg.ShowDialog(this);
        }

        private void BrowseScript()
        {
            using var dlg = new OpenFileDialog { Filter = "SQL files (*.sql)|*.sql|All files (*.*)|*.*", Title = "Post-restore script" };
            if (File.Exists(_scriptText.Text))
                dlg.InitialDirectory = Path.GetDirectoryName(Path.GetFullPath(_scriptText.Text));
            if (dlg.ShowDialog(this) == DialogResult.OK)
                _scriptText.Text = dlg.FileName;
        }

        private void BrowseLocalFolder()
        {
            using var dlg = new FolderBrowserDialog { Description = "Local folder for staging backups and run logs", UseDescriptionForTitle = true };
            if (Directory.Exists(_localFolderText.Text))
                dlg.SelectedPath = _localFolderText.Text;
            if (dlg.ShowDialog(this) == DialogResult.OK)
                _localFolderText.Text = dlg.SelectedPath;
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_dirty)
                return;
            var r = MessageBox.Show(this, "Save changes to the transfer configuration?", "Database Transfer", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (r == DialogResult.Cancel || (r == DialogResult.Yes && !TrySave(showSuccess: false)))
                e.Cancel = true;
        }

        #endregion

        /// <summary>Host / user / port / folder editor for one side.</summary>
        private class EndpointEditor : TableLayoutPanel
        {
            private readonly TextBox _host = new TextBox { Width = 160 };
            private readonly TextBox _user = new TextBox { Width = 100, PlaceholderText = "(ssh config)" };
            private readonly NumericUpDown _port = new NumericUpDown { Minimum = 1, Maximum = 65535, Value = 22, Width = 70 };
            private readonly TextBox _folder = new TextBox { PlaceholderText = @"D:\Backups" };
            private bool _setting;

            public event EventHandler Changed;

            public EndpointEditor()
            {
                AutoSize = true;
                ColumnCount = 8;
                RowCount = 1;
                Margin = Padding.Empty;
                for (int i = 0; i < 7; i++)
                    ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                Label L(string t) => new Label { Text = t, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(6, 7, 2, 3) };
                Controls.Add(L("Host"), 0, 0);
                Controls.Add(_host, 1, 0);
                Controls.Add(L("User"), 2, 0);
                Controls.Add(_user, 3, 0);
                Controls.Add(L("Port"), 4, 0);
                Controls.Add(_port, 5, 0);
                Controls.Add(L("Folder"), 6, 0);
                _folder.Dock = DockStyle.Fill;
                Controls.Add(_folder, 7, 0);

                _host.TextChanged += OnChanged;
                _user.TextChanged += OnChanged;
                _port.ValueChanged += OnChanged;
                _folder.TextChanged += OnChanged;
            }

            private void OnChanged(object sender, EventArgs e)
            {
                if (!_setting)
                    Changed?.Invoke(this, EventArgs.Empty);
            }

            public void SetHostHint(string host) =>
                _host.PlaceholderText = string.IsNullOrEmpty(host) ? "(from SQL server)" : host;

            public void SetValue(SshEndpoint ep)
            {
                _setting = true;
                try
                {
                    _host.Text = ep?.Host ?? "";
                    _user.Text = ep?.User ?? "";
                    _port.Value = ep == null || ep.Port < 1 || ep.Port > 65535 ? 22 : ep.Port;
                    _folder.Text = ep?.Folder ?? "";
                }
                finally
                {
                    _setting = false;
                }
            }

            public SshEndpoint GetValue() => new SshEndpoint
            {
                Host = _host.Text.Trim(),
                User = _user.Text.Trim(),
                Port = (int)_port.Value,
                Folder = _folder.Text.Trim(),
            };
        }
    }
}
