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
            StartPosition = FormStartPosition.CenterParent;
            InitializeComponent();
            comboBoxProviders.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxProviders.Items.Clear();
            comboBoxProviders.BeginUpdate();
            foreach (SqlExecuter.DatabaseProvider provider in Enum.GetValues(typeof(SqlExecuter.DatabaseProvider)))
            {
                comboBoxProviders.Items.Add(SqlExecuter.GetProviderName(provider));
            }
            comboBoxProviders.SelectedIndex = 0;
            comboBoxProviders.EndUpdate();
            comboBoxProviders.SelectedValueChanged += new EventHandler(comboBoxProviders_SelectedValueChanged);
        }

        void comboBoxProviders_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((string)comboBoxProviders.SelectedItem == SqlExecuter.GetProviderName(SqlExecuter.DatabaseProvider.SQLITE) ||
                (string)comboBoxProviders.SelectedItem == SqlExecuter.GetProviderName(SqlExecuter.DatabaseProvider.SQLSERVERCE))
            {
                textBoxServer.Enabled = false;
                textBoxDatabase.Enabled = true;
                textBoxUser.Enabled = false;
                textBoxPassword.Enabled = false;
                checkBoxIntegratedSecurity.Enabled = false;
                buttonDBBrowser.Enabled = true;
            }
            else if ((string)comboBoxProviders.SelectedItem == SqlExecuter.GetProviderName(SqlExecuter.DatabaseProvider.ODBC))
            {
                textBoxServer.Enabled = true;
                labelServer.Text = "Con String";
                textBoxDatabase.Enabled = false;
                textBoxUser.Enabled = false;
                textBoxPassword.Enabled = false;
                checkBoxIntegratedSecurity.Enabled = false;
                buttonDBBrowser.Enabled = false;
            }
            else
            {
                textBoxServer.Enabled = true;
                textBoxDatabase.Enabled = true;
                textBoxUser.Enabled = true;
                textBoxPassword.Enabled = true;
                checkBoxIntegratedSecurity.Enabled = true;
                buttonDBBrowser.Enabled = false;
            }
        }

        public Connection ConnectionRow
        {
            get { return _conRow; }
            set 
            { 
                _conRow = value;
                Initialize();
            }
        }

        private void Initialize()
        {
            if (_conRow.provider != "")
            {
                comboBoxProviders.SelectedIndex = comboBoxProviders.FindStringExact(_conRow.provider);
            }
            else
                comboBoxProviders.SelectedIndex = 0;

            textBoxDescription.Text = _conRow.description;
            textBoxServer.Text = _conRow.server;
            textBoxDatabase.Text = _conRow.db;
            textBoxUser.Text = _conRow.user;
            textBoxPassword.Text = _conRow.password;
            checkBoxIntegratedSecurity.Checked = _conRow.integrated_security;
            checkBoxDefaultConnection.Checked = _conRow.default_connection;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            _conRow.provider = (string)comboBoxProviders.SelectedItem;
            _conRow.description = textBoxDescription.Text;
            _conRow.server = textBoxServer.Text;
            _conRow.db = textBoxDatabase.Text;
            _conRow.user = textBoxUser.Text;
            _conRow.password = textBoxPassword.Text;
            _conRow.integrated_security = checkBoxIntegratedSecurity.Checked;
            _conRow.default_connection = checkBoxDefaultConnection.Checked;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void checkBoxIntegratedSecurity_CheckedChanged(object sender, EventArgs e)
        {
            textBoxUser.Enabled = !checkBoxIntegratedSecurity.Checked;
            textBoxPassword.Enabled = !checkBoxIntegratedSecurity.Checked;
        }

        private void buttonDBBrowser_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = textBoxDatabase.Text;
            if (ofd.ShowDialog() == DialogResult.OK)
                textBoxDatabase.Text = ofd.FileName;
        }
    }
}