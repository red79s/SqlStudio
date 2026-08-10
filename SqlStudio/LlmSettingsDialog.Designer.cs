namespace SqlStudio
{
    partial class LlmSettingsDialog
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
            this.labelProvider = new System.Windows.Forms.Label();
            this.comboBoxProvider = new System.Windows.Forms.ComboBox();
            this.labelApiKey = new System.Windows.Forms.Label();
            this.textBoxApiKey = new System.Windows.Forms.TextBox();
            this.checkBoxShowKey = new System.Windows.Forms.CheckBox();
            this.labelModel = new System.Windows.Forms.Label();
            this.comboBoxModel = new System.Windows.Forms.ComboBox();
            this.labelEndpoint = new System.Windows.Forms.Label();
            this.textBoxEndpoint = new System.Windows.Forms.TextBox();
            this.labelInfo = new System.Windows.Forms.Label();
            this.buttonTest = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // labelProvider
            //
            this.labelProvider.AutoSize = true;
            this.labelProvider.Location = new System.Drawing.Point(12, 15);
            this.labelProvider.Name = "labelProvider";
            this.labelProvider.Size = new System.Drawing.Size(49, 13);
            this.labelProvider.TabIndex = 0;
            this.labelProvider.Text = "Provider:";
            //
            // comboBoxProvider
            //
            this.comboBoxProvider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBoxProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxProvider.Location = new System.Drawing.Point(100, 12);
            this.comboBoxProvider.Name = "comboBoxProvider";
            this.comboBoxProvider.Size = new System.Drawing.Size(200, 21);
            this.comboBoxProvider.TabIndex = 1;
            this.comboBoxProvider.SelectedIndexChanged += new System.EventHandler(this.comboBoxProvider_SelectedIndexChanged);
            //
            // labelApiKey
            //
            this.labelApiKey.AutoSize = true;
            this.labelApiKey.Location = new System.Drawing.Point(12, 44);
            this.labelApiKey.Name = "labelApiKey";
            this.labelApiKey.Size = new System.Drawing.Size(47, 13);
            this.labelApiKey.TabIndex = 2;
            this.labelApiKey.Text = "API key:";
            //
            // textBoxApiKey
            //
            this.textBoxApiKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxApiKey.Location = new System.Drawing.Point(100, 41);
            this.textBoxApiKey.Name = "textBoxApiKey";
            this.textBoxApiKey.Size = new System.Drawing.Size(330, 20);
            this.textBoxApiKey.TabIndex = 3;
            this.textBoxApiKey.UseSystemPasswordChar = true;
            //
            // checkBoxShowKey
            //
            this.checkBoxShowKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxShowKey.AutoSize = true;
            this.checkBoxShowKey.Location = new System.Drawing.Point(100, 67);
            this.checkBoxShowKey.Name = "checkBoxShowKey";
            this.checkBoxShowKey.Size = new System.Drawing.Size(74, 17);
            this.checkBoxShowKey.TabIndex = 4;
            this.checkBoxShowKey.Text = "Show key";
            this.checkBoxShowKey.UseVisualStyleBackColor = true;
            this.checkBoxShowKey.CheckedChanged += new System.EventHandler(this.checkBoxShowKey_CheckedChanged);
            //
            // labelModel
            //
            this.labelModel.AutoSize = true;
            this.labelModel.Location = new System.Drawing.Point(12, 96);
            this.labelModel.Name = "labelModel";
            this.labelModel.Size = new System.Drawing.Size(39, 13);
            this.labelModel.TabIndex = 5;
            this.labelModel.Text = "Model:";
            //
            // comboBoxModel
            //
            this.comboBoxModel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxModel.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxModel.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.comboBoxModel.FormattingEnabled = true;
            this.comboBoxModel.Location = new System.Drawing.Point(100, 93);
            this.comboBoxModel.Name = "comboBoxModel";
            this.comboBoxModel.Size = new System.Drawing.Size(330, 21);
            this.comboBoxModel.TabIndex = 6;
            //
            // labelEndpoint
            //
            this.labelEndpoint.AutoSize = true;
            this.labelEndpoint.Location = new System.Drawing.Point(12, 122);
            this.labelEndpoint.Name = "labelEndpoint";
            this.labelEndpoint.Size = new System.Drawing.Size(52, 13);
            this.labelEndpoint.TabIndex = 7;
            this.labelEndpoint.Text = "Endpoint:";
            //
            // textBoxEndpoint
            //
            this.textBoxEndpoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxEndpoint.Location = new System.Drawing.Point(100, 119);
            this.textBoxEndpoint.Name = "textBoxEndpoint";
            this.textBoxEndpoint.Size = new System.Drawing.Size(330, 20);
            this.textBoxEndpoint.TabIndex = 8;
            //
            // labelInfo
            //
            this.labelInfo.AutoSize = true;
            this.labelInfo.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelInfo.Location = new System.Drawing.Point(12, 150);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(255, 13);
            this.labelInfo.TabIndex = 9;
            this.labelInfo.Text = "Changes apply to new connections / after restart.";
            //
            // buttonTest
            //
            this.buttonTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonTest.Location = new System.Drawing.Point(12, 177);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(110, 23);
            this.buttonTest.TabIndex = 10;
            this.buttonTest.Text = "Test connection";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            //
            // buttonOK
            //
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(274, 177);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 11;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            //
            // buttonCancel
            //
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(355, 177);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 12;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            //
            // LlmSettingsDialog
            //
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(442, 212);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonTest);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.textBoxEndpoint);
            this.Controls.Add(this.labelEndpoint);
            this.Controls.Add(this.comboBoxModel);
            this.Controls.Add(this.labelModel);
            this.Controls.Add(this.checkBoxShowKey);
            this.Controls.Add(this.textBoxApiKey);
            this.Controls.Add(this.labelApiKey);
            this.Controls.Add(this.comboBoxProvider);
            this.Controls.Add(this.labelProvider);
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.Name = "LlmSettingsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "LLM Settings";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label labelProvider;
        private System.Windows.Forms.ComboBox comboBoxProvider;
        private System.Windows.Forms.Label labelApiKey;
        private System.Windows.Forms.TextBox textBoxApiKey;
        private System.Windows.Forms.CheckBox checkBoxShowKey;
        private System.Windows.Forms.Label labelModel;
        private System.Windows.Forms.ComboBox comboBoxModel;
        private System.Windows.Forms.Label labelEndpoint;
        private System.Windows.Forms.TextBox textBoxEndpoint;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}
