using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SqlStudio
{
    public class ConnectToolStripItem : ToolStripMenuItem
    {
        public delegate void OnConnectionClickDelegate(object sender, long key);
        public event OnConnectionClickDelegate OnConnectionClick;
        public delegate void OnEditConnectionDelegate(object sender, long key);
        public event OnEditConnectionDelegate OnEditConnection;
        public delegate void OnDeleteConnectionDelegate(object sender, long key);
        public event OnDeleteConnectionDelegate OnDeleteConnection;

        private long _key = 0;

        public ConnectToolStripItem(string text, long key)
        {
            Text = text;
            _key = key;

            ToolStripMenuItem tsmiConnect = new ToolStripMenuItem("Connect");
            tsmiConnect.Click += new EventHandler(tsmiConnect_Click);
            DropDownItems.Add(tsmiConnect);

            ToolStripItem tsmiEdit = new ToolStripMenuItem("Edit...");
            tsmiEdit.Click += new EventHandler(tsmiEdit_Click);
            DropDownItems.Add(tsmiEdit);

            ToolStripMenuItem tsmiDelete = new ToolStripMenuItem("Delete");
            tsmiDelete.Click += new EventHandler(tsmiDelete_Click);
            DropDownItems.Add(tsmiDelete);
        }

        public long Key
        {
            get { return _key; }
            set { _key = value; }
        }

        void tsmiConnect_Click(object sender, EventArgs e)
        {
            if (OnConnectionClick != null)
                OnConnectionClick(this, _key);
        }

        void tsmiDelete_Click(object sender, EventArgs e)
        {
            if (OnDeleteConnection != null)
            {
                OnDeleteConnection(this, _key);
            }
        }

        void tsmiEdit_Click(object sender, EventArgs e)
        {
            if (OnEditConnection != null)
                OnEditConnection(this, _key);
        }
    }
}
