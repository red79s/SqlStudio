using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CfgDataStore;
using Common;
using Common.Model;

namespace SqlStudio
{
    public class TabDataGridContainer : Control
    {
        public delegate void UpdatedResultsDelegate(object sender, int rows, string message);
        public event UpdatedResultsDelegate UpdatedResults;
        public EventHandler<int> VisibleRowsChanged;

        private TabDataGrid _tdg = null;
        private bool _filterRow = true;
        private List<TextBox> _filterControlls = null;
        private ISearchControl _searchControl = null;
        private IServiceProvider _serviceProvider = null;

        public TabDataGridContainer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;    
            _filterControlls = new List<TextBox>();

            _searchControl = new SearchControl();
            _searchControl.SearchDown += _searchControl_SearchDown;
            _searchControl.SearchUp += _searchControl_SearchUp;
            _searchControl.HideRows += _searchControl_HideRows;
            _searchControl.UnhideRows += _searchControl_UnhideRows;
            _searchControl.IsVisible = true;
            Controls.Add(_searchControl as UserControl);

            _tdg = new TabDataGrid(_serviceProvider);
            _tdg.UpdatedResults += new TabDataGrid.UpdatedResultsDelegate(_tdg_UpdatedResults);
            _tdg.ColumnWidthChanged += new DataGridViewColumnEventHandler(_tdg_ColumnWidthChanged);
            _tdg.Scroll += new ScrollEventHandler(_tdg_Scroll);
            _tdg.Dock = DockStyle.Fill;
            _tdg.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText;
            _tdg.CellDoubleClick += _tdg_CellDoubleClick;
            Controls.Add(_tdg);

            SetLayout();
        }

        const int WM_KEYDOWN = 0x100;
        private readonly IExecuteQueryCallback _executeQueryCallback;
        private readonly IDatabaseSchemaInfo _databaseSchemaInfo;
        private readonly IDatabaseKeywordEscape _databaseKeywordEscape;
        private readonly IColumnValueDescriptionProvider _columnMetadataInfo;

        protected override bool ProcessKeyPreview(ref Message m)
        {
            if (m.Msg == WM_KEYDOWN && (Keys)m.WParam == Keys.F5)
            {
                if (_tdg != null)
                {
                    var res = _tdg.GetLastSqlResult();
                    if (res != null && res.SqlQuery != null )
                    {
                        _executeQueryCallback.ExecuteQuery(res.SqlQuery, false, "");
                    }
                }
                return true;
            }
            return base.ProcessKeyPreview(ref m);
        }

        private void _tdg_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            _searchControl.SetSearchText(_tdg.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
        }

        private void _searchControl_HideRows(object sender, string e)
        {
            if (_tdg.SelectedCells.Count < 1)
            {
                return;
            }
            var filterValue = new HideRowFilterValue { ColumnName = _tdg.SelectedCells[0].OwningColumn.Name, FilterText = e };
            _hideFilterStrings.Add(filterValue);
            try
            {
                FilterChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Invalid filter: {ex.Message}");
                _hideFilterStrings.Remove(filterValue);
            }
        }

        private void _searchControl_UnhideRows(object sender, EventArgs e)
        {
            _hideFilterStrings.Clear();
            FilterChanged();
        }

        private void _searchControl_SearchUp(object sender, string e)
        {
            _tdg.FindText(e, false);
        }

        private void _searchControl_SearchDown(object sender, string e)
        {
            _tdg.FindText(e, true);
        }

