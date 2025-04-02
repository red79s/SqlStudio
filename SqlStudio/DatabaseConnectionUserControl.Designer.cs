using SqlStudio.Properties;
using System.Drawing;
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseConnectionUserControl));
            MainViewSplitContainer = new SplitContainer();
            toolStripScripts = new ToolStrip();
            toolStripButtonNewScript = new ToolStripButton();
            toolStripButtonOpenScript = new ToolStripButton();
            toolStripButtoSaveScript = new ToolStripButton();
            toolStripButtonRunScript = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripLabel1 = new ToolStripLabel();
            toolStripDatabaseConnectionsComboBox = new ToolStripComboBox();
            tabControlMainDocs = new TabControl();
            tabPage1 = new TabPage();
            cmdLineControl = new CommandPrompt.CmdLineControl();
            statusStrip = new StatusStrip();
            toolStripMessageLabel = new ToolStripStatusLabel();
            visibleRowsToolStripStatusLabel = new ToolStripStatusLabel();
            toolStripProgressBar = new ToolStripProgressBar();
            toolStripStatusLabelMetaData = new ToolStripStatusLabel();
            sqlOutput = new SqlOutputTabContainer();
            tabPageAddOutputTab = new TabPage();
            cmScriptTabs = new ContextMenuStrip(components);
            closeToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem4 = new ToolStripSeparator();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem5 = new ToolStripSeparator();
            runToolStripMenuItem = new ToolStripMenuItem();
            tabPageTextOutput = new TabPage();
            ((System.ComponentModel.ISupportInitialize)MainViewSplitContainer).BeginInit();
            MainViewSplitContainer.Panel1.SuspendLayout();
            MainViewSplitContainer.Panel2.SuspendLayout();
            MainViewSplitContainer.SuspendLayout();
            toolStripScripts.SuspendLayout();
            tabControlMainDocs.SuspendLayout();
            tabPage1.SuspendLayout();
            statusStrip.SuspendLayout();
            cmScriptTabs.SuspendLayout();
            SuspendLayout();
            // 
            // MainViewSplitContainer
            // 
            MainViewSplitContainer.Dock = DockStyle.Fill;
            MainViewSplitContainer.Location = new Point(0, 0);
            MainViewSplitContainer.Name = "MainViewSplitContainer";
            MainViewSplitContainer.Orientation = Orientation.Horizontal;
            // 
            // MainViewSplitContainer.Panel1
            // 
            MainViewSplitContainer.Panel1.Controls.Add(toolStripScripts);
            MainViewSplitContainer.Panel1.Controls.Add(tabControlMainDocs);
            // 
            // MainViewSplitContainer.Panel2
            // 
            MainViewSplitContainer.Panel2.Controls.Add(statusStrip);
            MainViewSplitContainer.Panel2.Controls.Add(sqlOutput);
            MainViewSplitContainer.Size = new Size(954, 676);
            MainViewSplitContainer.SplitterDistance = 411;
            MainViewSplitContainer.TabIndex = 0;
            // 
            // toolStripScripts
            // 
            toolStripScripts.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            toolStripScripts.AutoSize = false;
            toolStripScripts.Dock = DockStyle.None;
            toolStripScripts.Items.AddRange(new ToolStripItem[] { toolStripButtonNewScript, toolStripButtonOpenScript, toolStripButtoSaveScript, toolStripButtonRunScript, toolStripSeparator1, toolStripLabel1, toolStripDatabaseConnectionsComboBox });
            toolStripScripts.Location = new Point(0, 0);
            toolStripScripts.Name = "toolStripScripts";
            toolStripScripts.Size = new Size(954, 25);
            toolStripScripts.TabIndex = 1;
            toolStripScripts.Text = "toolStrip1";
            // 
            // toolStripButtonNewScript
            // 
            toolStripButtonNewScript.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonNewScript.Image = (Image)resources.GetObject("toolStripButtonNewScript.Image");
            toolStripButtonNewScript.ImageTransparentColor = Color.Magenta;
            toolStripButtonNewScript.Name = "toolStripButtonNewScript";
            toolStripButtonNewScript.Size = new Size(23, 22);
            toolStripButtonNewScript.Text = "toolStripButton1";
            toolStripButtonNewScript.ToolTipText = "New script";
            toolStripButtonNewScript.Click += toolStripButtonNewScript_Click;
            // 
            // toolStripButtonOpenScript
            // 
            toolStripButtonOpenScript.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonOpenScript.Image = (Image)resources.GetObject("toolStripButtonOpenScript.Image");
            toolStripButtonOpenScript.ImageTransparentColor = Color.Magenta;
            toolStripButtonOpenScript.Name = "toolStripButtonOpenScript";
            toolStripButtonOpenScript.Size = new Size(23, 22);
            toolStripButtonOpenScript.Text = "toolStripButton2";
            toolStripButtonOpenScript.ToolTipText = "Open";
            toolStripButtonOpenScript.Click += toolStripButtonOpenScript_Click;
            // 
            // toolStripButtoSaveScript
            // 
            toolStripButtoSaveScript.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtoSaveScript.Image = (Image)resources.GetObject("toolStripButtoSaveScript.Image");
            toolStripButtoSaveScript.ImageTransparentColor = Color.Magenta;
            toolStripButtoSaveScript.Name = "toolStripButtoSaveScript";
            toolStripButtoSaveScript.Size = new Size(23, 22);
            toolStripButtoSaveScript.Text = "toolStripButton1";
            toolStripButtoSaveScript.ToolTipText = "Save";
            toolStripButtoSaveScript.Click += toolStripButtoSaveScript_Click;
            // 
            // toolStripButtonRunScript
            // 
            toolStripButtonRunScript.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonRunScript.Image = (Image)resources.GetObject("toolStripButtonRunScript.Image");
            toolStripButtonRunScript.ImageTransparentColor = Color.Magenta;
            toolStripButtonRunScript.Name = "toolStripButtonRunScript";
            toolStripButtonRunScript.Size = new Size(23, 22);
            toolStripButtonRunScript.Text = "toolStripButton3";
            toolStripButtonRunScript.ToolTipText = "Execute";
            toolStripButtonRunScript.Click += toolStripButtonRunScript_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(55, 22);
            toolStripLabel1.Text = "Database";
            // 
            // toolStripDatabaseConnectionsComboBox
            // 
            toolStripDatabaseConnectionsComboBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            toolStripDatabaseConnectionsComboBox.Name = "toolStripDatabaseConnectionsComboBox";
            toolStripDatabaseConnectionsComboBox.Size = new Size(200, 25);
            // 
            // tabControlMainDocs
            // 
            tabControlMainDocs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControlMainDocs.Controls.Add(tabPage1);
            tabControlMainDocs.Location = new Point(0, 28);
            tabControlMainDocs.Name = "tabControlMainDocs";
            tabControlMainDocs.SelectedIndex = 0;
            tabControlMainDocs.Size = new Size(954, 380);
            tabControlMainDocs.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(cmdLineControl);
            tabPage1.Font = new Font("Microsoft Sans Serif", 9F);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(946, 352);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Input";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // cmdLineControl
            // 
            cmdLineControl.AcceptsKeyInput = false;
            cmdLineControl.AutoScroll = true;
            cmdLineControl.AutoScrollMinSize = new Size(38, 29);
            cmdLineControl.BackColor = Color.White;
            cmdLineControl.BackColorIconPane = Color.Yellow;
            cmdLineControl.BackColorLineNum = Color.FromArgb(224, 224, 224);
            cmdLineControl.BackColorSelection = Color.FromArgb(122, 150, 223);
            cmdLineControl.Cursor = Cursors.IBeam;
            cmdLineControl.Dock = DockStyle.Fill;
            cmdLineControl.Font = new Font("Microsoft Sans Serif", 12F);
            cmdLineControl.ForeColor = Color.Black;
            cmdLineControl.ForeColorLineNum = Color.FromArgb(64, 64, 64);
            cmdLineControl.Location = new Point(3, 3);
            cmdLineControl.Name = "cmdLineControl";
            cmdLineControl.ShowIconPane = false;
            cmdLineControl.ShowLineNumbers = true;
            cmdLineControl.Size = new Size(940, 346);
            cmdLineControl.TabIndex = 3;
            // 
            // statusStrip
            // 
            statusStrip.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            statusStrip.AutoSize = false;
            statusStrip.Dock = DockStyle.None;
            statusStrip.Items.AddRange(new ToolStripItem[] { toolStripMessageLabel, visibleRowsToolStripStatusLabel, toolStripProgressBar, toolStripStatusLabelMetaData });
            statusStrip.Location = new Point(0, 239);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(954, 22);
            statusStrip.TabIndex = 1;
            statusStrip.Text = "statusStrip1";
            // 
            // toolStripMessageLabel
            // 
            toolStripMessageLabel.AutoSize = false;
            toolStripMessageLabel.Name = "toolStripMessageLabel";
            toolStripMessageLabel.Size = new Size(767, 17);
            toolStripMessageLabel.Spring = true;
            toolStripMessageLabel.Text = "Disconnected";
            toolStripMessageLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // visibleRowsToolStripStatusLabel
            // 
            visibleRowsToolStripStatusLabel.Name = "visibleRowsToolStripStatusLabel";
            visibleRowsToolStripStatusLabel.Size = new Size(13, 17);
            visibleRowsToolStripStatusLabel.Text = "0";
            // 
            // toolStripProgressBar
            // 
            toolStripProgressBar.ForeColor = SystemColors.AppWorkspace;
            toolStripProgressBar.Name = "toolStripProgressBar";
            toolStripProgressBar.Size = new Size(100, 16);
            // 
            // toolStripStatusLabelMetaData
            // 
            toolStripStatusLabelMetaData.Name = "toolStripStatusLabelMetaData";
            toolStripStatusLabelMetaData.Size = new Size(57, 17);
            toolStripStatusLabelMetaData.Text = "No cache";
            // 
            // sqlOutput
            // 
            sqlOutput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            sqlOutput.DisplayFilterRow = false;
            sqlOutput.Location = new Point(0, -1);
            sqlOutput.Name = "sqlOutput";
            sqlOutput.SelectedIndex = 0;
            sqlOutput.Size = new Size(951, 237);
            sqlOutput.TabIndex = 0;
            // 
            // tabPageAddOutputTab
            // 
            tabPageAddOutputTab.Location = new Point(4, 24);
            tabPageAddOutputTab.Name = "tabPageAddOutputTab";
            tabPageAddOutputTab.Size = new Size(946, 233);
            tabPageAddOutputTab.TabIndex = 1;
            tabPageAddOutputTab.Text = "   +";
            tabPageAddOutputTab.Visible = false;
            // 
            // cmScriptTabs
            // 
            cmScriptTabs.ImageScalingSize = new Size(20, 20);
            cmScriptTabs.Items.AddRange(new ToolStripItem[] { closeToolStripMenuItem, toolStripMenuItem4, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripMenuItem5, runToolStripMenuItem });
            cmScriptTabs.Name = "cmScriptTabs";
            cmScriptTabs.Size = new Size(126, 120);
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Image = (Image)resources.GetObject("closeToolStripMenuItem.Image");
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new Size(125, 26);
            closeToolStripMenuItem.Tag = "";
            closeToolStripMenuItem.Text = "Close";
            closeToolStripMenuItem.Click += closeToolStripMenuItem_Click;
            // 
            // toolStripMenuItem4
            // 
            toolStripMenuItem4.Name = "toolStripMenuItem4";
            toolStripMenuItem4.Size = new Size(122, 6);
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Image = (Image)resources.GetObject("saveToolStripMenuItem.Image");
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(125, 26);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveScriptToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Image = (Image)resources.GetObject("saveAsToolStripMenuItem.Image");
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(125, 26);
            saveAsToolStripMenuItem.Text = "Save as...";
            saveAsToolStripMenuItem.Click += saveScriptAsToolStripMenuItem_Click;
            // 
            // toolStripMenuItem5
            // 
            toolStripMenuItem5.Name = "toolStripMenuItem5";
            toolStripMenuItem5.Size = new Size(122, 6);
            // 
            // runToolStripMenuItem
            // 
            runToolStripMenuItem.Image = (Image)resources.GetObject("runToolStripMenuItem.Image");
            runToolStripMenuItem.Name = "runToolStripMenuItem";
            runToolStripMenuItem.Size = new Size(125, 26);
            runToolStripMenuItem.Text = "Run";
            runToolStripMenuItem.Click += toolStripButtonRunScript_Click;
            // 
            // tabPageTextOutput
            // 
            tabPageTextOutput.Location = new Point(4, 24);
            tabPageTextOutput.Name = "tabPageTextOutput";
            tabPageTextOutput.Size = new Size(943, 209);
            tabPageTextOutput.TabIndex = 0;
            tabPageTextOutput.Text = "Output";
            tabPageTextOutput.Visible = false;
            // 
            // DatabaseConnectionUserControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(MainViewSplitContainer);
            Name = "DatabaseConnectionUserControl";
            Size = new Size(954, 676);
            MainViewSplitContainer.Panel1.ResumeLayout(false);
            MainViewSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)MainViewSplitContainer).EndInit();
            MainViewSplitContainer.ResumeLayout(false);
            toolStripScripts.ResumeLayout(false);
            toolStripScripts.PerformLayout();
            tabControlMainDocs.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            cmScriptTabs.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ContextMenuStrip cmScriptTabs;
		private ToolStripMenuItem closeToolStripMenuItem;
		private ToolStripSeparator toolStripMenuItem4;
		private ToolStripMenuItem saveToolStripMenuItem;
		private ToolStripMenuItem saveAsToolStripMenuItem;
		private ToolStripSeparator toolStripMenuItem5;
		private ToolStripMenuItem runToolStripMenuItem;
		private SplitContainer MainViewSplitContainer;
		private TabControl tabControlMainDocs;
		private TabPage tabPage1;
		private CommandPrompt.CmdLineControl cmdLineControl;
		private StatusStrip statusStrip;
		private ToolStripStatusLabel toolStripMessageLabel;
		private ToolStripStatusLabel visibleRowsToolStripStatusLabel;
		private ToolStripProgressBar toolStripProgressBar;
		private ToolStripStatusLabel toolStripStatusLabelMetaData;
		private ToolStrip toolStripScripts;
		private ToolStripButton toolStripButtoSaveScript;
		private ToolStripButton toolStripButtonOpenScript;
		private ToolStripButton toolStripButtonRunScript;
		private ToolStripButton toolStripButtonNewScript;
		private ToolStripSeparator toolStripSeparator1;
		private SqlOutputTabContainer sqlOutput;
		private TabPage tabPageAddOutputTab;
		private TabPage tabPageTextOutput;
        private ToolStripComboBox toolStripDatabaseConnectionsComboBox;
        private ToolStripLabel toolStripLabel1;
    }
}
