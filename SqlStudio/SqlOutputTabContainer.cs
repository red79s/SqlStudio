using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

using SqlExecute;
using CfgDataStore;

namespace SqlStudio
{
    public class SqlOutputTabContainer : TabControl
    {
        public delegate void UpdatedResultsDelegate(object sender, int rows, string message);
        public event UpdatedResultsDelegate UpdatedResults;
        public EventHandler<int> VisibleRowsChanged;

        private TabPage _outputTab = null;
        private TextBox _textBoxOutput = null;
        private int _resCounter = 0;

        ContextMenuStrip _dataTabContextMenu = null;
        private ConfigDataStore _configDataStore;
        private IExecuteQueryCallback _executeCallback;

        public SqlOutputTabContainer()
        {
            ShowToolTips = true;

            _dataTabContextMenu = CreateDataTabsContextMenu();

            TabPages.Clear();
            _outputTab = new TabPage("Output");
            _textBoxOutput = new TextBox();
            _textBoxOutput.KeyDown += (sender, args) =>
            {
                if (args.Control && args.KeyCode == Keys.A)
                {
                    _textBoxOutput.SelectAll();
                    args.SuppressKeyPress = true;
                }
            };
            _textBoxOutput.Multiline = true;
            _textBoxOutput.ScrollBars = ScrollBars.Both;
            _textBoxOutput.WordWrap = false;
            _textBoxOutput.Font = new System.Drawing.Font("Lucida Sans Typewriter", 12F, System.Drawing.FontStyle.Regular, 
                                                                System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _outputTab.Controls.Add(_textBoxOutput);
            _textBoxOutput.Dock = DockStyle.Fill;

            TabPages.Add(_outputTab);

            var addTabPage = new TabPage("   +");
            TabPages.Add(addTabPage);
        }

        public void SetConfig(ConfigDataStore configDataStore)
        {
            _configDataStore = configDataStore;
        }

        public void SetExecuteCallback(IExecuteQueryCallback executeCallback)
        {
            _executeCallback = executeCallback;
        }

        private ContextMenuStrip CreateDataTabsContextMenu()
        {
            ContextMenuStrip cms = new ContextMenuStrip();
            ToolStripMenuItem tsmiDataClose = new ToolStripMenuItem("Close", null, tsmiDataClose_Click);
            cms.Items.Add(tsmiDataClose);
            ToolStripMenuItem tsmiDataRename = new ToolStripMenuItem("Rename", null, tsmiDataRename_Click);
            cms.Items.Add(tsmiDataRename);
            return cms;
        }

        private bool _dispFilterRow = false;
        public bool DisplayFilterRow
        {
            get { return _dispFilterRow; }
            set
            {
                _dispFilterRow = value;
                foreach (TabPage tp in TabPages)
                {
                    if (tp is DataSetTabPage)
                        ((DataSetTabPage)tp).DisplayFilterRow = value;
                }

            }
        }

        void tsmiDataClose_Click(object sender, EventArgs e)
        {
            TabPages.Remove(SelectedTab);
        }

        void tsmiDataRename_Click(object sender, EventArgs e)
        {
            RenameDialog rnd = new RenameDialog();
            rnd.NameText = SelectedTab.Text;
            if (rnd.ShowDialog() == DialogResult.OK)
                SelectedTab.Text = rnd.NameText;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < TabPages.Count; i++)
                {
                    if (GetTabRect(i).Contains(e.Location))
                    {
                        if (TabPages[i] is DataSetTabPage || 
                            TabPages[i] is ImageOutputTabPage || 
                            TabPages[i] is TextOutputTabPage ||
                            TabPages[i] is GraphOutputTabPage)
                        {
                            SelectedTab = TabPages[i];
                            _dataTabContextMenu.Show(this, e.Location);
                        }
                        return;
                    }
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                var lastIndex = TabCount - 1;
                if (GetTabRect(lastIndex).Contains(e.Location))
                {
                    InsertNewDataTab("Data");
                }
            }
        }

