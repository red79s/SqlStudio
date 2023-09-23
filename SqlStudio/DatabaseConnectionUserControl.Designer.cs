using SqlStudio.Properties;
using System.Resources;
using System.Windows.Forms;

namespace SqlStudio
{
    partial class DatabaseConnectionUserControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            components = new System.ComponentModel.Container();
            MainViewSplitContainer = new System.Windows.Forms.SplitContainer();
            tabControlMainDocs = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            cmdLineControl = new CommandPrompt.CmdLineControl();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripMessageLabel = new System.Windows.Forms.ToolStripStatusLabel();
            visibleRowsToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            toolStripStatusLabelMetaData = new System.Windows.Forms.ToolStripStatusLabel();
            sqlOutput = new SqlOutputTabContainer();
            tabPage4 = new System.Windows.Forms.TabPage();
            tabPage5 = new System.Windows.Forms.TabPage();
            toolStripScripts = new System.Windows.Forms.ToolStrip();
            toolStripButtonNewScript = new System.Windows.Forms.ToolStripButton();
            toolStripButtonOpenScript = new System.Windows.Forms.ToolStripButton();
            toolStripButtoSaveScript = new System.Windows.Forms.ToolStripButton();
            toolStripButtonRunScript = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripDatabaseConnectionsDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)MainViewSplitContainer).BeginInit();
            MainViewSplitContainer.Panel1.SuspendLayout();
            MainViewSplitContainer.Panel2.SuspendLayout();
            MainViewSplitContainer.SuspendLayout();
            tabControlMainDocs.SuspendLayout();
            tabPage1.SuspendLayout();
            statusStrip1.SuspendLayout();
            sqlOutput.SuspendLayout();
            SuspendLayout();
            // 
            // MainViewSplitContainer
            // 
            MainViewSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            MainViewSplitContainer.Location = new System.Drawing.Point(0, 0);
            MainViewSplitContainer.Name = "MainViewSplitContainer";
            MainViewSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // MainViewSplitContainer.Panel1
            // 
            MainViewSplitContainer.Panel1.Controls.Add(toolStripScripts);
            MainViewSplitContainer.Panel1.Controls.Add(tabControlMainDocs);
            // 
            // MainViewSplitContainer.Panel2
            // 
            MainViewSplitContainer.Panel2.Controls.Add(statusStrip1);
            MainViewSplitContainer.Panel2.Controls.Add(sqlOutput);
            MainViewSplitContainer.Size = new System.Drawing.Size(954, 676);
            MainViewSplitContainer.SplitterDistance = 411;
            MainViewSplitContainer.TabIndex = 0;
            // 
            // tabControlMainDocs
            // 
            tabControlMainDocs.Controls.Add(tabPage1);
            tabControlMainDocs.Location = new System.Drawing.Point(0, 31);
            tabControlMainDocs.Name = "tabControlMainDocs";
            tabControlMainDocs.SelectedIndex = 0;
            tabControlMainDocs.Size = new System.Drawing.Size(954, 380);
            tabControlMainDocs.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(cmdLineControl);
            tabPage1.Location = new System.Drawing.Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(946, 352);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // cmdLineControl
            // 
            cmdLineControl.AcceptsKeyInput = false;
            cmdLineControl.AutoScroll = true;
            cmdLineControl.AutoScrollMinSize = new System.Drawing.Size(54, 26);
            cmdLineControl.BackColor = System.Drawing.Color.White;
            cmdLineControl.BackColorIconPane = System.Drawing.Color.Yellow;
            cmdLineControl.BackColorLineNum = System.Drawing.Color.FromArgb(224, 224, 224);
            cmdLineControl.BackColorSelection = System.Drawing.Color.FromArgb(122, 150, 223);
            cmdLineControl.Dock = System.Windows.Forms.DockStyle.Fill;
            cmdLineControl.ForeColor = System.Drawing.Color.Black;
            cmdLineControl.ForeColorLineNum = System.Drawing.Color.FromArgb(64, 64, 64);
            cmdLineControl.Location = new System.Drawing.Point(3, 3);
            cmdLineControl.Name = "cmdLineControl";
            cmdLineControl.ShowIconPane = true;
            cmdLineControl.ShowLineNumbers = true;
            cmdLineControl.Size = new System.Drawing.Size(940, 346);
            cmdLineControl.TabIndex = 0;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripMessageLabel, visibleRowsToolStripStatusLabel, toolStripProgressBar, toolStripStatusLabelMetaData });
            statusStrip1.Location = new System.Drawing.Point(0, 239);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(954, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripMessageLabel
            // 
            toolStripMessageLabel.AutoSize = false;
            toolStripMessageLabel.Name = "toolStripMessageLabel";
            toolStripMessageLabel.Size = new System.Drawing.Size(707, 17);
            toolStripMessageLabel.Text = "Disconnected";
            toolStripMessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // visibleRowsToolStripStatusLabel
            // 
            visibleRowsToolStripStatusLabel.Name = "visibleRowsToolStripStatusLabel";
            visibleRowsToolStripStatusLabel.Size = new System.Drawing.Size(13, 17);
            visibleRowsToolStripStatusLabel.Text = "0";
            // 
            // toolStripProgressBar
            // 
            toolStripProgressBar.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            toolStripProgressBar.Name = "toolStripProgressBar";
            toolStripProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripStatusLabelMetaData
            // 
            toolStripStatusLabelMetaData.Name = "toolStripStatusLabelMetaData";
            toolStripStatusLabelMetaData.Size = new System.Drawing.Size(57, 17);
            toolStripStatusLabelMetaData.Text = "No cache";
            // 
            // cmScriptTabs
            // 
            cmScriptTabs.ImageScalingSize = new System.Drawing.Size(20, 20);
            cmScriptTabs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { closeToolStripMenuItem, toolStripMenuItem4, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripMenuItem5, runToolStripMenuItem });
            cmScriptTabs.Name = "cmScriptTabs";
            cmScriptTabs.Size = new System.Drawing.Size(128, 120);
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("closeToolStripMenuItem.Image");
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new System.Drawing.Size(127, 26);
            closeToolStripMenuItem.Text = "Close";
            closeToolStripMenuItem.Click += closeToolStripMenuItem_Click;
            // 
            // toolStripMenuItem4
            // 
            toolStripMenuItem4.Name = "toolStripMenuItem4";
            toolStripMenuItem4.Size = new System.Drawing.Size(124, 6);
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("saveToolStripMenuItem.Image");
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new System.Drawing.Size(127, 26);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveScriptToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new System.Drawing.Size(127, 26);
            saveAsToolStripMenuItem.Text = "Save As...";
            saveAsToolStripMenuItem.Click += saveScriptAsToolStripMenuItem_Click;
            // 
            // toolStripMenuItem5
            // 
            toolStripMenuItem5.Name = "toolStripMenuItem5";
            toolStripMenuItem5.Size = new System.Drawing.Size(124, 6);
            // 
            // runToolStripMenuItem
            // 
            runToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("runToolStripMenuItem.Image");
            runToolStripMenuItem.Name = "runToolStripMenuItem";
            runToolStripMenuItem.Size = new System.Drawing.Size(127, 26);
            runToolStripMenuItem.Text = "Run";
            runToolStripMenuItem.Click += toolStripButtonRunScript_Click;
            // 
            // sqlOutput
            // 
            sqlOutput.Controls.Add(tabPage4);
            sqlOutput.Controls.Add(tabPage5);
            sqlOutput.DisplayFilterRow = false;
            sqlOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            sqlOutput.Location = new System.Drawing.Point(0, 0);
            sqlOutput.Name = "sqlOutput";
            sqlOutput.SelectedIndex = 0;
            sqlOutput.Size = new System.Drawing.Size(954, 261);
            sqlOutput.TabIndex = 0;
            // 
            // tabPage4
            // 
            tabPage4.Location = new System.Drawing.Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new System.Drawing.Size(946, 233);
            tabPage4.TabIndex = 0;
            tabPage4.Text = "Output";
            tabPage4.Visible = false;
            // 
            // tabPage5
            // 
            tabPage5.Location = new System.Drawing.Point(4, 24);
            tabPage5.Name = "tabPage5";
            tabPage5.Size = new System.Drawing.Size(946, 233);
            tabPage5.TabIndex = 1;
            tabPage5.Text = "   +";
            tabPage5.Visible = false;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripDatabaseConnectionsDropDownButton
            // 
            toolStripDatabaseConnectionsDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripDatabaseConnectionsDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDatabaseConnectionsDropDownButton.Name = "toolStripDatabaseConnectionsDropDownButton";
            toolStripDatabaseConnectionsDropDownButton.Size = new System.Drawing.Size(73, 24);
            toolStripDatabaseConnectionsDropDownButton.Text = "Databases";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripScripts
            // 
            toolStripScripts.Location = new System.Drawing.Point(0, 0);
            toolStripScripts.Name = "toolStripScripts";
            toolStripScripts.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripButtonNewScript, toolStripButtonOpenScript, toolStripButtoSaveScript, toolStripButtonRunScript, toolStripSeparator1, toolStripDatabaseConnectionsDropDownButton, toolStripSeparator2 });
            toolStripScripts.Size = new System.Drawing.Size(954, 25);
            toolStripScripts.TabIndex = 1;
            toolStripScripts.Text = "toolStrip1";

            // 
            // toolStripButtonNewScript
            // 
            toolStripButtonNewScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            toolStripButtonNewScript.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonNewScript.Image");
            toolStripButtonNewScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonNewScript.Name = "toolStripButtonNewScript";
            toolStripButtonNewScript.Size = new System.Drawing.Size(24, 24);
            toolStripButtonNewScript.Text = "toolStripButton1";
            toolStripButtonNewScript.ToolTipText = "New script";
            toolStripButtonNewScript.Click += toolStripButtonNewScript_Click;
            // 
            // toolStripButtonOpenScript
            // 
            toolStripButtonOpenScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            toolStripButtonOpenScript.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonOpenScript.Image");
            toolStripButtonOpenScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonOpenScript.Name = "toolStripButtonOpenScript";
            toolStripButtonOpenScript.Size = new System.Drawing.Size(24, 24);
            toolStripButtonOpenScript.Text = "toolStripButton2";
            toolStripButtonOpenScript.ToolTipText = "Open";
            toolStripButtonOpenScript.Click += toolStripButtonOpenScript_Click;
            // 
            // toolStripButtoSaveScript
            // 
            toolStripButtoSaveScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            toolStripButtoSaveScript.Image = (System.Drawing.Image)resources.GetObject("toolStripButtoSaveScript.Image");
            toolStripButtoSaveScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtoSaveScript.Name = "toolStripButtoSaveScript";
            toolStripButtoSaveScript.Size = new System.Drawing.Size(24, 24);
            toolStripButtoSaveScript.Text = "toolStripButton1";
            toolStripButtoSaveScript.ToolTipText = "Save";
            toolStripButtoSaveScript.Click += toolStripButtoSaveScript_Click;
            // 
            // toolStripButtonRunScript
            // 
            toolStripButtonRunScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            toolStripButtonRunScript.Image = (System.Drawing.Image)resources.GetObject("toolStripButtonRunScript.Image");
            toolStripButtonRunScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButtonRunScript.Name = "toolStripButtonRunScript";
            toolStripButtonRunScript.Size = new System.Drawing.Size(24, 24);
            toolStripButtonRunScript.Text = "toolStripButton3";
            toolStripButtonRunScript.ToolTipText = "Execute";
            toolStripButtonRunScript.Click += toolStripButtonRunScript_Click;
            // 
            // DatabaseConnectionUserControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(MainViewSplitContainer);
            Name = "DatabaseConnectionUserControl";
            Size = new System.Drawing.Size(954, 676);
            MainViewSplitContainer.Panel1.ResumeLayout(false);
            MainViewSplitContainer.Panel1.PerformLayout();
            MainViewSplitContainer.Panel2.ResumeLayout(false);
            MainViewSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)MainViewSplitContainer).EndInit();
            MainViewSplitContainer.ResumeLayout(false);
            tabControlMainDocs.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            sqlOutput.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip cmScriptTabs;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.SplitContainer MainViewSplitContainer;
        private System.Windows.Forms.TabControl tabControlMainDocs;
        private System.Windows.Forms.TabPage tabPage1;
        private CommandPrompt.CmdLineControl cmdLineControl;
        private SqlOutputTabContainer sqlOutput;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripMessageLabel;
        private System.Windows.Forms.ToolStripStatusLabel visibleRowsToolStripStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelMetaData;
        private System.Windows.Forms.ToolStrip toolStripScripts;
        private System.Windows.Forms.ToolStripButton toolStripButtoSaveScript;
        private System.Windows.Forms.ToolStripButton toolStripButtonOpenScript;
        private System.Windows.Forms.ToolStripButton toolStripButtonRunScript;
        private System.Windows.Forms.ToolStripButton toolStripButtonNewScript;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDatabaseConnectionsDropDownButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}
