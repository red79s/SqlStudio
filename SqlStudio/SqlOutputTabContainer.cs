using Common;
using Common.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

#pragma warning disable CA1416

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
        private IServiceProvider _serviceProvider = null;

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
            _textBoxOutput.Font = new Font(FontFamily.GenericMonospace, 12F);
            _outputTab.Controls.Add(_textBoxOutput);
            _textBoxOutput.Dock = DockStyle.Fill;

            TabPages.Add(_outputTab);

            var addTabPage = new TabPage("   +");
            TabPages.Add(addTabPage);
        }

        public void SetDependencyObjects(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private ContextMenuStrip CreateDataTabsContextMenu()
        {
            var cms = new ContextMenuStrip();
            var tsmiDataClose = new ToolStripMenuItem("Close", null, tsmiDataClose_Click);
            cms.Items.Add(tsmiDataClose);
            var tsmiDataCloseAll = new ToolStripMenuItem("Close All", null, tsmiDataCloseAll_Click);
            cms.Items.Add(tsmiDataCloseAll);
            var tsmiDataCloseAllButThis = new ToolStripMenuItem("Close All But This", null, tsmiDataCloseAllButThis_Click);
            cms.Items.Add(tsmiDataCloseAllButThis);
            var tsmiDataRename = new ToolStripMenuItem("Rename", null, tsmiDataRename_Click);
            cms.Items.Add(tsmiDataRename);
            var tsmiDataRefresh = new ToolStripMenuItem("Refresh", null, tsmiDataRefresh_Click);
            tsmiDataRefresh.ShortcutKeys = Keys.F5;
            cms.Items.Add(tsmiDataRefresh);
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

        private void tsmiDataCloseAll_Click(object sender, EventArgs e)
        {
			if (TabPages.Count > 2)
            {
				for (int i = TabPages.Count - 2; i >= 1; i--)
				{
						TabPages.RemoveAt(i);
				}
			}
		}

        private void tsmiDataCloseAllButThis_Click(object sender, EventArgs e)
        {
            if (TabPages.Count > 2)
            {
                var currentTab = SelectedTab;
                for (int i = TabPages.Count - 2; i >= 1; i--)
                {
                    if (TabPages[i] != currentTab)
                    {
                        TabPages.RemoveAt(i);
                    }
                }
            }
		}

		void tsmiDataRename_Click(object sender, EventArgs e)
        {
            RenameDialog rnd = new RenameDialog();
            rnd.NameText = SelectedTab.Text;
            if (rnd.ShowDialog() == DialogResult.OK)
                SelectedTab.Text = rnd.NameText;
        }

        void tsmiDataRefresh_Click(object sender, EventArgs e)
        {
            if (SelectedTab is DataSetTabPage)
            {
                var results = ((DataSetTabPage)SelectedTab).GetSqlResults();
                string queries = "";
                foreach (var res in results)
                {
                    if (!string.IsNullOrEmpty(res.SqlQuery))
                    {
                        queries += res.SqlQuery;
                    }
                }

                if (queries != "")
                {
                    var executeCallback = _serviceProvider.GetService<IExecuteQueryCallback>();
                    executeCallback.ExecuteQuery(queries, false, "");
                }
            }
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

        public void DisplayResults(IList<SqlResult> results)
        {
            if (results.Count < 1)
                return;

            for (int i = results.Count - 1; i >= 0; i--)
            {
                SqlResult res = results[i];
                if (res.DisplayAsText && res.DataTable != null)
                {
                    SetOutputText(DataFormatingUtils.GetDataTableAsString(res.DataTable, true, res.TableName, _resCounter++));
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

        private void InsertNewTabPage(TabPage tabPage)
        {
            TabPages.Insert(TabCount - 1, tabPage);
            SelectedTab = tabPage;
        }

        public void CreateNewImageTab(string label, Bitmap bm)
        {
            ImageOutputTabPage iotp = new ImageOutputTabPage();
            iotp.SetImage(bm);
            iotp.Text = label;
            InsertNewTabPage(iotp);
        }

        public void CreateNewGraphTab(string label, GraphData data)
        {
            GraphOutputTabPage gotp = new GraphOutputTabPage();
            gotp.Text = label;
            gotp.SetData(data);
            InsertNewTabPage(gotp);
        }

        public void CreateNewTextOutputTab(string label, string text)
        {
            TextOutputTabPage totp = new TextOutputTabPage();
            totp.SetText(text);
            totp.Text = label;
            InsertNewTabPage(totp);
        }

        private DataSetTabPage InsertNewDataTab(string label)
        {
            DataSetTabPage dstp = new DataSetTabPage(_serviceProvider);
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
}
