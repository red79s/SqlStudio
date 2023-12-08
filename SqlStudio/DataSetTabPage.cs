using CfgDataStore;
using Common;
using Common.Model;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SqlStudio
{
    public class DataSetTabPage : TabPage
    {
        public delegate void UpdatedResultsDelegate(object sender, int rows, string message);
        public event UpdatedResultsDelegate UpdatedResults;
        public EventHandler<int> VisibleRowsChanged;

        private IServiceProvider _serviceProvider;
        private IList<SqlResult> _results;
        public DataSetTabPage(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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

        public IList<SqlResult> GetSqlResults()
        {
            return _results;
        }

        private string GetToolTipText(IList<SqlResult> results)
        {
            string toolTipText = string.Empty;
            foreach (SqlResult result in results)
            {
                toolTipText += result.SqlQuery + Environment.NewLine;
            }
            return toolTipText;
        }
        public void SetResults(IList<SqlResult> results)
        {
            _results = results;

            Controls.Clear();
            SplitContainer lastSplit = null;

            if (results.Count > 0)
            {
                ToolTipText = GetToolTipText(results);

                if (results.Count == 1)
                {
                    Text = results[0].TableName;
                    if (string.IsNullOrEmpty(Text))
                    {
                        Text = results[0].ResType.ToString();
                    }
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
                    TabDataGridContainer tgrid = new TabDataGridContainer(_serviceProvider);
                    tgrid.FilterRow = DisplayFilterRow;
                    tgrid.UpdatedResults += new TabDataGridContainer.UpdatedResultsDelegate(tgrid_UpdatedResults);
                    tgrid.VisibleRowsChanged += (s, e) => { VisibleRowsChanged?.Invoke(s, e); };
                    tgrid.Dock = DockStyle.Fill;
                    tgrid.SqlResult = results[i];
                    lastSplit.Panel1.Controls.Add(tgrid);
                }
                else if (i == 0)
                {
                    TabDataGridContainer tgrid = new TabDataGridContainer(_serviceProvider);
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
                        TabDataGridContainer tgrid = new TabDataGridContainer(_serviceProvider);
                        tgrid.FilterRow = DisplayFilterRow;
                        tgrid.UpdatedResults += new TabDataGridContainer.UpdatedResultsDelegate(tgrid_UpdatedResults);
                        tgrid.VisibleRowsChanged += (s, e) => { VisibleRowsChanged?.Invoke(s, e); };
                        tgrid.Dock = DockStyle.Fill;
                        tgrid.SqlResult = results[i];
                        lastSplit.Panel2.Controls.Add(tgrid);
                    }
                    else
                    {
                        TabDataGridContainer tgrid = new TabDataGridContainer(_serviceProvider);
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
