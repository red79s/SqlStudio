using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CfgDataStore;

namespace SqlStudio.DatabaseTransfer
{
    /// <summary>
    /// Runs a transfer and shows a step checklist, a progress bar for the current step, a live elapsed-time
    /// counter and the run log, so a long backup/copy/restore never looks hung.
    /// </summary>
    public class TransferProgressForm : Form
    {
        private readonly TransferPair _pair;
        private readonly IReadOnlyList<Connection> _connections;
        private readonly string _localFolder;
        private readonly bool _testRun;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly Dictionary<TransferStep, ListViewItem> _stepItems = new Dictionary<TransferStep, ListViewItem>();
        private readonly Stopwatch _total = new Stopwatch();
        private readonly Stopwatch _stepWatch = new Stopwatch();

        private ListView _steps;
        private ProgressBar _bar;
        private Label _detailLabel, _elapsedLabel, _resultLabel;
        private TextBox _log;
        private Button _cancelButton, _copyButton, _openFolderButton, _copyRestoreButton;
        private System.Windows.Forms.Timer _timer;

        private DatabaseTransferRunner _runner;
        private bool _running;
        private TransferStep _currentStep;

        public TransferProgressForm(TransferPair pair, IReadOnlyList<Connection> connections, string localFolder, Connection source, Connection target, bool testRun = false)
        {
            _testRun = testRun;
            _pair = pair;
            _connections = connections;
            _localFolder = localFolder;
            BuildUi($"{source.server} / {source.db}", $"{target.server} / {target.db}");
            Shown += async (s, e) => await RunAsync();
            FormClosing += OnFormClosing;
        }

