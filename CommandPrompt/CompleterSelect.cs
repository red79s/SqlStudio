using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CommandPrompt
{
    public partial class CompleterSelect : Form
    {
        private string[] _items = null;
        private string _selectedItem = string.Empty;
        private string _selFilter = string.Empty;

        public CompleterSelect()
        {
            InitializeComponent();

            this.listBoxSelections.KeyDown += new KeyEventHandler(listBoxSelections_KeyDown);
            this.listBoxSelections.KeyPress += new KeyPressEventHandler(listBoxSelections_KeyPress);
            this.listBoxSelections.MouseDoubleClick += new MouseEventHandler(listBoxSelections_MouseDoubleClick);
            ContextMenu cm = new ContextMenu();
            MenuItem miReset = new MenuItem("Reset");
            miReset.Click += new EventHandler(miReset_Click);
            cm.MenuItems.Add(miReset);
            this.listBoxSelections.ContextMenu = cm;
        }

        void listBoxSelections_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar))
            {
                this._selFilter += e.KeyChar;
                this._selFilter = this._selFilter.ToLower();
                this.HighlightFilter();
            }
        }

        void miReset_Click(object sender, EventArgs e)
        {
            this._selFilter = string.Empty;
            if (this.listBoxSelections.Items.Count > 0)
            {
                this.listBoxSelections.SelectedItem = this.listBoxSelections.Items[0];
            }
        }

        void listBoxSelections_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.SelectedItem = (string)this.listBoxSelections.SelectedItem;
                this.CloseOK();
            }
        }

        void listBoxSelections_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SelectedItem = (string)this.listBoxSelections.SelectedItem;
                this.CloseOK();
            }
            else if (e.KeyCode == Keys.Space)
            {
                this.SelectedItem = (string)this.listBoxSelections.SelectedItem + " ";
                this.CloseOK();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.CloseCancel();
            }
            else if (e.KeyCode == Keys.Back)
            {
                if (this._selFilter.Length > 0)
                    this._selFilter = this._selFilter.Substring(0, this._selFilter.Length - 1);
            }
        }

        private void HighlightFilter()
        {
            int count = this.listBoxSelections.Items.Count;
            for (int i = 0; i < count; i++)
            {
                if (((string)this.listBoxSelections.Items[i]).ToLower().IndexOf(this._selFilter) >= 0)
                {
                    this.listBoxSelections.SelectedItem = this.listBoxSelections.Items[i];
                }
            }
        }

        public void SetItems(string[] items)
        {
            Array.Sort(items);
            this._items = items;
            this.listBoxSelections.Items.Clear();
            foreach (string s in this._items)
            {
                this.listBoxSelections.Items.Add(s);
            }
            if (items.Length > 0)
                this.listBoxSelections.SelectedIndex = 0;
        }

        public string SelectedItem
        {
            get { return this._selectedItem; }
            set { this._selectedItem = value; }
        }

        private void CloseOK()
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CloseCancel()
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}