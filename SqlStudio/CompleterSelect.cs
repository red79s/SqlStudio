using System;
using System.Windows.Forms;

namespace SqlStudio
{
    public partial class CompleterSelect : Form
    {
        private string[] _items = null;
        private string _selectedItem = string.Empty;
        private string _selFilter = string.Empty;

        public CompleterSelect()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;

            listBoxSelections.KeyDown += new KeyEventHandler(listBoxSelections_KeyDown);
            listBoxSelections.KeyPress += new KeyPressEventHandler(listBoxSelections_KeyPress);
            listBoxSelections.MouseDoubleClick += new MouseEventHandler(listBoxSelections_MouseDoubleClick);
            ContextMenu cm = new ContextMenu();
            MenuItem miReset = new MenuItem("Reset");
            miReset.Click += new EventHandler(miReset_Click);
            cm.MenuItems.Add(miReset);
            listBoxSelections.ContextMenu = cm;
        }

        void listBoxSelections_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar))
            {
                _selFilter += e.KeyChar;
                _selFilter = _selFilter.ToLower();
                HighlightFilter();
            }
        }

        void miReset_Click(object sender, EventArgs e)
        {
            _selFilter = string.Empty;
            if (listBoxSelections.Items.Count > 0)
            {
                listBoxSelections.SelectedItem = listBoxSelections.Items[0];
            }
        }

        void listBoxSelections_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SelectedItem = (string)listBoxSelections.SelectedItem;
                CloseOK();
            }
        }

        void listBoxSelections_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SelectedItem = (string)listBoxSelections.SelectedItem;
                CloseOK();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                CloseCancel();
            }
            else if (e.KeyCode == Keys.Back)
            {
                if (_selFilter.Length > 0)
                    _selFilter = _selFilter.Substring(0, _selFilter.Length - 1);
            }
        }

        private void HighlightFilter()
        {
            int count = listBoxSelections.Items.Count;
            for (int i = 0; i < count; i++)
            {
                if (((string)listBoxSelections.Items[i]).ToLower().IndexOf(_selFilter) >= 0)
                {
                    listBoxSelections.SelectedItem = listBoxSelections.Items[i];
                }
            }
        }

        public void SetItems(string[] items)
        {
            _items = items;
            listBoxSelections.Items.Clear();
            foreach (string s in _items)
            {
                listBoxSelections.Items.Add(s);
            }
        }

        public string SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; }
        }

        private void CloseOK()
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CloseCancel()
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}