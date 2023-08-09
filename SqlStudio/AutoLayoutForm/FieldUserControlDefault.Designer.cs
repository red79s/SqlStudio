namespace SqlStudio.AutoLayoutForm
{
    partial class FieldUserControlDefault
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            FieldLabel = new System.Windows.Forms.Label();
            tbValue = new System.Windows.Forms.TextBox();
            cbDbNull = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // FieldLabel
            // 
            FieldLabel.Location = new System.Drawing.Point(0, 5);
            FieldLabel.Name = "FieldLabel";
            FieldLabel.Size = new System.Drawing.Size(100, 15);
            FieldLabel.TabIndex = 0;
            FieldLabel.Text = "Name";
            // 
            // tbValue
            // 
            tbValue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbValue.Location = new System.Drawing.Point(103, 2);
            tbValue.Name = "tbValue";
            tbValue.Size = new System.Drawing.Size(150, 23);
            tbValue.TabIndex = 1;
            // 
            // cbDbNull
            // 
            cbDbNull.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            cbDbNull.Location = new System.Drawing.Point(259, 4);
            cbDbNull.Name = "cbDbNull";
            cbDbNull.Size = new System.Drawing.Size(50, 19);
            cbDbNull.TabIndex = 2;
            cbDbNull.Text = "Null";
            cbDbNull.UseVisualStyleBackColor = true;
            // 
            // FieldUserControlDefault
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(cbDbNull);
            Controls.Add(tbValue);
            Controls.Add(FieldLabel);
            Name = "FieldUserControlDefault";
            Size = new System.Drawing.Size(312, 31);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label FieldLabel;
        private System.Windows.Forms.TextBox tbValue;
        private System.Windows.Forms.CheckBox cbDbNull;
    }
}
