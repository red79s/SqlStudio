using CfgDataStore;
using Common;
using SqlExecute;
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

        private ConfigDataStore _configDataStore;
        private readonly IExecuteQueryCallback _executeQueryCallback;
        private readonly IDatabaseSchemaInfo _databaseSchemaInfo;
        private readonly IDatabaseKeywordEscape _databaseKeywordEscape;
        private readonly IColumnValueDescriptionProvider _columnMetadataInfo;
        private List<SqlResult> _results;
        public DataSetTabPage(ConfigDataStore configDataStore,
            IExecuteQueryCallback executeQueryCallback,
            IDatabaseSchemaInfo databaseSchemaInfo,
            IDatabaseKeywordEscape databaseKeywordEscape,
            IColumnValueDescriptionProvider columnMetadataInfo)
        {
            _configDataStore = configDataStore;
            _executeQueryCallback = executeQueryCallback;
            _databaseSchemaInfo = databaseSchemaInfo;
            _databaseKeywordEscape = databaseKeywordEscape;
            _columnMetadataInfo = columnMetadataInfo;
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

        public List<SqlResult> GetSqlResults()
        {
            return _results;
        }

        public void SetResults(List<SqlResult> results)
        {
            _results = results;

            Controls.Clear();
            SplitContainer lastSplit = null;

            if (results.Count > 0)
            {
                ToolTipText = results[0].SqlQuery;
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
                    TabDataGridContainer tgrid = new TabDataGridContainer(_configDataStore, _executeQueryCallback, _databaseSchemaInfo, _databaseKeywordEscape, _columnMetadataInfo);
                    tgrid.FilterRow = DisplayFilterRow;
                    tgrid.UpdatedResults += new TabDataGridContainer.UpdatedResultsDelegate(tgrid_UpdatedResults);
                    tgrid.VisibleRowsChanged += (s, e) => { VisibleRowsChanged?.Invoke(s, e); };
                    tgrid.Dock = DockStyle.Fill;
                    tgrid.SqlResult = results[i];
                    lastSplit.Panel1.Controls.Add(tgrid);
                }
                else if (i == 0)
                {
                    TabDataGridContainer tgrid = new TabDataGridContainer(_configDataStore, _executeQueryCallback, _databaseSchemaInfo, _databaseKeywordEscape, _columnMetadataInfo);
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
                        TabDataGridContainer tgrid = new TabDataGridContainer(_configDataStore, _executeQueryCallback, _databaseSchemaInfo, _databaseKeywordEscape, _columnMetadataInfo);
                        tgrid.FilterRow = DisplayFilterRow;
                        tgrid.UpdatedResults += new TabDataGridContainer.UpdatedResultsDelegate(tgrid_UpdatedResults);
                        tgrid.VisibleRowsChanged += (s, e) => { VisibleRowsChanged?.Invoke(s, e); };
                        tgrid.Dock = DockStyle.Fill;
                        tgrid.SqlResult = results[i];
                        lastSplit.Panel2.Controls.Add(tgrid);
                    }
                    else
                    {
                        TabDataGridContainer tgrid = new TabDataGridContainer(_configDataStore, _executeQueryCallback, _databaseSchemaInfo, _databaseKeywordEscape, _columnMetadataInfo);
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
