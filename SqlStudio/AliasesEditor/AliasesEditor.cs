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
        public string Name;
        public string Value;
        public Alias(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    public partial class AliasesEditor : Form
    {
        private DataTable _dtAliases = null;

        public AliasesEditor()
        {
            InitializeComponent();
            _dtAliases = new DataTable();
            _dtAliases.Columns.Add("Name", typeof(string));
            _dtAliases.Columns.Add("Value", typeof(string));
            InitData();
        }

        private void InitData()
        {
            dataGridView.DataSource = _dtAliases;
            dataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView.RowHeadersWidth = 15;
        }

        public void AddAlias(Alias alias)
        {
            DataRow dr = _dtAliases.NewRow();
            dr["Name"] = alias.Name;
            dr["Value"] = alias.Value;
            _dtAliases.Rows.Add(dr);
            _dtAliases.AcceptChanges();
            InitData();
        }

        public List<Alias> GetModified()
        {
            List<Alias> ret = new List<Alias>();
            foreach (DataRow dr in _dtAliases.Rows)
            {
                if (dr.RowState == DataRowState.Modified)
                    ret.Add(new Alias((string)dr["Name"], (string)dr["Value"]));
            }
            return ret;
        }

        public List<Alias> GetNew()
        {
            List<Alias> ret = new List<Alias>();
            foreach (DataRow dr in _dtAliases.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                    ret.Add(new Alias((string)dr["Name"], (string)dr["Value"]));
            }
            return ret;
        }

        public List<Alias> GetDeleted()
        {
            List<Alias> ret = new List<Alias>();
            foreach (DataRow dr in _dtAliases.Rows)
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
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}