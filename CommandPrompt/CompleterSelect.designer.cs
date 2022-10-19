namespace CommandPrompt
{
    partial class CompleterSelect
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
            this.listBoxSelections = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBoxSelections
            // 
            this.listBoxSelections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxSelections.FormattingEnabled = true;
            this.listBoxSelections.Location = new System.Drawing.Point(0, 0);
            this.listBoxSelections.Name = "listBoxSelections";
            this.listBoxSelections.Size = new System.Drawing.Size(208, 303);
            this.listBoxSelections.TabIndex = 0;
            // 
            // CompleterSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(208, 311);
            this.ControlBox = false;
            this.Controls.Add(this.listBoxSelections);
            this.Name = "CompleterSelect";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxSelections;
    }
}