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
            this.Text = text;
            this._key = key;

            ToolStripMenuItem tsmiConnect = new ToolStripMenuItem("Connect");
            tsmiConnect.Click += new EventHandler(tsmiConnect_Click);
            this.DropDownItems.Add(tsmiConnect);

            ToolStripItem tsmiEdit = new ToolStripMenuItem("Edit...");
            tsmiEdit.Click += new EventHandler(tsmiEdit_Click);
            this.DropDownItems.Add(tsmiEdit);

            ToolStripMenuItem tsmiDelete = new ToolStripMenuItem("Delete");
            tsmiDelete.Click += new EventHandler(tsmiDelete_Click);
            this.DropDownItems.Add(tsmiDelete);
        }

        public long Key
        {
            get { return this._key; }
            set { this._key = value; }
        }

        void tsmiConnect_Click(object sender, EventArgs e)
        {
            if (this.OnConnectionClick != null)
                this.OnConnectionClick(this, this._key);
        }

        void tsmiDelete_Click(object sender, EventArgs e)
        {
            if (this.OnDeleteConnection != null)
            {
                this.OnDeleteConnection(this, this._key);
            }
        }

        void tsmiEdit_Click(object sender, EventArgs e)
        {
            if (this.OnEditConnection != null)
                this.OnEditConnection(this, this._key);
        }
    }
}