        void _tdg_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                SetLayout();
        }

        void _tdg_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            SetLayout();
        }

        void _tdg_UpdatedResults(object sender, int rows, string message)
        {
            if (UpdatedResults != null)
                UpdatedResults(sender, rows, message);
        }

        public SqlResult SqlResult
        {
            get { return _tdg.SqlResult; }
            set
            {
                _tdg.SqlResult = value;
                SetLayout();
                FilterChanged();
            }
        }

        public bool FilterRow
        {
            get { return _filterRow; }
            set
            {
                _filterRow = value;
                SetLayout();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            SetLayout();
        }

        private void SetLayout()
        {
            if (FilterRow == false || _tdg.RowCount < 1)
            {
                _tdg.Dock = DockStyle.Fill;
                _tdg.Top = 0;
                _tdg.Left = 0;
                _tdg.Width = Width;
                _tdg.Height = Height;
                return;
            }

            var searchControl = _searchControl as UserControl;
            searchControl.Dock = DockStyle.None;
            searchControl.Top = 0;
            searchControl.Left = 0;
            searchControl.Size = new Size(Width, 29);
            searchControl.Visible = _searchControl.IsVisible;

            var filterTop = _searchControl.IsVisible ? searchControl.Height + 1 : 0;

            int gridTop = 0;
            if (_filterControlls.Count < 1)
                InitFilterRow();
            if (_filterControlls.Count > 0)
                gridTop = _filterControlls[0].Height + 1 + filterTop;


            _tdg.Dock = DockStyle.None;
            _tdg.Top = gridTop;
            _tdg.Left = 0;
            _tdg.Width = Width;
            _tdg.Height = Height - gridTop;

            for (int i = 0; i < _filterControlls.Count; i++)
            {
                Rectangle rec = _tdg.GetCellDisplayRectangle(i, -1, true);
                TextBox tb = _filterControlls[i];
                tb.Top = filterTop;
                tb.Left = rec.Left;
                tb.Width = rec.Width;
            }
        }

        private void InitFilterRow()
        {
            if (FilterRow == false)
            {
                return;
            }

            _filterControlls.Clear();

            for (int i = 0; i < _tdg.Columns.Count; i++)
            {
                TextBox tb = new TextBox();
                tb.TextChanged += new EventHandler(tb_TextChanged);
                _filterControlls.Add(tb);
                Controls.Add(tb);
            }
        }

        List<HideRowFilterValue> _hideFilterStrings = new List<HideRowFilterValue>();

        void tb_TextChanged(object sender, EventArgs e)
        {
            FilterChanged();
        }

        private void FilterChanged()
        {
            string filterStr = GetFilterString();

            foreach (var hideVal in _hideFilterStrings)
            {
                var hideFilter = "";
                if (filterStr.Length > 0)
                {
                    hideFilter = " AND ";
                }

                hideFilter += $" [{hideVal.ColumnName}] NOT LIKE '%{hideVal.FilterText}%'";
                filterStr += hideFilter;
            }

            //record selected row
            object obj = null;
            if (_tdg.SelectedRows.Count > 0)
            {
                obj = _tdg.SelectedRows[0].DataBoundItem;
            }
            else if (_tdg.SelectedCells.Count == 1)
            {
                obj = _tdg.SelectedCells[0].OwningRow.DataBoundItem;
            }
            _tdg.Filter = filterStr;

            var visibleRows = _tdg.Rows.GetRowCount(DataGridViewElementStates.Visible);
            VisibleRowsChanged?.Invoke(this, visibleRows);

            //try to restore selected row
            if (obj != null)
            {
                foreach (DataGridViewRow row in _tdg.Rows)
                {
                    if (row.DataBoundItem == obj)
                    {
                        row.Selected = true;
                        _tdg.FirstDisplayedScrollingRowIndex = row.Index;
                        return;
                    }
                }
            }
        }

        private string GetFilterString()
        {
            if (_filterControlls == null)
                return "";

            StringBuilder sbFilter = new StringBuilder();
            for (int i = 0; i < _filterControlls.Count; i++)
            {
                TextBox tb = _filterControlls[i];
                if (tb.Text == "")
                    continue;

                string colName = _tdg.SqlResult.DataTable.Columns[i].ColumnName;
                Type colType = _tdg.SqlResult.DataTable.Columns[i].DataType;

                string filter = "";
                if (sbFilter.Length > 0)
                    filter = " AND ";

                FilterValue fv = new FilterValue(tb.Text, colType);
                if (fv.Operator == "" || fv.Filter == "")
                    continue;

                if (colType == typeof(Guid) || colType == typeof(DateTime))
                    filter += "Convert([" + colName + "], 'System.String') " + fv.Operator + " " + fv.Filter;
                else
                    filter += "[" + colName + "] " + fv.Operator + " " + fv.Filter;

                sbFilter.Append(filter);
            }

            return sbFilter.ToString();
        }
    }

    internal class HideRowFilterValue
    {
        public string ColumnName { get; set; }
        public string FilterText { get; set; }
    }

    internal class FilterValue
    {
        private string _filter = "";
        private string _operator = "";

        public FilterValue(string filter, Type type)
        {
            if (filter == null)
                return;

            filter = filter.Trim();
            if (filter == "")
                return;

            int filterStartIndex = 0;

            if (filter[0] == '=')
            {
                _operator = "=";
                filterStartIndex = 1;
            }
            else if (filter[0] == '>' || filter[0] == '<')
            {
                _operator = filter[0].ToString();
                filterStartIndex = 1;
                if (filter.Length > 1 && filter[1] == '=')
                {
                    _operator += "=";
                    filterStartIndex = 2;
                }
            }
            else if (type == typeof(string) ||
                    type == typeof(Guid) ||
                    type == typeof(DateTime))
            {
                _operator = "LIKE";
            }
            else
            {
                _operator = "=";
            }

            string filterText = filter.Substring(filterStartIndex, filter.Length - filterStartIndex);
            if (filterText == "")
                return;

            if (type == typeof(string) || type == typeof(Guid))
            {
                if (filterStartIndex > 0)
                    _filter = "'" + filterText + "'";
                else
                    _filter = "'%" + filterText + "%'";
            }
            else if (type == typeof(bool))
            {
                if (filterText[0] == 't' || filterText[0] == 'T' || filterText[0] == '1')
                    _filter += "1";
                else
                    _filter += "0";
            }
            else if (type == typeof(int))
            {
                int val = 0;
                if (!int.TryParse(filterText, out val))
                    return;
                _filter = filterText;
            }
            else if (type == typeof(long))
            {
                long val = 0;
                if (!long.TryParse(filterText, out val))
                    return;
                _filter = filterText;
            }
            else if (type == typeof(DateTime))
            {
                _filter = "'%" + filterText + "%'";
            }
            else
            {
                _filter = filterText;
            }
        }

        public string Filter
        {
            get { return _filter; }
        }

        public string Operator
        {
            get { return _operator; }
        }
    }
}