namespace SqlStudio
{
    partial class TextOutputDialog
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
            textBoxContent = new System.Windows.Forms.TextBox();
            btnClose = new System.Windows.Forms.Button();
            btnFormat = new System.Windows.Forms.Button();
            findButton = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // textBoxContent
            // 
            textBoxContent.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxContent.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            textBoxContent.Location = new System.Drawing.Point(0, 1);
            textBoxContent.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBoxContent.MaxLength = 2000000;
            textBoxContent.Multiline = true;
            textBoxContent.Name = "textBoxContent";
            textBoxContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            textBoxContent.Size = new System.Drawing.Size(1143, 486);
            textBoxContent.TabIndex = 0;
            // 
            // btnClose
            // 
            btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnClose.Location = new System.Drawing.Point(1056, 493);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(75, 23);
            btnClose.TabIndex = 1;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += CloseButton_Click;
            // 
            // btnFormat
            // 
            btnFormat.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnFormat.Location = new System.Drawing.Point(975, 493);
            btnFormat.Name = "btnFormat";
            btnFormat.Size = new System.Drawing.Size(75, 23);
            btnFormat.TabIndex = 2;
            btnFormat.Text = "Format";
            btnFormat.UseVisualStyleBackColor = true;
            btnFormat.Click += FormatButton_Click;
            // 
            // findButton
            // 
            findButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            findButton.Location = new System.Drawing.Point(894, 493);
            findButton.Name = "findButton";
            findButton.Size = new System.Drawing.Size(75, 23);
            findButton.TabIndex = 3;
            findButton.Text = "Find...";
            findButton.UseVisualStyleBackColor = true;
            findButton.Click += findButton_Click;
            // 
            // TextOutputDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1143, 525);
            Controls.Add(findButton);
            Controls.Add(btnFormat);
            Controls.Add(btnClose);
            Controls.Add(textBoxContent);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "TextOutputDialog";
            Text = "TextOutputDialog";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox textBoxContent;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnFormat;
        private System.Windows.Forms.Button findButton;
    }
}