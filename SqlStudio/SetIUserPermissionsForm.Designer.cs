namespace SqlStudio
{
    partial class SetUserPermissionsForm
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
            btnRun = new System.Windows.Forms.Button();
            btnClose = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            checkedListBoxUsers = new System.Windows.Forms.CheckedListBox();
            SuspendLayout();
            // 
            // btnRun
            // 
            btnRun.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnRun.Location = new System.Drawing.Point(604, 393);
            btnRun.Name = "btnRun";
            btnRun.Size = new System.Drawing.Size(103, 23);
            btnRun.TabIndex = 5;
            btnRun.Text = "Set permissions";
            btnRun.UseVisualStyleBackColor = true;
            btnRun.Click += btnRun_Click;
            // 
            // btnClose
            // 
            btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnClose.Location = new System.Drawing.Point(713, 393);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(75, 23);
            btnClose.TabIndex = 9;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnCancel_Click;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(523, 393);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(75, 23);
            button1.TabIndex = 10;
            button1.Text = "Get users";
            button1.UseVisualStyleBackColor = true;
            button1.Click += GetUsers_Click;
            // 
            // checkedListBoxUsers
            // 
            checkedListBoxUsers.FormattingEnabled = true;
            checkedListBoxUsers.Location = new System.Drawing.Point(12, 9);
            checkedListBoxUsers.Name = "checkedListBoxUsers";
            checkedListBoxUsers.Size = new System.Drawing.Size(776, 364);
            checkedListBoxUsers.TabIndex = 11;
            // 
            // SetUserPermissionsForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 428);
            Controls.Add(checkedListBoxUsers);
            Controls.Add(button1);
            Controls.Add(btnClose);
            Controls.Add(btnRun);
            Name = "SetUserPermissionsForm";
            Text = "GenerateDataForm";
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckedListBox checkedListBoxUsers;
    }
}