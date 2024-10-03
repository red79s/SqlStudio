namespace SqlStudio
{
    partial class NewDBConnectionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            comboBoxProviders = new System.Windows.Forms.ComboBox();
            textBoxServer = new System.Windows.Forms.TextBox();
            textBoxDatabase = new System.Windows.Forms.TextBox();
            textBoxUser = new System.Windows.Forms.TextBox();
            textBoxPassword = new System.Windows.Forms.TextBox();
            buttonCancel = new System.Windows.Forms.Button();
            buttonOK = new System.Windows.Forms.Button();
            checkBoxIntegratedSecurity = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            labelServer = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            textBoxDescription = new System.Windows.Forms.TextBox();
            buttonDBBrowser = new System.Windows.Forms.Button();
            checkBoxDefaultConnection = new System.Windows.Forms.CheckBox();
            checkBoxIsProduction = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // comboBoxProviders
            // 
            comboBoxProviders.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            comboBoxProviders.FormattingEnabled = true;
            comboBoxProviders.Location = new System.Drawing.Point(163, 17);
            comboBoxProviders.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            comboBoxProviders.Name = "comboBoxProviders";
            comboBoxProviders.Size = new System.Drawing.Size(270, 23);
            comboBoxProviders.TabIndex = 0;
            // 
            // textBoxServer
            // 
            textBoxServer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxServer.Location = new System.Drawing.Point(163, 90);
            textBoxServer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxServer.Name = "textBoxServer";
            textBoxServer.Size = new System.Drawing.Size(270, 23);
            textBoxServer.TabIndex = 2;
            // 
            // textBoxDatabase
            // 
            textBoxDatabase.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxDatabase.Location = new System.Drawing.Point(163, 120);
            textBoxDatabase.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxDatabase.Name = "textBoxDatabase";
            textBoxDatabase.Size = new System.Drawing.Size(228, 23);
            textBoxDatabase.TabIndex = 3;
            // 
            // textBoxUser
            // 
            textBoxUser.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxUser.Location = new System.Drawing.Point(163, 150);
            textBoxUser.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxUser.Name = "textBoxUser";
            textBoxUser.Size = new System.Drawing.Size(270, 23);
            textBoxUser.TabIndex = 5;
            // 
            // textBoxPassword
            // 
            textBoxPassword.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxPassword.Location = new System.Drawing.Point(163, 180);
            textBoxPassword.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new System.Drawing.Size(270, 23);
            textBoxPassword.TabIndex = 6;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.Location = new System.Drawing.Point(252, 297);
            buttonCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(88, 27);
            buttonCancel.TabIndex = 8;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // buttonOK
            // 
            buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            buttonOK.Location = new System.Drawing.Point(347, 297);
            buttonOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new System.Drawing.Size(88, 27);
            buttonOK.TabIndex = 7;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // checkBoxIntegratedSecurity
            // 
            checkBoxIntegratedSecurity.AutoSize = true;
            checkBoxIntegratedSecurity.Location = new System.Drawing.Point(163, 210);
            checkBoxIntegratedSecurity.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxIntegratedSecurity.Name = "checkBoxIntegratedSecurity";
            checkBoxIntegratedSecurity.Size = new System.Drawing.Size(146, 19);
            checkBoxIntegratedSecurity.TabIndex = 7;
            checkBoxIntegratedSecurity.Text = "Use integrated security";
            checkBoxIntegratedSecurity.UseVisualStyleBackColor = true;
            checkBoxIntegratedSecurity.CheckedChanged += checkBoxIntegratedSecurity_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(33, 21);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(51, 15);
            label1.TabIndex = 8;
            label1.Text = "Provider";
            // 
            // labelServer
            // 
            labelServer.AutoSize = true;
            labelServer.Location = new System.Drawing.Point(33, 93);
            labelServer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelServer.Name = "labelServer";
            labelServer.Size = new System.Drawing.Size(39, 15);
            labelServer.TabIndex = 9;
            labelServer.Text = "Server";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(33, 123);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(55, 15);
            label3.TabIndex = 10;
            label3.Text = "Database";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(33, 153);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(60, 15);
            label4.TabIndex = 11;
            label4.Text = "Username";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(33, 183);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(57, 15);
            label5.TabIndex = 12;
            label5.Text = "Password";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(33, 63);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(67, 15);
            label6.TabIndex = 13;
            label6.Text = "Description";
            // 
            // textBoxDescription
            // 
            textBoxDescription.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxDescription.Location = new System.Drawing.Point(163, 60);
            textBoxDescription.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.Size = new System.Drawing.Size(270, 23);
            textBoxDescription.TabIndex = 1;
            // 
            // buttonDBBrowser
            // 
            buttonDBBrowser.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonDBBrowser.Location = new System.Drawing.Point(399, 118);
            buttonDBBrowser.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonDBBrowser.Name = "buttonDBBrowser";
            buttonDBBrowser.Size = new System.Drawing.Size(35, 27);
            buttonDBBrowser.TabIndex = 4;
            buttonDBBrowser.Text = "...";
            buttonDBBrowser.UseVisualStyleBackColor = true;
            buttonDBBrowser.Click += buttonDBBrowser_Click;
            // 
            // checkBoxDefaultConnection
            // 
            checkBoxDefaultConnection.AutoSize = true;
            checkBoxDefaultConnection.Location = new System.Drawing.Point(163, 238);
            checkBoxDefaultConnection.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxDefaultConnection.Name = "checkBoxDefaultConnection";
            checkBoxDefaultConnection.Size = new System.Drawing.Size(129, 19);
            checkBoxDefaultConnection.TabIndex = 14;
            checkBoxDefaultConnection.Text = "Default Connection";
            checkBoxDefaultConnection.UseVisualStyleBackColor = true;
            // 
            // checkBoxIsProduction
            // 
            checkBoxIsProduction.AutoSize = true;
            checkBoxIsProduction.Location = new System.Drawing.Point(163, 263);
            checkBoxIsProduction.Name = "checkBoxIsProduction";
            checkBoxIsProduction.Size = new System.Drawing.Size(114, 19);
            checkBoxIsProduction.TabIndex = 15;
            checkBoxIsProduction.Text = "Is production DB";
            checkBoxIsProduction.UseVisualStyleBackColor = true;
            // 
            // NewDBConnectionForm
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(448, 331);
            Controls.Add(checkBoxIsProduction);
            Controls.Add(checkBoxDefaultConnection);
            Controls.Add(buttonDBBrowser);
            Controls.Add(textBoxDescription);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(labelServer);
            Controls.Add(label1);
            Controls.Add(checkBoxIntegratedSecurity);
            Controls.Add(buttonOK);
            Controls.Add(buttonCancel);
            Controls.Add(textBoxPassword);
            Controls.Add(textBoxUser);
            Controls.Add(textBoxDatabase);
            Controls.Add(textBoxServer);
            Controls.Add(comboBoxProviders);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximumSize = new System.Drawing.Size(1164, 400);
            MinimumSize = new System.Drawing.Size(464, 370);
            Name = "NewDBConnectionForm";
            Text = "Database connection";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxProviders;
        private System.Windows.Forms.TextBox textBoxServer;
        private System.Windows.Forms.TextBox textBoxDatabase;
        private System.Windows.Forms.TextBox textBoxUser;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.CheckBox checkBoxIntegratedSecurity;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelServer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Button buttonDBBrowser;
        private System.Windows.Forms.CheckBox checkBoxDefaultConnection;
        private System.Windows.Forms.CheckBox checkBoxIsProduction;
    }
}