        protected override void OnSelecting(TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == (TabCount - 1))
            {
                e.Cancel = true;
            }
        }

        public void DisplayResults(List<SqlResult> results)
        {
            if (results.Count < 1)
                return;

            for (int i = results.Count - 1; i >= 0; i--)
            {
                SqlResult res = results[i];
                if (res.DisplayAsText && res.DataTable != null)
                {
                    SetOutputText(DataFormatingUtils.GetDataTableAsString(res.DataTable, true, _resCounter++));
                    results.RemoveAt(i);
                }
                else if (res.DataTable == null)
                {
                    results.RemoveAt(i);
                }
            }

            if (results.Count < 1)
                return;

            DataSetTabPage currentDataTab = GetCurrentDataTab();
            currentDataTab.SetResults(results);
        }

        private DataSetTabPage GetCurrentDataTab()
        {
            TabPage tp = SelectedTab;
            if (tp is DataSetTabPage)
                return (DataSetTabPage)tp;
            for (int i = TabPages.Count - 1; i >= 0; i--)
            {
                if (TabPages[i] is DataSetTabPage)
                    return (DataSetTabPage)TabPages[i];
            }

            return InsertNewDataTab(null);
        }

        public void CreateNewDataTab(string label)
        {
            InsertNewDataTab(label);
        }

        public void CreateNewImageTab(string label, Bitmap bm)
        {
            ImageOutputTabPage iotp = new ImageOutputTabPage();
            iotp.SetImage(bm);
            iotp.Text = label;
            TabPages.Add(iotp);
            SelectedTab = iotp;
        }

        public void CreateNewGraphTab(string label, GraphData data)
        {
            GraphOutputTabPage gotp = new GraphOutputTabPage();
            gotp.Text = label;
            gotp.SetData(data);
            TabPages.Add(gotp);
            SelectedTab = gotp;
        }

        public void CreateNewTextOutputTab(string label, string text)
        {
            TextOutputTabPage totp = new TextOutputTabPage();
            totp.SetText(text);
            totp.Text = label;
            TabPages.Add(totp);
            SelectedTab = totp;
        }

        private DataSetTabPage InsertNewDataTab(string label)
        {
            DataSetTabPage dstp = new DataSetTabPage(_configDataStore, _executeCallback);
            dstp.DisplayFilterRow = DisplayFilterRow;
            dstp.UpdatedResults += new DataSetTabPage.UpdatedResultsDelegate(_currentDataTab_UpdatedResults);
            dstp.VisibleRowsChanged += (s, e) => { VisibleRowsChanged?.Invoke(s, e); };
            if (label != null && label != "")
                dstp.Text = label;
            else
                dstp.Text = GetNextTabLabel();

            TabPages.Insert(TabCount -1, dstp);
            SelectedTab = dstp;
            return dstp;
        }

        private string GetNextTabLabel()
        {
            int num = 0;
            foreach (TabPage tp in TabPages)
            {
                if (tp is DataSetTabPage)
                    num++;
            }

            if (num < 1)
                return "Data";
            return string.Format("Data {0}", num);
        }

        void _currentDataTab_UpdatedResults(object sender, int rows, string message)
        {
            UpdatedResults?.Invoke(sender, rows, message);
        }

