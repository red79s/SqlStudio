using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using ConfigDataStore;
using SqlExecute.DbSchemaCache;

namespace EntityGenerator
{
    public partial class TableSelector : Form
    {
        private DataView _view = null;
        private DataTable _tables = null;
        private CfgDataStore _cfgStore = null;
        private SqlExecute.DbSchemaCache.DbSchemaCache _cache = null;
        private List<TableDef> _tablesToGenerate = new List<TableDef>();

        public TableSelector(SqlExecute.DbSchemaCache.DbSchemaCache cache, CfgDataStore cfgStore)
        {
            _cfgStore = cfgStore;
            _cache = cache;
            InitializeComponent();
            this.dgvTables.AllowUserToAddRows = false;

            this.tbFilter.TextChanged += new EventHandler(tbFilter_TextChanged);

            InitializeComboBoxes();
            LoadData();
            InitializeGrid();
        }

        private void InitializeComboBoxes()
        {
            for (int i = 0; i < 10; i++)
            {
                string dir = _cfgStore.GetStringValue(string.Format("EntityGenerator_Directory{0}", i));
                if (dir != "")
                    this.cbOutputDirectory.Items.Add(dir);
                string nspace = _cfgStore.GetStringValue(string.Format("EntityGenerator_Namespace{0}", i));
                if (nspace != "")
                    this.cbNamespace.Items.Add(nspace);
            }
            if (this.cbOutputDirectory.Items.Count > 0)
            {
                this.cbOutputDirectory.SelectedIndex = 0;
            }
            if (this.cbNamespace.Items.Count > 0)
            {
                this.cbNamespace.SelectedIndex = 0;
            }
        }

        public void StoreComboBoxes()
        {
            for (int i = 0; i < this.cbOutputDirectory.Items.Count && i < 10; i++)
                _cfgStore.SetValue(string.Format("EntityGenerator_Directory{0}", i), (string)this.cbOutputDirectory.Items[i]);
            for (int i = 0; i < this.cbNamespace.Items.Count && i < 10; i++)
                _cfgStore.SetValue(string.Format("EntityGenerator_Namespace{0}", i), (string)this.cbNamespace.Items[i]);
        }

        void tbFilter_TextChanged(object sender, EventArgs e)
        {
            string filter = string.Format("[Table Name] LIKE '%{0}%'", this.tbFilter.Text);
            _view.RowFilter = filter;
            SetColumnWidths();
        }

        private void InitializeGrid()
        {
            _view = new DataView(_tables);

            this.dgvTables.CellContentClick += new DataGridViewCellEventHandler(dgvTables_CellContentClick);
            this.dgvTables.DataSource = _view;

            DataGridViewButtonColumn editColumns = new DataGridViewButtonColumn();
            editColumns.Name = "Columns";
            editColumns.HeaderText = "Columns";
            editColumns.Text = "Edit";
            editColumns.UseColumnTextForButtonValue = true;
            editColumns.FlatStyle = FlatStyle.Standard;
            editColumns.DisplayIndex = 3;
            this.dgvTables.Columns.Insert(3, editColumns);

            DataGridViewButtonColumn relationsColumns = new DataGridViewButtonColumn();
            relationsColumns.Name = "Relations";
            relationsColumns.HeaderText = "Relations";
            relationsColumns.Text = "Add";
            relationsColumns.UseColumnTextForButtonValue = true;
            relationsColumns.FlatStyle = FlatStyle.Standard;
            relationsColumns.DisplayIndex = 4;
            this.dgvTables.Columns.Insert(4, relationsColumns);

            this.dgvTables.Columns["Index"].Visible = false;
            this.dgvTables.Resize += new EventHandler(dgvTables_Resize);

            SetColumnWidths();
        }

        void dgvTables_Resize(object sender, EventArgs e)
        {
            SetColumnWidths();
        }

        void SetColumnWidths()
        {
            int tableWidth = this.dgvTables.Width - (170 + this.dgvTables.RowHeadersWidth);
            if (this.dgvTables.ScrollBars == ScrollBars.Vertical || this.dgvTables.ScrollBars == ScrollBars.Both)
                tableWidth -= (SystemInformation.VerticalScrollBarWidth + 2);

            this.dgvTables.Columns["Select"].Width = 50;
            this.dgvTables.Columns["Table Name"].Width = tableWidth;
            this.dgvTables.Columns["Columns"].Width = 60;
            this.dgvTables.Columns["Relations"].Width = 60;
        }

