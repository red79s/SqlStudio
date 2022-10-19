namespace SqlStudio
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControlMainDocs = new System.Windows.Forms.TabControl();
            this.tpMainInput = new System.Windows.Forms.TabPage();
            this.cmdLineControl = new CommandPrompt.CmdLineControl();
            this.toolStripMainWindow = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonNewScript = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonOpenScript = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtoSaveScript = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRunScript = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDatabaseConnectionsDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSQLiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSqlCEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openConfigDbToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openCvsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.newScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveScriptAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.cancelExecutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatQueryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayFilterRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aliasesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoQueriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadSBDZipFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataTabsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripMessageLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.visibleRowsToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabelMetaData = new System.Windows.Forms.ToolStripStatusLabel();
            this.cmScriptTabs = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sqlOutput = new SqlStudio.SqlOutputTabContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControlMainDocs.SuspendLayout();
            this.tpMainInput.SuspendLayout();
            this.toolStripMainWindow.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.cmScriptTabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControlMainDocs);
            this.splitContainer1.Panel1.Controls.Add(this.toolStripMainWindow);
            this.splitContainer1.Panel1.Controls.Add(this.menuStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.sqlOutput);
            this.splitContainer1.Size = new System.Drawing.Size(811, 626);
            this.splitContainer1.SplitterDistance = 417;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabControlMainDocs
            // 
            this.tabControlMainDocs.Controls.Add(this.tpMainInput);
            this.tabControlMainDocs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMainDocs.Location = new System.Drawing.Point(0, 51);
            this.tabControlMainDocs.Name = "tabControlMainDocs";
            this.tabControlMainDocs.SelectedIndex = 0;
            this.tabControlMainDocs.Size = new System.Drawing.Size(811, 366);
            this.tabControlMainDocs.TabIndex = 4;
            // 
            // tpMainInput
            // 
            this.tpMainInput.Controls.Add(this.cmdLineControl);
            this.tpMainInput.Location = new System.Drawing.Point(4, 22);
            this.tpMainInput.Name = "tpMainInput";
            this.tpMainInput.Padding = new System.Windows.Forms.Padding(3);
            this.tpMainInput.Size = new System.Drawing.Size(803, 340);
            this.tpMainInput.TabIndex = 0;
            this.tpMainInput.Text = "Input";
            this.tpMainInput.UseVisualStyleBackColor = true;
            // 
            // cmdLineControl
            // 
            this.cmdLineControl.AcceptsKeyInput = false;
            this.cmdLineControl.AutoScroll = true;
            this.cmdLineControl.AutoScrollMinSize = new System.Drawing.Size(38, 29);
            this.cmdLineControl.BackColor = System.Drawing.Color.White;
            this.cmdLineControl.BackColorIconPane = System.Drawing.Color.Yellow;
            this.cmdLineControl.BackColorLineNum = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.cmdLineControl.BackColorSelection = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(150)))), ((int)(((byte)(223)))));
            this.cmdLineControl.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.cmdLineControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdLineControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdLineControl.ForeColor = System.Drawing.Color.Black;
            this.cmdLineControl.ForeColorLineNum = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cmdLineControl.Location = new System.Drawing.Point(3, 3);
            this.cmdLineControl.Name = "cmdLineControl";
            this.cmdLineControl.ShowIconPane = false;
            this.cmdLineControl.ShowLineNumbers = true;
            this.cmdLineControl.Size = new System.Drawing.Size(797, 334);
            this.cmdLineControl.TabIndex = 3;
            // 
            // toolStripMainWindow
            // 
            this.toolStripMainWindow.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripMainWindow.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonNewScript,
            this.toolStripButtonOpenScript,
            this.toolStripButtoSaveScript,
            this.toolStripButtonRunScript,
            this.toolStripSeparator1,
            this.toolStripDatabaseConnectionsDropDownButton,
            this.toolStripSeparator2});
            this.toolStripMainWindow.Location = new System.Drawing.Point(0, 24);
            this.toolStripMainWindow.Name = "toolStripMainWindow";
            this.toolStripMainWindow.Size = new System.Drawing.Size(811, 27);
            this.toolStripMainWindow.TabIndex = 2;
            this.toolStripMainWindow.Text = "toolStrip1";
            // 
            // toolStripButtonNewScript
            // 
            this.toolStripButtonNewScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonNewScript.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonNewScript.Image")));
            this.toolStripButtonNewScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNewScript.Name = "toolStripButtonNewScript";
            this.toolStripButtonNewScript.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonNewScript.Text = "toolStripButton1";
            this.toolStripButtonNewScript.ToolTipText = "New script";
            this.toolStripButtonNewScript.Click += new System.EventHandler(this.toolStripButtonNewScript_Click);
            // 
            // toolStripButtonOpenScript
            // 
            this.toolStripButtonOpenScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonOpenScript.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonOpenScript.Image")));
            this.toolStripButtonOpenScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOpenScript.Name = "toolStripButtonOpenScript";
            this.toolStripButtonOpenScript.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonOpenScript.Text = "toolStripButton2";
            this.toolStripButtonOpenScript.ToolTipText = "Open";
            this.toolStripButtonOpenScript.Click += new System.EventHandler(this.toolStripButtonOpenScript_Click);
            // 
            // toolStripButtoSaveScript
            // 
            this.toolStripButtoSaveScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtoSaveScript.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtoSaveScript.Image")));
            this.toolStripButtoSaveScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtoSaveScript.Name = "toolStripButtoSaveScript";
            this.toolStripButtoSaveScript.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtoSaveScript.Text = "toolStripButton1";
            this.toolStripButtoSaveScript.ToolTipText = "Save";
            this.toolStripButtoSaveScript.Click += new System.EventHandler(this.toolStripButtoSaveScript_Click);
            // 
            // toolStripButtonRunScript
            // 
            this.toolStripButtonRunScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRunScript.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRunScript.Image")));
            this.toolStripButtonRunScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRunScript.Name = "toolStripButtonRunScript";
            this.toolStripButtonRunScript.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonRunScript.Text = "toolStripButton3";
            this.toolStripButtonRunScript.ToolTipText = "Execute";
            this.toolStripButtonRunScript.Click += new System.EventHandler(this.toolStripButtonRunScript_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripDatabaseConnectionsDropDownButton
            // 
            this.toolStripDatabaseConnectionsDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDatabaseConnectionsDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDatabaseConnectionsDropDownButton.Name = "toolStripDatabaseConnectionsDropDownButton";
            this.toolStripDatabaseConnectionsDropDownButton.Size = new System.Drawing.Size(73, 24);
            this.toolStripDatabaseConnectionsDropDownButton.Text = "Databases";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.windowsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(811, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.openSQLiteToolStripMenuItem,
            this.openSqlCEToolStripMenuItem,
            this.openConfigDbToolStripMenuItem,
            this.openCvsToolStripMenuItem,
            this.toolStripMenuItem6,
            this.connectToolStripMenuItem,
            this.closeConnectionToolStripMenuItem,
            this.toolStripMenuItem3,
            this.newScriptToolStripMenuItem,
            this.openScriptToolStripMenuItem,
            this.saveScriptToolStripMenuItem,
            this.saveScriptAsToolStripMenuItem,
            this.runScriptToolStripMenuItem,
            this.toolStripMenuItem2,
            this.cancelExecutionToolStripMenuItem,
            this.toolStripMenuItem7,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // openSQLiteToolStripMenuItem
            // 
            this.openSQLiteToolStripMenuItem.Name = "openSQLiteToolStripMenuItem";
            this.openSQLiteToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.openSQLiteToolStripMenuItem.Text = "Open SQLite...";
            this.openSQLiteToolStripMenuItem.Click += new System.EventHandler(this.openSQLiteToolStripMenuItem_Click);
            // 
            // openSqlCEToolStripMenuItem
            // 
            this.openSqlCEToolStripMenuItem.Name = "openSqlCEToolStripMenuItem";
            this.openSqlCEToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.openSqlCEToolStripMenuItem.Text = "Open SqlCE...";
            this.openSqlCEToolStripMenuItem.Click += new System.EventHandler(this.openSqlCEToolStripMenuItem_Click);
            // 
            // openConfigDbToolStripMenuItem
            // 
            this.openConfigDbToolStripMenuItem.Name = "openConfigDbToolStripMenuItem";
            this.openConfigDbToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.openConfigDbToolStripMenuItem.Text = "Open config db";
            this.openConfigDbToolStripMenuItem.Click += new System.EventHandler(this.openConfigDbToolStripMenuItem_Click);
            // 
            // openCvsToolStripMenuItem
            // 
            this.openCvsToolStripMenuItem.Name = "openCvsToolStripMenuItem";
            this.openCvsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.openCvsToolStripMenuItem.Text = "Open csv...";
            this.openCvsToolStripMenuItem.Click += new System.EventHandler(this.openCsvToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(163, 6);
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.newToolStripMenuItem});
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.connectToolStripMenuItem.Text = "Connect";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(104, 6);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.newToolStripMenuItem.Text = "New...";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // closeConnectionToolStripMenuItem
            // 
            this.closeConnectionToolStripMenuItem.Name = "closeConnectionToolStripMenuItem";
            this.closeConnectionToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.closeConnectionToolStripMenuItem.Text = "Close connection";
            this.closeConnectionToolStripMenuItem.Click += new System.EventHandler(this.closeConnectionToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(163, 6);
            // 
            // newScriptToolStripMenuItem
            // 
            this.newScriptToolStripMenuItem.Name = "newScriptToolStripMenuItem";
            this.newScriptToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.newScriptToolStripMenuItem.Text = "New script";
            this.newScriptToolStripMenuItem.Click += new System.EventHandler(this.newScriptToolStripMenuItem_Click);
            // 
            // openScriptToolStripMenuItem
            // 
            this.openScriptToolStripMenuItem.Name = "openScriptToolStripMenuItem";
            this.openScriptToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.openScriptToolStripMenuItem.Text = "Open script...";
            this.openScriptToolStripMenuItem.Click += new System.EventHandler(this.openScriptToolStripMenuItem_Click);
            // 
            // saveScriptToolStripMenuItem
            // 
            this.saveScriptToolStripMenuItem.Name = "saveScriptToolStripMenuItem";
            this.saveScriptToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.saveScriptToolStripMenuItem.Text = "Save script";
            this.saveScriptToolStripMenuItem.Click += new System.EventHandler(this.saveScriptToolStripMenuItem_Click);
            // 
            // saveScriptAsToolStripMenuItem
            // 
            this.saveScriptAsToolStripMenuItem.Name = "saveScriptAsToolStripMenuItem";
            this.saveScriptAsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.saveScriptAsToolStripMenuItem.Text = "Save script as...";
            this.saveScriptAsToolStripMenuItem.Click += new System.EventHandler(this.saveScriptAsToolStripMenuItem_Click);
            // 
            // runScriptToolStripMenuItem
            // 
            this.runScriptToolStripMenuItem.Name = "runScriptToolStripMenuItem";
            this.runScriptToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.runScriptToolStripMenuItem.Text = "Run script...";
            this.runScriptToolStripMenuItem.Click += new System.EventHandler(this.runScriptToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(163, 6);
            // 
            // cancelExecutionToolStripMenuItem
            // 
            this.cancelExecutionToolStripMenuItem.Name = "cancelExecutionToolStripMenuItem";
            this.cancelExecutionToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.cancelExecutionToolStripMenuItem.Text = "Cancel execution";
            this.cancelExecutionToolStripMenuItem.Click += new System.EventHandler(this.cancelExecutionToolStripMenuItem_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(163, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.formatQueryToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // formatQueryToolStripMenuItem
            // 
            this.formatQueryToolStripMenuItem.Name = "formatQueryToolStripMenuItem";
            this.formatQueryToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.formatQueryToolStripMenuItem.Text = "Format Query...";
            this.formatQueryToolStripMenuItem.Click += new System.EventHandler(this.formatQueryToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.displayFilterRowToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // displayFilterRowToolStripMenuItem
            // 
            this.displayFilterRowToolStripMenuItem.CheckOnClick = true;
            this.displayFilterRowToolStripMenuItem.Name = "displayFilterRowToolStripMenuItem";
            this.displayFilterRowToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.displayFilterRowToolStripMenuItem.Text = "Display Filter Row";
            this.displayFilterRowToolStripMenuItem.CheckedChanged += new System.EventHandler(this.displayFilterRowToolStripMenuItem_CheckedChanged);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aliasesToolStripMenuItem,
            this.autoQueriesToolStripMenuItem,
            this.uploadSBDZipFileToolStripMenuItem,
            this.logSearchToolStripMenuItem,
            this.logImportToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // aliasesToolStripMenuItem
            // 
            this.aliasesToolStripMenuItem.Name = "aliasesToolStripMenuItem";
            this.aliasesToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.aliasesToolStripMenuItem.Text = "Aliases...";
            this.aliasesToolStripMenuItem.Click += new System.EventHandler(this.aliasesToolStripMenuItem_Click);
            // 
            // autoQueriesToolStripMenuItem
            // 
            this.autoQueriesToolStripMenuItem.Name = "autoQueriesToolStripMenuItem";
            this.autoQueriesToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.autoQueriesToolStripMenuItem.Text = "Auto Queries...";
            this.autoQueriesToolStripMenuItem.Click += new System.EventHandler(this.autoQueriesToolStripMenuItem_Click);
            // 
            // uploadSBDZipFileToolStripMenuItem
            // 
            this.uploadSBDZipFileToolStripMenuItem.Name = "uploadSBDZipFileToolStripMenuItem";
            this.uploadSBDZipFileToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.uploadSBDZipFileToolStripMenuItem.Text = "Upload SBD zip file...";
            this.uploadSBDZipFileToolStripMenuItem.Click += new System.EventHandler(this.uploadSBDZipFileToolStripMenuItem_Click);
            // 
            // logSearchToolStripMenuItem
            // 
            this.logSearchToolStripMenuItem.Name = "logSearchToolStripMenuItem";
            this.logSearchToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.logSearchToolStripMenuItem.Text = "LogSearch...";
            this.logSearchToolStripMenuItem.Click += new System.EventHandler(this.logSearchToolStripMenuItem_Click);
            // 
            // logImportToolStripMenuItem
            // 
            this.logImportToolStripMenuItem.Name = "logImportToolStripMenuItem";
            this.logImportToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.logImportToolStripMenuItem.Text = "LogImport...";
            this.logImportToolStripMenuItem.Click += new System.EventHandler(this.logImportToolStripMenuItem_Click);
            // 
            // windowsToolStripMenuItem
            // 
            this.windowsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataTabsToolStripMenuItem});
            this.windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
            this.windowsToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.windowsToolStripMenuItem.Text = "Windows";
            // 
            // dataTabsToolStripMenuItem
            // 
            this.dataTabsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem1});
            this.dataTabsToolStripMenuItem.Name = "dataTabsToolStripMenuItem";
            this.dataTabsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.dataTabsToolStripMenuItem.Text = "Data tabs";
            // 
            // newToolStripMenuItem1
            // 
            this.newToolStripMenuItem1.Name = "newToolStripMenuItem1";
            this.newToolStripMenuItem1.Size = new System.Drawing.Size(98, 22);
            this.newToolStripMenuItem1.Text = "New";
            this.newToolStripMenuItem1.Click += new System.EventHandler(this.newToolStripMenuItem1_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.helpToolStripMenuItem1});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(108, 22);
            this.helpToolStripMenuItem1.Text = "Help...";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMessageLabel,
            this.visibleRowsToolStripStatusLabel,
            this.toolStripProgressBar,
            this.toolStripStatusLabelMetaData});
            this.statusStrip1.Location = new System.Drawing.Point(0, 626);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(811, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripMessageLabel
            // 
            this.toolStripMessageLabel.Name = "toolStripMessageLabel";
            this.toolStripMessageLabel.Size = new System.Drawing.Size(622, 17);
            this.toolStripMessageLabel.Spring = true;
            this.toolStripMessageLabel.Text = "Disconnected";
            this.toolStripMessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // visibleRowsToolStripStatusLabel
            // 
            this.visibleRowsToolStripStatusLabel.Name = "visibleRowsToolStripStatusLabel";
            this.visibleRowsToolStripStatusLabel.Size = new System.Drawing.Size(13, 17);
            this.visibleRowsToolStripStatusLabel.Text = "0";
            this.visibleRowsToolStripStatusLabel.ToolTipText = "Number of rows displayed";
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripStatusLabelMetaData
            // 
            this.toolStripStatusLabelMetaData.Name = "toolStripStatusLabelMetaData";
            this.toolStripStatusLabelMetaData.Size = new System.Drawing.Size(59, 17);
            this.toolStripStatusLabelMetaData.Text = "No Cache";
            // 
            // cmScriptTabs
            // 
            this.cmScriptTabs.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmScriptTabs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem,
            this.toolStripMenuItem4,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem5,
            this.runToolStripMenuItem});
            this.cmScriptTabs.Name = "cmScriptTabs";
            this.cmScriptTabs.Size = new System.Drawing.Size(128, 120);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("closeToolStripMenuItem.Image")));
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(127, 26);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(124, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(127, 26);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveScriptToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(127, 26);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveScriptAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(124, 6);
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("runToolStripMenuItem.Image")));
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(127, 26);
            this.runToolStripMenuItem.Text = "Run";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.toolStripButtonRunScript_Click);
            // 
            // sqlOutput
            // 
            this.sqlOutput.DisplayFilterRow = false;
            this.sqlOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sqlOutput.Location = new System.Drawing.Point(0, 0);
            this.sqlOutput.Name = "sqlOutput";
            this.sqlOutput.SelectedIndex = 0;
            this.sqlOutput.Size = new System.Drawing.Size(811, 205);
            this.sqlOutput.TabIndex = 0;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(811, 648);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "Sql Studio";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControlMainDocs.ResumeLayout(false);
            this.tpMainInput.ResumeLayout(false);
            this.toolStripMainWindow.ResumeLayout(false);
            this.toolStripMainWindow.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.cmScriptTabs.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStrip toolStripMainWindow;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeConnectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem aliasesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataTabsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private SqlOutputTabContainer sqlOutput;
        private CommandPrompt.CmdLineControl cmdLineControl;
        private System.Windows.Forms.TabControl tabControlMainDocs;
        private System.Windows.Forms.TabPage tpMainInput;
        private System.Windows.Forms.ToolStripMenuItem newScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtoSaveScript;
        private System.Windows.Forms.ToolStripButton toolStripButtonOpenScript;
        private System.Windows.Forms.ToolStripButton toolStripButtonRunScript;
        private System.Windows.Forms.ToolStripButton toolStripButtonNewScript;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripMessageLabel;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripMenuItem saveScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveScriptAsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip cmScriptTabs;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSQLiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem displayFilterRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelMetaData;
        private System.Windows.Forms.ToolStripMenuItem runScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatQueryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSqlCEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uploadSBDZipFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cancelExecutionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem openConfigDbToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoQueriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel visibleRowsToolStripStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem logSearchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openCvsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logImportToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDatabaseConnectionsDropDownButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}

