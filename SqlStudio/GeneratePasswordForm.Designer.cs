namespace SqlStudio
{
    partial class GeneratePasswordForm
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
            label1 = new System.Windows.Forms.Label();
            tbPassword = new System.Windows.Forms.TextBox();
            btnGenerateHash = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            tbHash = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(15, 12);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(57, 15);
            label1.TabIndex = 0;
            label1.Text = "Password";
            // 
            // tbPassword
            // 
            tbPassword.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbPassword.Location = new System.Drawing.Point(90, 9);
            tbPassword.Name = "tbPassword";
            tbPassword.Size = new System.Drawing.Size(561, 23);
            tbPassword.TabIndex = 1;
            // 
            // btnGenerateHash
            // 
            btnGenerateHash.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnGenerateHash.Location = new System.Drawing.Point(657, 9);
            btnGenerateHash.Name = "btnGenerateHash";
            btnGenerateHash.Size = new System.Drawing.Size(105, 23);
            btnGenerateHash.TabIndex = 2;
            btnGenerateHash.Text = "Generate hash";
            btnGenerateHash.UseVisualStyleBackColor = true;
            btnGenerateHash.Click += GeneratePasswordHash_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(15, 48);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(34, 15);
            label2.TabIndex = 3;
            label2.Text = "Hash";
            // 
            // tbHash
            // 
            tbHash.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbHash.Location = new System.Drawing.Point(90, 45);
            tbHash.Name = "tbHash";
            tbHash.Size = new System.Drawing.Size(561, 23);
            tbHash.TabIndex = 4;
            // 
            // GeneratePasswordForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(774, 96);
            Controls.Add(tbHash);
            Controls.Add(label2);
            Controls.Add(btnGenerateHash);
            Controls.Add(tbPassword);
            Controls.Add(label1);
            Name = "GeneratePasswordForm";
            Text = "Generate Password Hash";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Button btnGenerateHash;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbHash;
    }
}