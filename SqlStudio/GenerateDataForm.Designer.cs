namespace SqlStudio
{
    partial class GenerateDataForm
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
            sqlScriptComponent = new CommandPrompt.SQLScript();
            label1 = new System.Windows.Forms.Label();
            tbNumberOfInserts = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            tbCommitInterval = new System.Windows.Forms.TextBox();
            btnRun = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            tbBatchNumberOfRows = new System.Windows.Forms.TextBox();
            btnClose = new System.Windows.Forms.Button();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            tbStatus = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            tbStartIndex = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // sqlScriptComponent
            // 
            sqlScriptComponent.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            sqlScriptComponent.Location = new System.Drawing.Point(8, 61);
            sqlScriptComponent.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            sqlScriptComponent.Name = "sqlScriptComponent";
            sqlScriptComponent.Size = new System.Drawing.Size(779, 326);
            sqlScriptComponent.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(195, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(102, 15);
            label1.TabIndex = 1;
            label1.Text = "Number of inserts";
            // 
            // tbNumberOfInserts
            // 
            tbNumberOfInserts.Location = new System.Drawing.Point(303, 6);
            tbNumberOfInserts.Name = "tbNumberOfInserts";
            tbNumberOfInserts.Size = new System.Drawing.Size(100, 23);
            tbNumberOfInserts.TabIndex = 2;
            tbNumberOfInserts.Text = "1000";
            tbNumberOfInserts.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(514, 9);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(82, 15);
            label2.TabIndex = 3;
            label2.Text = "Commit every";
            // 
            // tbCommitInterval
            // 
            tbCommitInterval.Location = new System.Drawing.Point(602, 6);
            tbCommitInterval.Name = "tbCommitInterval";
            tbCommitInterval.Size = new System.Drawing.Size(53, 23);
            tbCommitInterval.TabIndex = 4;
            tbCommitInterval.Text = "1000";
            tbCommitInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnRun
            // 
            btnRun.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnRun.Location = new System.Drawing.Point(632, 393);
            btnRun.Name = "btnRun";
            btnRun.Size = new System.Drawing.Size(75, 23);
            btnRun.TabIndex = 5;
            btnRun.Text = "Run";
            btnRun.UseVisualStyleBackColor = true;
            btnRun.Click += btnRun_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(674, 9);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(65, 15);
            label3.TabIndex = 6;
            label3.Text = "Batch rows";
            // 
            // tbBatchNumberOfRows
            // 
            tbBatchNumberOfRows.Location = new System.Drawing.Point(745, 6);
            tbBatchNumberOfRows.Name = "tbBatchNumberOfRows";
            tbBatchNumberOfRows.Size = new System.Drawing.Size(42, 23);
            tbBatchNumberOfRows.TabIndex = 7;
            tbBatchNumberOfRows.Text = "100";
            tbBatchNumberOfRows.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            label4.Location = new System.Drawing.Point(12, 43);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(526, 15);
            label4.TabIndex = 10;
            label4.Text = "rowNo is current row. Example Insert into foo values({rowNo}, '{DateTime.Now.AddDays(rowNo)}')";
            // 
            // label5
            // 
            label5.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(8, 396);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(39, 15);
            label5.TabIndex = 11;
            label5.Text = "Status";
            // 
            // tbStatus
            // 
            tbStatus.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbStatus.Location = new System.Drawing.Point(53, 393);
            tbStatus.Name = "tbStatus";
            tbStatus.Size = new System.Drawing.Size(559, 23);
            tbStatus.TabIndex = 12;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(12, 9);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(59, 15);
            label6.TabIndex = 13;
            label6.Text = "Star index";
            // 
            // tbStartIndex
            // 
            tbStartIndex.Location = new System.Drawing.Point(77, 6);
            tbStartIndex.Name = "tbStartIndex";
            tbStartIndex.Size = new System.Drawing.Size(100, 23);
            tbStartIndex.TabIndex = 14;
            tbStartIndex.Text = "0";
            tbStartIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // GenerateDataForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 428);
            Controls.Add(tbStartIndex);
            Controls.Add(label6);
            Controls.Add(tbStatus);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(btnClose);
            Controls.Add(tbBatchNumberOfRows);
            Controls.Add(label3);
            Controls.Add(btnRun);
            Controls.Add(tbCommitInterval);
            Controls.Add(label2);
            Controls.Add(tbNumberOfInserts);
            Controls.Add(label1);
            Controls.Add(sqlScriptComponent);
            Name = "GenerateDataForm";
            Text = "GenerateDataForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CommandPrompt.SQLScript sqlScriptComponent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbNumberOfInserts;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbCommitInterval;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbBatchNumberOfRows;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbStatus;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbStartIndex;
    }
}