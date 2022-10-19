using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SqlStudio.AliasesEditor
{
    public struct Alias
    {
        public string name;
        public string value;
        public Alias(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
    }

    public partial class AliasesEditor : Form
    {
        private DataTable _dtAliases = null;

        public AliasesEditor()
        {
            InitializeComponent();
            this._dtAliases = new DataTable();
            this._dtAliases.Columns.Add("Name", typeof(string));
            this._dtAliases.Columns.Add("Value", typeof(string));
            this.InitData();
        }

        private void InitData()
        {
            this.dataGridView.DataSource = this._dtAliases;
            this.dataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView.RowHeadersWidth = 15;
        }

        public void AddAlias(Alias alias)
        {
            DataRow dr = this._dtAliases.NewRow();
            dr["Name"] = alias.name;
            dr["Value"] = alias.value;
            this._dtAliases.Rows.Add(dr);
            this._dtAliases.AcceptChanges();
            this.InitData();
        }

        public List<Alias> GetModified()
        {
            List<Alias> ret = new List<Alias>();
            foreach (DataRow dr in this._dtAliases.Rows)
            {
                if (dr.RowState == DataRowState.Modified)
                    ret.Add(new Alias((string)dr["Name"], (string)dr["Value"]));
            }
            return ret;
        }

        public List<Alias> GetNew()
        {
            List<Alias> ret = new List<Alias>();
            foreach (DataRow dr in this._dtAliases.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                    ret.Add(new Alias((string)dr["Name"], (string)dr["Value"]));
            }
            return ret;
        }

        public List<Alias> GetDeleted()
        {
            List<Alias> ret = new List<Alias>();
            foreach (DataRow dr in this._dtAliases.Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                {
                    dr.RejectChanges();
                    ret.Add(new Alias((string)dr["Name"], (string)dr["Value"]));
                }
            }
            return ret;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}