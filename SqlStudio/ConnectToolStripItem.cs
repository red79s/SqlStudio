using CfgDataStore;
using System;
using System.Windows.Forms;

namespace SqlStudio
{
    public class ConnectToolStripItem : ToolStripMenuItem
    {
        public delegate void OnConnectionClickDelegate(object sender, Connection connection);
        public event OnConnectionClickDelegate OnConnectionClick;
        public delegate void OnEditConnectionDelegate(object sender, Connection connection);
        public event OnEditConnectionDelegate OnEditConnection;
        public delegate void OnDeleteConnectionDelegate(object sender, Connection connection);
        public event OnDeleteConnectionDelegate OnDeleteConnection;

        public Connection Connection { get; set; }

        public ConnectToolStripItem(Connection connection)
        {
            Connection = connection;

            Text = connection.description;

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

        void tsmiConnect_Click(object sender, EventArgs e)
        {
            if (OnConnectionClick != null)
                OnConnectionClick(this, Connection);
        }

        void tsmiDelete_Click(object sender, EventArgs e)
        {
            if (OnDeleteConnection != null)
            {
                OnDeleteConnection(this, Connection);
            }
        }

        void tsmiEdit_Click(object sender, EventArgs e)
        {
            if (OnEditConnection != null)
                OnEditConnection(this, Connection);
        }
    }
}