        public void SetOutputText(string output)
        {
            _textBoxOutput.AppendText(output + Environment.NewLine);
            _textBoxOutput.SelectionLength = 0;
            _textBoxOutput.SelectionStart = _textBoxOutput.Text.Length;
            _textBoxOutput.ScrollToCaret();
        }
    }

    public class DataSetTabPage : TabPage
    {
        public delegate void UpdatedResultsDelegate(object sender, int rows, string message);
        public event UpdatedResultsDelegate UpdatedResults;
        public EventHandler<int> VisibleRowsChanged;

        private ConfigDataStore _configDataStore;
        private readonly IExecuteQueryCallback _executeQueryCallback;

        public DataSetTabPage(ConfigDataStore configDataStore, IExecuteQueryCallback executeQueryCallback)
        {
            _configDataStore = configDataStore;
            _executeQueryCallback = executeQueryCallback;
        }

        private bool _dispFilterRow = false;
        public bool DisplayFilterRow
        {
            get { return _dispFilterRow; }
            set
            {
                _dispFilterRow = value;
                foreach (TabDataGridContainer tdgc in Controls)
                {
                    tdgc.FilterRow = value;
                }
            }
        }

        public void SetResults(List<SqlResult> results)
        {
            Controls.Clear();
            SplitContainer lastSplit = null;

            if (results.Count > 0)
            {
                ToolTipText = results[0].SqlQuery;
                if (results.Count == 1)
                {
                    Text = results[0].TableName;
                }
                else
                {
                    Text = "Multiple";
                }
            }

            for (int i = 0; i < results.Count; i++)
            {
                if (i == 0 && results.Count > 1)
                {
                    lastSplit = new SplitContainer();
                    lastSplit.Orientation = Orientation.Horizontal;
                    lastSplit.Dock = DockStyle.Fill;
                    Controls.Add(lastSplit);
                    TabDataGridContainer tgrid = new TabDataGridContainer(_configDataStore, _executeQueryCallback);
                    tgrid.FilterRow = DisplayFilterRow;
                    tgrid.UpdatedResults += new TabDataGridContainer.UpdatedResultsDelegate(tgrid_UpdatedResults);
                    tgrid.VisibleRowsChanged += (s, e) => { VisibleRowsChanged?.Invoke(s, e); };
                    tgrid.Dock = DockStyle.Fill;
                    tgrid.SqlResult = results[i];
                    lastSplit.Panel1.Controls.Add(tgrid);
                }
                else if (i == 0)
                {
                    TabDataGridContainer tgrid = new TabDataGridContainer(_configDataStore, _executeQueryCallback);
                    tgrid.FilterRow = DisplayFilterRow;
                    tgrid.UpdatedResults += new TabDataGridContainer.UpdatedResultsDelegate(tgrid_UpdatedResults);
                    tgrid.VisibleRowsChanged += (s, e) => { VisibleRowsChanged?.Invoke(s, e); };
                    tgrid.Dock = DockStyle.Fill;
                    Controls.Add(tgrid);
                    tgrid.SqlResult = results[i];
                }
                else
                {
                    if (i < (results.Count - 1))
                    {
                        SplitContainer sc = new SplitContainer();
                        sc.Orientation = Orientation.Horizontal;
                        sc.Dock = DockStyle.Fill;
                        lastSplit.Panel2.Controls.Add(sc);
                        lastSplit = sc;
                        TabDataGridContainer tgrid = new TabDataGridContainer(_configDataStore, _executeQueryCallback);
                        tgrid.FilterRow = DisplayFilterRow;
                        tgrid.UpdatedResults += new TabDataGridContainer.UpdatedResultsDelegate(tgrid_UpdatedResults);
                        tgrid.VisibleRowsChanged += (s, e) => { VisibleRowsChanged?.Invoke(s, e); };
                        tgrid.Dock = DockStyle.Fill;
                        tgrid.SqlResult = results[i];
                        lastSplit.Panel2.Controls.Add(tgrid);
                    }
                    else
                    {
                        TabDataGridContainer tgrid = new TabDataGridContainer(_configDataStore, _executeQueryCallback);
                        tgrid.FilterRow = DisplayFilterRow;
                        tgrid.UpdatedResults += new TabDataGridContainer.UpdatedResultsDelegate(tgrid_UpdatedResults);
                        tgrid.VisibleRowsChanged += (s, e) => { VisibleRowsChanged?.Invoke(s, e); };
                        tgrid.Dock = DockStyle.Fill;
                        tgrid.SqlResult = results[i];
                        lastSplit.Panel2.Controls.Add(tgrid);
                    }
                }
            }
        }

        void tgrid_UpdatedResults(object sender, int rows, string message)
        {
            if (UpdatedResults != null)
                UpdatedResults(sender, rows, message);
        }
    }
}
