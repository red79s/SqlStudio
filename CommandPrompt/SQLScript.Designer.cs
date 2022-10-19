namespace CommandPrompt
{
    partial class SQLScript
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
            this.formatTextControl = new FormatTextControl.FormatTextControl();
            this.SuspendLayout();
            // 
            // formatTextControl
            // 
            this.formatTextControl.AcceptsKeyInput = true;
            this.formatTextControl.AutoScroll = true;
            this.formatTextControl.AutoScrollMinSize = new System.Drawing.Size(38, 29);
            this.formatTextControl.BackColor = System.Drawing.Color.White;
            this.formatTextControl.BackColorIconPane = System.Drawing.Color.Yellow;
            this.formatTextControl.BackColorLineNum = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.formatTextControl.BackColorSelection = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(150)))), ((int)(((byte)(223)))));
            this.formatTextControl.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.formatTextControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formatTextControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.formatTextControl.ForeColor = System.Drawing.Color.Black;
            this.formatTextControl.ForeColorLineNum = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.formatTextControl.Location = new System.Drawing.Point(0, 0);
            this.formatTextControl.Name = "formatTextControl";
            this.formatTextControl.ShowIconPane = false;
            this.formatTextControl.ShowLineNumbers = true;
            this.formatTextControl.Size = new System.Drawing.Size(490, 637);
            this.formatTextControl.TabIndex = 0;
            // 
            // SQLScript
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.formatTextControl);
            this.Name = "SQLScript";
            this.Size = new System.Drawing.Size(490, 637);
            this.ResumeLayout(false);

        }

        #endregion

        private FormatTextControl.FormatTextControl formatTextControl;
    }
}