        private void BuildUi(string source, string target)
        {
            Text = (_testRun ? "Database Transfer TEST RUN — " : "Database Transfer — ") + _pair.Name;
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(900, 680);
            MinimumSize = new Size(700, 500);
            Font = new Font("Segoe UI", 9F);
            ShowInTaskbar = false;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 7, Padding = new Padding(10) };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // header
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 200));  // steps
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // detail
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // bar
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // result
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // log
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // buttons
            Controls.Add(root);

            var header = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, WrapContents = true, Margin = new Padding(0, 0, 0, 6) };
            header.Controls.Add(new Label { Text = "PROD  " + source, AutoSize = true, ForeColor = Color.DarkRed, Font = new Font(Font, FontStyle.Bold), Margin = new Padding(0, 0, 8, 0) });
            header.Controls.Add(new Label { Text = "→", AutoSize = true, Font = new Font(Font, FontStyle.Bold), Margin = new Padding(0, 0, 8, 0) });
            header.Controls.Add(new Label { Text = "TEST  " + target, AutoSize = true, ForeColor = Color.DarkGreen, Font = new Font(Font, FontStyle.Bold) });
            if (_testRun)
                header.Controls.Add(new Label { Text = "TEST RUN: test database will not be changed", AutoSize = true, ForeColor = Color.DarkOrange, Font = new Font(Font, FontStyle.Bold), Margin = new Padding(16, 0, 0, 0) });
            root.Controls.Add(header, 0, 0);

            _steps = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                MultiSelect = false,
            };
            _steps.Columns.Add("", 34);
            _steps.Columns.Add("Step", 260);
            _steps.Columns.Add("Details", 540);
            AddStep(TransferStep.Preflight, "Check configuration, SQL and ssh access");
            AddStep(TransferStep.Backup, "Back up production database (SQL)");
            AddStep(TransferStep.Download, "Copy backup from prod to this machine (scp)");
            AddStep(TransferStep.Upload, "Copy backup to test server (scp)");
            AddStep(TransferStep.Restore, _testRun ? "Build restore command (not executed)" : "Restore database on test server (SQL)");
            AddStep(TransferStep.Relink, _testRun ? "Re-link users to logins (skipped)" : "Re-link users to logins");
            AddStep(TransferStep.Script, _testRun ? "Run post-restore script (skipped)" : "Run post-restore script");
            AddStep(TransferStep.Cleanup, "Delete backup files");
            _steps.Resize += (s, e) => _steps.Columns[2].Width = Math.Max(150, _steps.ClientSize.Width - _steps.Columns[0].Width - _steps.Columns[1].Width - 4);
            root.Controls.Add(_steps, 0, 1);

            var detailRow = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 2, Margin = new Padding(0, 8, 0, 2) };
            detailRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            detailRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _detailLabel = new Label { AutoSize = true, Text = "Starting...", Anchor = AnchorStyles.Left };
            _elapsedLabel = new Label { AutoSize = true, Anchor = AnchorStyles.Right, ForeColor = SystemColors.GrayText };
            detailRow.Controls.Add(_detailLabel, 0, 0);
            detailRow.Controls.Add(_elapsedLabel, 1, 0);
            root.Controls.Add(detailRow, 0, 2);

            _bar = new ProgressBar { Dock = DockStyle.Fill, Height = 22, Minimum = 0, Maximum = 1000 };
            root.Controls.Add(_bar, 0, 3);

            _resultLabel = new Label { AutoSize = true, Font = new Font(Font.FontFamily, 11F, FontStyle.Bold), Margin = new Padding(0, 8, 0, 4), Visible = false };
            root.Controls.Add(_resultLabel, 0, 4);

            _log = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                WordWrap = false,
                Font = new Font("Consolas", 9F),
                BackColor = SystemColors.Window,
                Margin = new Padding(0, 6, 0, 6),
            };
            root.Controls.Add(_log, 0, 5);

            var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.RightToLeft, WrapContents = false };
            _cancelButton = new Button { Text = "Cancel", AutoSize = true };
            _cancelButton.Click += (s, e) => OnCancelOrClose();
            _copyButton = new Button { Text = "Copy log", AutoSize = true };
            _copyButton.Click += (s, e) => { if (_log.TextLength > 0) Clipboard.SetText(_log.Text); };
            _openFolderButton = new Button { Text = "Open log folder", AutoSize = true, Enabled = false };
            _openFolderButton.Click += (s, e) => OpenLogFolder();
            _copyRestoreButton = new Button { Text = "Copy restore command", AutoSize = true, Enabled = false, Visible = _testRun };
            _copyRestoreButton.Click += (s, e) => { if (!string.IsNullOrEmpty(_runner?.PlannedRestoreSql)) Clipboard.SetText(_runner.PlannedRestoreSql); };
            buttons.Controls.AddRange(new Control[] { _cancelButton, _copyButton, _openFolderButton, _copyRestoreButton });
            root.Controls.Add(buttons, 0, 6);

            _timer = new System.Windows.Forms.Timer { Interval = 1000 };
            _timer.Tick += (s, e) => UpdateElapsed();
        }

        private void AddStep(TransferStep step, string title)
        {
            var item = new ListViewItem(new[] { "", title, "" });
            _steps.Items.Add(item);
            _stepItems[step] = item;
        }

        private async Task RunAsync()
        {
            _running = true;
            _total.Start();
            _timer.Start();

            var progress = new Progress<TransferEvent>(OnProgress);
            _runner = new DatabaseTransferRunner(_pair, _connections, _localFolder, progress, testRun: _testRun);

            TransferOutcome outcome;
            try
            {
                // Run off the UI thread; Progress<T> marshals events back here.
                outcome = await Task.Run(() => _runner.RunAsync(_cts.Token));
            }
            catch (Exception ex)
            {
                AppendLog("Unexpected error: " + ex);
                outcome = TransferOutcome.Failed;
            }

            _running = false;
            _timer.Stop();
            _total.Stop();
            UpdateElapsed();

            _cancelButton.Text = "Close";
            _cancelButton.Enabled = true;
            _openFolderButton.Enabled = _runner.LogFilePath != null;
            _copyRestoreButton.Enabled = !string.IsNullOrEmpty(_runner.PlannedRestoreSql);

            _resultLabel.Visible = true;
            switch (outcome)
            {
                case TransferOutcome.Succeeded:
                    _resultLabel.Text = _testRun
                        ? "✓ Test run completed: backup is on the test server, restore command is in the log"
                        : "✓ Transfer completed successfully";
                    _resultLabel.ForeColor = Color.DarkGreen;
                    _bar.Value = _bar.Maximum;
                    _detailLabel.Text = "Done.";
                    break;
                case TransferOutcome.Cancelled:
                    _resultLabel.Text = "Transfer cancelled — see log for the state of files and database";
                    _resultLabel.ForeColor = Color.DarkOrange;
                    _detailLabel.Text = "Cancelled.";
                    break;
                default:
                    _resultLabel.Text = "✗ Transfer failed — see log";
                    _resultLabel.ForeColor = Color.Firebrick;
                    _detailLabel.Text = "Failed.";
                    break;
            }

            // Steps that never started.
            foreach (var kv in _stepItems)
            {
                if (kv.Value.Tag == null)
                    SetStatus(kv.Key, StepStatus.Skipped, outcome == TransferOutcome.Succeeded ? null : "Not run");
            }

            System.Media.SystemSounds.Asterisk.Play();
            FlashWindow();
        }

        private void OnProgress(TransferEvent e)
        {
            if (IsDisposed)
                return;

            if (e.LogLine != null)
                AppendLog(e.LogLine);

            if (e.Status.HasValue)
            {
                SetStatus(e.Step, e.Status.Value, e.Detail);
                if (e.Status == StepStatus.Running)
                {
                    _currentStep = e.Step;
                    _stepWatch.Restart();
                    _stepItems[e.Step].EnsureVisible();
                    _detailLabel.Text = _stepItems[e.Step].SubItems[1].Text + "...";
                    _bar.Value = 0;
                    // Cleanup must not be interrupted half-way.
                    if (e.Step == TransferStep.Cleanup)
                        _cancelButton.Enabled = false;
                }
            }
            else if (e.Detail != null && _stepItems.TryGetValue(e.Step, out var item))
            {
                item.SubItems[2].Text = e.Detail;
                if (e.Step == _currentStep && !string.IsNullOrEmpty(e.Detail))
                    _detailLabel.Text = _stepItems[e.Step].SubItems[1].Text + ": " + e.Detail;
            }

            if (e.Percent.HasValue && e.Step == _currentStep && !e.Status.HasValue)
                _bar.Value = Math.Max(0, Math.Min(_bar.Maximum, (int)(e.Percent.Value * 10)));
        }

        private void SetStatus(TransferStep step, StepStatus status, string detail)
        {
            var item = _stepItems[step];
            item.Tag = status;
            switch (status)
            {
                case StepStatus.Running: item.Text = "▶"; item.ForeColor = Color.RoyalBlue; item.Font = new Font(_steps.Font, FontStyle.Bold); break;
                case StepStatus.Succeeded: item.Text = "✓"; item.ForeColor = Color.DarkGreen; item.Font = _steps.Font; break;
                case StepStatus.Failed: item.Text = "✗"; item.ForeColor = Color.Firebrick; item.Font = new Font(_steps.Font, FontStyle.Bold); break;
                case StepStatus.Warning: item.Text = "⚠"; item.ForeColor = Color.DarkOrange; item.Font = _steps.Font; break;
                case StepStatus.Skipped: item.Text = "–"; item.ForeColor = SystemColors.GrayText; item.Font = _steps.Font; break;
                default: item.Text = ""; break;
            }
            if (detail != null)
                item.SubItems[2].Text = detail;
        }

        private void AppendLog(string line)
        {
            _log.AppendText(line + Environment.NewLine);
        }

        private void UpdateElapsed()
        {
            var text = "Elapsed " + DatabaseTransferRunner.FormatElapsed(_total.Elapsed);
            if (_running && _stepWatch.IsRunning)
                text += "   (this step " + DatabaseTransferRunner.FormatElapsed(_stepWatch.Elapsed) + ")";
            _elapsedLabel.Text = text;

            if (_running && _stepItems.TryGetValue(_currentStep, out var item) && item.Tag is StepStatus s && s == StepStatus.Running
                && string.IsNullOrEmpty(item.SubItems[2].Text))
                item.SubItems[2].Text = "working...";
        }

        private void OnCancelOrClose()
        {
            if (!_running)
            {
                Close();
                return;
            }
            if (_cts.IsCancellationRequested || _currentStep == TransferStep.Cleanup)
                return;
            if (MessageBox.Show(this, "Cancel the transfer?\n\nCancelling during restore leaves the test database in RESTORING state.",
                    "Cancel transfer", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;
            AppendLog($"{DateTime.Now:HH:mm:ss}  Cancel requested...");
            _cancelButton.Enabled = false;
            _cancelButton.Text = "Cancelling...";
            _cts.Cancel();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_running)
                return;
            e.Cancel = true;
            OnCancelOrClose();
        }

        private void OpenLogFolder()
        {
            var path = _runner?.LogFilePath;
            if (path == null || !File.Exists(path))
                return;
            try
            {
                Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{path}\"") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Open folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool FlashWindow(IntPtr hwnd, bool invert);

        private void FlashWindow()
        {
            if (Form.ActiveForm != this)
            {
                try { FlashWindow(Handle, true); } catch { }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer?.Dispose();
                _cts.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
