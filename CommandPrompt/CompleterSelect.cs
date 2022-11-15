using System;
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
                e.Handled = true;
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
            else if (e.KeyCode == Keys.Space)
            {
                SelectedItem = (string)listBoxSelections.SelectedItem + " ";
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

                HighlightFilter();
                e.Handled = true;
            }
        }

        private void HighlightFilter()
        {
          int count = listBoxSelections.Items.Count;
            for (int i = 0; i < count; i++)
            {
                var itemText = (string)listBoxSelections.Items[i];

                if (itemText.IndexOf(_selFilter, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    if (listBoxSelections.SelectedItem != listBoxSelections.Items[i])
                    {
                        listBoxSelections.SelectedItem = listBoxSelections.Items[i];
                    }
                    return;
                }
            }
        }

        public void SetItems(string[] items)
        {
            Array.Sort(items);
            _items = items;
            listBoxSelections.Items.Clear();
            foreach (string s in _items)
            {
                listBoxSelections.Items.Add(s);
            }
            if (items.Length > 0)
                listBoxSelections.SelectedIndex = 0;
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