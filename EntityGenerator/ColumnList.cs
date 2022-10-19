using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;

using SqlExecute.DbSchemaCache;

namespace EntityGenerator
{
    public partial class ColumnList : Form
    {
        TableDef _tableDef = null;
        DbSchemaCache _cache = null;
        private DataTable _dt = null;

        public ColumnList(DbSchemaCache cache, TableDef tableDef)
        {
            InitializeComponent();

            _cache = cache;
            _tableDef = tableDef;
            this.Text = "Select Columns for " + _tableDef.TableName;

            InitGrid();
        }

        private void InitGrid()
        {
            _dt = new DataTable();
            _dt.Columns.Add("Select", typeof(bool));
            _dt.Columns.Add("Name", typeof(string));
            _dt.Columns.Add("Type", typeof(string));
            _dt.Columns.Add("Data Length", typeof(int));
            _dt.Columns.Add("Is Key", typeof(bool));
            _dt.Columns.Add("Is Nullable", typeof(bool));
            _dt.Columns.Add("Virtual", typeof(bool));

            _cache.ColumnCache.Clear();
            _cache.ColumnCache.FillQuery(string.Format("{0} = '{1}'", ColumnCacheDataRow.table_nameColumn, _tableDef.TableName));
            DataRow[] columns = _cache.ColumnCache.Select("", ColumnCacheDataRow.ordinal_positionColumn);

            foreach (ColumnCacheDataRow col in columns)
            {
                DataRow dr = _dt.NewRow();
                dr["Select"] = true;
                dr["Name"] = col.column_name;
                dr["Type"] = col.data_type;
                dr["Data Length"] = col.column_length;
                dr["Is Key"] = col.primary_key;
                dr["Is Nullable"] = col.is_nullable;
                dr["Virtual"] = false;
                _dt.Rows.Add(dr);
            }

            this.dgvColumns.DataSource = _dt;
            this.dgvColumns.Columns["Virtual"].Visible = false;

            this.dgvColumns.NewRowNeeded += new DataGridViewRowEventHandler(dgvColumns_NewRowNeeded);

            for (int i = 0; i < 5; i++)
                this.dgvColumns.Columns[i].Width = this.dgvColumns.Columns[i].GetPreferredWidth(DataGridViewAutoSizeColumnMode.DisplayedCells, true);
        }

        void dgvColumns_NewRowNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["Virtual"].Value = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DataRow[] rows = _dt.Select();
            foreach (DataRow row in rows)
            {
                if ((bool)row["Select"] == false)
                    continue;

                ColumnDef colDef = new ColumnDef((string)row["Name"], (string)row["Type"]);
                colDef.DataLength = (int)row["Data Length"];
                colDef.IsKey = (bool)row["Is Key"];
                colDef.IsNullable = (bool)row["Is Nullable"];
                colDef.IsVirtual = (bool)row["Virtual"];
                _tableDef.Columns.Add(colDef);
            }
        }
    }
}
