using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using SqlExecute;
using CfgDataStore;

namespace SqlStudio
{
    public partial class NewDBConnectionForm : Form
    {
        private Connection _conRow = null;

        public NewDBConnectionForm()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            InitializeComponent();
            this.comboBoxProviders.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxProviders.Items.Clear();
            this.comboBoxProviders.BeginUpdate();
            foreach (SqlExecuter.DatabaseProvider provider in Enum.GetValues(typeof(SqlExecuter.DatabaseProvider)))
            {
                this.comboBoxProviders.Items.Add(SqlExecuter.GetProviderName(provider));
            }
            this.comboBoxProviders.SelectedIndex = 0;
            this.comboBoxProviders.EndUpdate();
            this.comboBoxProviders.SelectedValueChanged += new EventHandler(comboBoxProviders_SelectedValueChanged);
        }

        void comboBoxProviders_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((string)this.comboBoxProviders.SelectedItem == SqlExecuter.GetProviderName(SqlExecuter.DatabaseProvider.SQLITE) ||
                (string)this.comboBoxProviders.SelectedItem == SqlExecuter.GetProviderName(SqlExecuter.DatabaseProvider.SQLSERVERCE))
            {
                this.textBoxServer.Enabled = false;
                this.textBoxDatabase.Enabled = true;
                this.textBoxUser.Enabled = false;
                this.textBoxPassword.Enabled = false;
                this.checkBoxIntegratedSecurity.Enabled = false;
                this.buttonDBBrowser.Enabled = true;
            }
            else if ((string)this.comboBoxProviders.SelectedItem == SqlExecuter.GetProviderName(SqlExecuter.DatabaseProvider.ODBC))
            {
                this.textBoxServer.Enabled = true;
                this.labelServer.Text = "Con String";
                this.textBoxDatabase.Enabled = false;
                this.textBoxUser.Enabled = false;
                this.textBoxPassword.Enabled = false;
                this.checkBoxIntegratedSecurity.Enabled = false;
                this.buttonDBBrowser.Enabled = false;
            }
            else
            {
                this.textBoxServer.Enabled = true;
                this.textBoxDatabase.Enabled = true;
                this.textBoxUser.Enabled = true;
                this.textBoxPassword.Enabled = true;
                this.checkBoxIntegratedSecurity.Enabled = true;
                this.buttonDBBrowser.Enabled = false;
            }
        }

        public Connection ConnectionRow
        {
            get { return this._conRow; }
            set 
            { 
                this._conRow = value;
                this.Initialize();
            }
        }

        private void Initialize()
        {
            if (this._conRow.provider != "")
            {
                this.comboBoxProviders.SelectedIndex = this.comboBoxProviders.FindStringExact(this._conRow.provider);
            }
            else
                this.comboBoxProviders.SelectedIndex = 0;

            this.textBoxDescription.Text = this._conRow.description;
            this.textBoxServer.Text = this._conRow.server;
            this.textBoxDatabase.Text = this._conRow.db;
            this.textBoxUser.Text = this._conRow.user;
            this.textBoxPassword.Text = this._conRow.password;
            this.checkBoxIntegratedSecurity.Checked = this._conRow.integrated_security;
            this.checkBoxDefaultConnection.Checked = this._conRow.default_connection;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this._conRow.provider = (string)this.comboBoxProviders.SelectedItem;
            this._conRow.description = this.textBoxDescription.Text;
            this._conRow.server = this.textBoxServer.Text;
            this._conRow.db = this.textBoxDatabase.Text;
            this._conRow.user = this.textBoxUser.Text;
            this._conRow.password = this.textBoxPassword.Text;
            this._conRow.integrated_security = this.checkBoxIntegratedSecurity.Checked;
            this._conRow.default_connection = this.checkBoxDefaultConnection.Checked;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void checkBoxIntegratedSecurity_CheckedChanged(object sender, EventArgs e)
        {
            this.textBoxUser.Enabled = !this.checkBoxIntegratedSecurity.Checked;
            this.textBoxPassword.Enabled = !this.checkBoxIntegratedSecurity.Checked;
        }

        private void buttonDBBrowser_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = this.textBoxDatabase.Text;
            if (ofd.ShowDialog() == DialogResult.OK)
                this.textBoxDatabase.Text = ofd.FileName;
        }
    }
}