        void dgvTables_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = (int)this.dgvTables.Rows[e.RowIndex].Cells["Index"].Value;
            _tables.Rows[index]["Select"] = true;
            string tableName = (string)_tables.Rows[index]["Table Name"];

            TableDef td = new TableDef(tableName);
            ColumnList cl = new ColumnList(_cache, td);
            cl.StartPosition = FormStartPosition.CenterParent;
            if (cl.ShowDialog() == DialogResult.OK)
                _tablesToGenerate.Add(td);
            else
                _tables.Rows[index]["Select"] = false;
        }

        public void LoadData()
        {
            _tables = new DataTable();
            _tables.Columns.Add("Index", typeof(int));
            _tables.Columns.Add("Select", typeof(bool));
            _tables.Columns.Add("Table Name", typeof(string));

            int index = 0;
            List<string> tables = _cache.GetTables("%");
            foreach (string table in tables)
            {
                DataRow dr = _tables.NewRow();
                dr["Index"] = index;
                dr["Select"] = false;
                dr["Table Name"] = table;
                _tables.Rows.Add(dr);
                index++;
            }
        }

        private void CollectTableDefs()
        {
            for (int i = 0; i < _tables.Rows.Count; i++)
            {
                DataRow dr = _tables.Rows[i];
                string tableName = (string)dr["Table Name"];
                TableDef tdef = GetDefFromList(tableName);
                if ((bool)dr["Select"] == false)
                {
                    if (tdef != null)
                        _tablesToGenerate.Remove(tdef);
                    continue;
                }

                if (tdef != null)
                    continue;

                _tablesToGenerate.Add(GenerateTableDef(tableName));
            }
        }

        private TableDef GenerateTableDef(string tableName)
        {
            TableDef tDef = new TableDef(tableName);

            _cache.ColumnCache.Clear();
            _cache.ColumnCache.FillQuery(string.Format("{0} = '{1}'", ColumnCacheDataRow.table_nameColumn, tableName));
            DataRow[] columns = _cache.ColumnCache.Select("", ColumnCacheDataRow.ordinal_positionColumn);

            foreach (ColumnCacheDataRow col in columns)
            {
                ColumnDef colDef = new ColumnDef(col.column_name, col.data_type);
                colDef.DataLength = col.column_length;
                colDef.IsKey = col.primary_key;
                colDef.IsNullable = col.is_nullable;
                colDef.IsVirtual = false;
                tDef.Columns.Add(colDef);
            }

            return tDef;
        }

        private void GenerateEntites(string path, string nameSpace)
        {
            GenerateEntity genEnt = new GenerateEntity();
            GenerateBaseEntity genBase = new GenerateBaseEntity();

            foreach (TableDef tableDef in _tablesToGenerate)
            {
                string className = tableDef.GetCSName(tableDef.TableName) + "Entity";
                string baseClass = tableDef.GetCSName(tableDef.TableName) + "EntityBase";

                string entityFile = path + '/' + className + ".cs";
                if (!File.Exists(entityFile))
                {
                    string fileContent = genEnt.Generate(path, nameSpace, className, baseClass, tableDef);
                    File.WriteAllText(path + '/' + className + ".cs", fileContent);
                }

                string baseFile = path + '/' + baseClass + ".cs";
                
                string baseContent = genBase.Generate(path, nameSpace, baseClass, "BaseEntity", tableDef);
                File.WriteAllText(baseFile, baseContent);
            }
        }

        private TableDef GetDefFromList(string tableName)
        {
            foreach (TableDef tdef in _tablesToGenerate)
            {
                if (tdef.TableName == tableName)
                    return tdef;
            }
            return null;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string dir = (string)this.cbOutputDirectory.SelectedItem;
            if (dir == null)
            {
                dir = this.cbOutputDirectory.Text;
                if (dir != "")
                    this.cbOutputDirectory.Items.Insert(0, dir);
            }

            string nspace = (string)this.cbNamespace.SelectedItem;
            if (nspace == null)
            {
                nspace = this.cbNamespace.Text;
                if (nspace != "")
                    this.cbNamespace.Items.Insert(0, nspace);
            }

            StoreComboBoxes();

            CollectTableDefs();

            GenerateEntites(dir, nspace);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnSelectDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string dir = fbd.SelectedPath;
                this.cbOutputDirectory.Items.Insert(0, dir);
                this.cbOutputDirectory.SelectedIndex = 0;
            }
        }
    }
}
