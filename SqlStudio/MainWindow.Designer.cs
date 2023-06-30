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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            tabControlMainDocs = new System.Windows.Forms.TabControl();
            tpMainInput = new System.Windows.Forms.TabPage();
            cmdLineControl = new CommandPrompt.CmdLineControl();
            toolStripMainWindow = new System.Windows.Forms.ToolStrip();
            toolStripButtonNewScript = new System.Windows.Forms.ToolStripButton();
            toolStripButtonOpenScript = new System.Windows.Forms.ToolStripButton();
            toolStripButtoSaveScript = new System.Windows.Forms.ToolStripButton();
            toolStripButtonRunScript = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripDatabaseConnectionsDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openSQLiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openSqlCEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openConfigDbToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openCvsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            closeConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            newScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveScriptAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            runScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            cancelExecutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            formatQueryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            copyConnectionStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            displayFilterRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            aliasesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            autoQueriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            uploadSBDZipFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            logSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            logImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            generatePasswordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            generateDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            windowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            dataTabsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            newToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            sqlOutput = new SqlOutputTabContainer();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripMessageLabel = new System.Windows.Forms.ToolStripStatusLabel();
            visibleRowsToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            toolStripStatusLabelMetaData = new System.Windows.Forms.ToolStripStatusLabel();
            cmScriptTabs = new System.Windows.Forms.ContextMenuStrip(components);
            closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            importEnumValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabControlMainDocs.SuspendLayout();
            tpMainInput.SuspendLayout();
            toolStripMainWindow.SuspendLayout();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            cmScriptTabs.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tabControlMainDocs);
            splitContainer1.Panel1.Controls.Add(toolStripMainWindow);
            splitContainer1.Panel1.Controls.Add(menuStrip1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(sqlOutput);
            splitContainer1.Size = new System.Drawing.Size(946, 724);
            splitContainer1.SplitterDistance = 482;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 0;
            // 
            // tabControlMainDocs
            // 
            tabControlMainDocs.Controls.Add(tpMainInput);
            tabControlMainDocs.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControlMainDocs.Location = new System.Drawing.Point(0, 51);
            tabControlMainDocs.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabControlMainDocs.Name = "tabControlMainDocs";
            tabControlMainDocs.SelectedIndex = 0;
            tabControlMainDocs.Size = new System.Drawing.Size(946, 431);
            tabControlMainDocs.TabIndex = 4;
            // 
            // tpMainInput
            // 
            tpMainInput.Controls.Add(cmdLineControl);
            tpMainInput.Location = new System.Drawing.Point(4, 24);
            tpMainInput.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpMainInput.Name = "tpMainInput";
            tpMainInput.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tpMainInput.Size = new System.Drawing.Size(938, 403);
            tpMainInput.TabIndex = 0;
            tpMainInput.Text = "Input";
            tpMainInput.UseVisualStyleBackColor = true;
            // 
            // cmdLineControl
            // 
            cmdLineControl.AcceptsKeyInput = false;
            cmdLineControl.AutoScroll = true;
            cmdLineControl.AutoScrollMinSize = new System.Drawing.Size(38, 29);
            cmdLineControl.BackColor = System.Drawing.Color.White;
            cmdLineControl.BackColorIconPane = System.Drawing.Color.Yellow;
            cmdLineControl.BackColorLineNum = System.Drawing.Color.FromArgb(224, 224, 224);
            cmdLineControl.BackColorSelection = System.Drawing.Color.FromArgb(122, 150, 223);
            cmdLineControl.Cursor = System.Windows.Forms.Cursors.IBeam;
            cmdLineControl.Dock = System.Windows.Forms.DockStyle.Fill;
            cmdLineControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            cmdLineControl.ForeColor = System.Drawing.Color.Black;
            cmdLineControl.ForeColorLineNum = System.Drawing.Color.FromArgb(64, 64, 64);
            cmdLineControl.Location = new System.Drawing.Point(4, 3);
            cmdLineControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmdLineControl.Name = "cmdLineControl";
            cmdLineControl.ShowIconPane = false;
            cmdLineControl.ShowLineNumbers = true;
            cmdLineControl.Size = new System.Drawing.Size(930, 397);
            cmdLineControl.TabIndex = 3;
            // 
            // toolStripMainWindow
            // 
            toolStripMainWindow.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStripMainWindow.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripButtonNewScript, toolStripButtonOpenScript, toolStripButtoSaveScript, toolStripButtonRunScript, toolStripSeparator1, toolStripDatabaseConnectionsDropDownButton, toolStripSeparator2 });
            toolStripMainWindow.Location = new System.Drawing.Point(0, 24);
            toolStripMainWindow.Name = "toolStripMainWindow";
            toolStripMainWindow.Size = new System.Drawing.Size(946, 27);
            toolStripMainWindow.TabIndex = 2;
            toolStripMainWindow.Text = "toolStrip1";
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
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, viewToolStripMenuItem, toolsToolStripMenuItem, windowsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            menuStrip1.Size = new System.Drawing.Size(946, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { openToolStripMenuItem, openSQLiteToolStripMenuItem, openSqlCEToolStripMenuItem, openConfigDbToolStripMenuItem, openCvsToolStripMenuItem, toolStripMenuItem6, connectToolStripMenuItem, closeConnectionToolStripMenuItem, toolStripMenuItem3, newScriptToolStripMenuItem, openScriptToolStripMenuItem, saveScriptToolStripMenuItem, saveScriptAsToolStripMenuItem, runScriptToolStripMenuItem, toolStripMenuItem2, cancelExecutionToolStripMenuItem, toolStripMenuItem7, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            openToolStripMenuItem.Text = "Open...";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // openSQLiteToolStripMenuItem
            // 
            openSQLiteToolStripMenuItem.Name = "openSQLiteToolStripMenuItem";
            openSQLiteToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            openSQLiteToolStripMenuItem.Text = "Open SQLite...";
            openSQLiteToolStripMenuItem.Click += openSQLiteToolStripMenuItem_Click;
            // 
            // openSqlCEToolStripMenuItem
            // 
            openSqlCEToolStripMenuItem.Name = "openSqlCEToolStripMenuItem";
            openSqlCEToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            openSqlCEToolStripMenuItem.Text = "Open SqlCE...";
            openSqlCEToolStripMenuItem.Click += openSqlCEToolStripMenuItem_Click;
            // 
            // openConfigDbToolStripMenuItem
            // 
            openConfigDbToolStripMenuItem.Name = "openConfigDbToolStripMenuItem";
            openConfigDbToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            openConfigDbToolStripMenuItem.Text = "Open config db";
            openConfigDbToolStripMenuItem.Click += openConfigDbToolStripMenuItem_Click;
            // 
            // openCvsToolStripMenuItem
            // 
            openCvsToolStripMenuItem.Name = "openCvsToolStripMenuItem";
            openCvsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            openCvsToolStripMenuItem.Text = "Open csv...";
            openCvsToolStripMenuItem.Click += openCsvToolStripMenuItem_Click;
            // 
            // toolStripMenuItem6
            // 
            toolStripMenuItem6.Name = "toolStripMenuItem6";
            toolStripMenuItem6.Size = new System.Drawing.Size(163, 6);
            // 
            // connectToolStripMenuItem
            // 
            connectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripMenuItem1, newToolStripMenuItem });
            connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            connectToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            connectToolStripMenuItem.Text = "Connect";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(104, 6);
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            newToolStripMenuItem.Text = "New...";
            newToolStripMenuItem.Click += newToolStripMenuItem_Click;
            // 
            // closeConnectionToolStripMenuItem
            // 
            closeConnectionToolStripMenuItem.Name = "closeConnectionToolStripMenuItem";
            closeConnectionToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            closeConnectionToolStripMenuItem.Text = "Close connection";
            closeConnectionToolStripMenuItem.Click += closeConnectionToolStripMenuItem_Click;
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new System.Drawing.Size(163, 6);
            // 
            // newScriptToolStripMenuItem
            // 
            newScriptToolStripMenuItem.Name = "newScriptToolStripMenuItem";
            newScriptToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            newScriptToolStripMenuItem.Text = "New script";
            newScriptToolStripMenuItem.Click += newScriptToolStripMenuItem_Click;
            // 
            // openScriptToolStripMenuItem
            // 
            openScriptToolStripMenuItem.Name = "openScriptToolStripMenuItem";
            openScriptToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            openScriptToolStripMenuItem.Text = "Open script...";
            openScriptToolStripMenuItem.Click += openScriptToolStripMenuItem_Click;
            // 
            // saveScriptToolStripMenuItem
            // 
            saveScriptToolStripMenuItem.Name = "saveScriptToolStripMenuItem";
            saveScriptToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            saveScriptToolStripMenuItem.Text = "Save script";
            saveScriptToolStripMenuItem.Click += saveScriptToolStripMenuItem_Click;
            // 
            // saveScriptAsToolStripMenuItem
            // 
            saveScriptAsToolStripMenuItem.Name = "saveScriptAsToolStripMenuItem";
            saveScriptAsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            saveScriptAsToolStripMenuItem.Text = "Save script as...";
            saveScriptAsToolStripMenuItem.Click += saveScriptAsToolStripMenuItem_Click;
            // 
            // runScriptToolStripMenuItem
            // 
            runScriptToolStripMenuItem.Name = "runScriptToolStripMenuItem";
            runScriptToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            runScriptToolStripMenuItem.Text = "Run script...";
            runScriptToolStripMenuItem.Click += runScriptToolStripMenuItem_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new System.Drawing.Size(163, 6);
            // 
            // cancelExecutionToolStripMenuItem
            // 
            cancelExecutionToolStripMenuItem.Name = "cancelExecutionToolStripMenuItem";
            cancelExecutionToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            cancelExecutionToolStripMenuItem.Text = "Cancel execution";
            cancelExecutionToolStripMenuItem.Click += cancelExecutionToolStripMenuItem_Click;
            // 
            // toolStripMenuItem7
            // 
            toolStripMenuItem7.Name = "toolStripMenuItem7";
            toolStripMenuItem7.Size = new System.Drawing.Size(163, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { cutToolStripMenuItem, copyToolStripMenuItem, pasteToolStripMenuItem, formatQueryToolStripMenuItem, copyConnectionStringToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            editToolStripMenuItem.Text = "Edit";
            // 
            // cutToolStripMenuItem
            // 
            cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            cutToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            cutToolStripMenuItem.Text = "Cut";
            cutToolStripMenuItem.Click += cutToolStripMenuItem_Click;
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            copyToolStripMenuItem.Text = "Copy";
            copyToolStripMenuItem.Click += copyToolStripMenuItem_Click;
            // 
            // pasteToolStripMenuItem
            // 
            pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            pasteToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            pasteToolStripMenuItem.Text = "Paste";
            pasteToolStripMenuItem.Click += pasteToolStripMenuItem_Click;
            // 
            // formatQueryToolStripMenuItem
            // 
            formatQueryToolStripMenuItem.Name = "formatQueryToolStripMenuItem";
            formatQueryToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            formatQueryToolStripMenuItem.Text = "Format Query...";
            formatQueryToolStripMenuItem.Click += formatQueryToolStripMenuItem_Click;
            // 
            // copyConnectionStringToolStripMenuItem
            // 
            copyConnectionStringToolStripMenuItem.Name = "copyConnectionStringToolStripMenuItem";
            copyConnectionStringToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            copyConnectionStringToolStripMenuItem.Text = "Copy ConnectionString";
            copyConnectionStringToolStripMenuItem.Click += copyConnectionStringToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { displayFilterRowToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            viewToolStripMenuItem.Text = "View";
            // 
            // displayFilterRowToolStripMenuItem
            // 
            displayFilterRowToolStripMenuItem.CheckOnClick = true;
            displayFilterRowToolStripMenuItem.Name = "displayFilterRowToolStripMenuItem";
            displayFilterRowToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            displayFilterRowToolStripMenuItem.Text = "Display Filter Row";
            displayFilterRowToolStripMenuItem.CheckedChanged += displayFilterRowToolStripMenuItem_CheckedChanged;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { aliasesToolStripMenuItem, autoQueriesToolStripMenuItem, uploadSBDZipFileToolStripMenuItem, logSearchToolStripMenuItem, logImportToolStripMenuItem, generatePasswordToolStripMenuItem, generateDataToolStripMenuItem, importEnumValuesToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // aliasesToolStripMenuItem
            // 
            aliasesToolStripMenuItem.Name = "aliasesToolStripMenuItem";
            aliasesToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            aliasesToolStripMenuItem.Text = "Aliases...";
            aliasesToolStripMenuItem.Click += aliasesToolStripMenuItem_Click;
            // 
            // autoQueriesToolStripMenuItem
            // 
            autoQueriesToolStripMenuItem.Name = "autoQueriesToolStripMenuItem";
            autoQueriesToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            autoQueriesToolStripMenuItem.Text = "Auto Queries...";
            autoQueriesToolStripMenuItem.Click += autoQueriesToolStripMenuItem_Click;
            // 
            // uploadSBDZipFileToolStripMenuItem
            // 
            uploadSBDZipFileToolStripMenuItem.Name = "uploadSBDZipFileToolStripMenuItem";
            uploadSBDZipFileToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            uploadSBDZipFileToolStripMenuItem.Text = "Upload SBD zip file...";
            uploadSBDZipFileToolStripMenuItem.Click += uploadSBDZipFileToolStripMenuItem_Click;
            // 
            // logSearchToolStripMenuItem
            // 
            logSearchToolStripMenuItem.Name = "logSearchToolStripMenuItem";
            logSearchToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            logSearchToolStripMenuItem.Text = "LogSearch...";
            logSearchToolStripMenuItem.Click += logSearchToolStripMenuItem_Click;
            // 
            // logImportToolStripMenuItem
            // 
            logImportToolStripMenuItem.Name = "logImportToolStripMenuItem";
            logImportToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            logImportToolStripMenuItem.Text = "LogImport...";
            logImportToolStripMenuItem.Click += logImportToolStripMenuItem_Click;
            // 
            // generatePasswordToolStripMenuItem
            // 
            generatePasswordToolStripMenuItem.Name = "generatePasswordToolStripMenuItem";
            generatePasswordToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            generatePasswordToolStripMenuItem.Text = "Generate password";
            generatePasswordToolStripMenuItem.Click += generatePasswordToolStripMenuItem_Click;
            // 
            // generateDataToolStripMenuItem
            // 
            generateDataToolStripMenuItem.Name = "generateDataToolStripMenuItem";
            generateDataToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            generateDataToolStripMenuItem.Text = "Generate data";
            generateDataToolStripMenuItem.Click += generateDataToolStripMenuItem_Click;
            // 
            // windowsToolStripMenuItem
            // 
            windowsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { dataTabsToolStripMenuItem });
            windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
            windowsToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            windowsToolStripMenuItem.Text = "Windows";
            // 
            // dataTabsToolStripMenuItem
            // 
            dataTabsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { newToolStripMenuItem1 });
            dataTabsToolStripMenuItem.Name = "dataTabsToolStripMenuItem";
            dataTabsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            dataTabsToolStripMenuItem.Text = "Data tabs";
            // 
            // newToolStripMenuItem1
            // 
            newToolStripMenuItem1.Name = "newToolStripMenuItem1";
            newToolStripMenuItem1.Size = new System.Drawing.Size(98, 22);
            newToolStripMenuItem1.Text = "New";
            newToolStripMenuItem1.Click += newToolStripMenuItem1_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { aboutToolStripMenuItem, helpToolStripMenuItem1 });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem1
            // 
            helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            helpToolStripMenuItem1.Size = new System.Drawing.Size(108, 22);
            helpToolStripMenuItem1.Text = "Help...";
            helpToolStripMenuItem1.Click += helpToolStripMenuItem1_Click;
            // 
            // sqlOutput
            // 
            sqlOutput.DisplayFilterRow = false;
            sqlOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            sqlOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            sqlOutput.Location = new System.Drawing.Point(0, 0);
            sqlOutput.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            sqlOutput.Name = "sqlOutput";
            sqlOutput.SelectedIndex = 0;
            sqlOutput.ShowToolTips = true;
            sqlOutput.Size = new System.Drawing.Size(946, 237);
            sqlOutput.TabIndex = 0;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripMessageLabel, visibleRowsToolStripStatusLabel, toolStripProgressBar, toolStripStatusLabelMetaData });
            statusStrip1.Location = new System.Drawing.Point(0, 724);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            statusStrip1.Size = new System.Drawing.Size(946, 24);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripMessageLabel
            // 
            toolStripMessageLabel.Name = "toolStripMessageLabel";
            toolStripMessageLabel.Size = new System.Drawing.Size(738, 19);
            toolStripMessageLabel.Spring = true;
            toolStripMessageLabel.Text = "Disconnected";
            toolStripMessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // visibleRowsToolStripStatusLabel
            // 
            visibleRowsToolStripStatusLabel.Name = "visibleRowsToolStripStatusLabel";
            visibleRowsToolStripStatusLabel.Size = new System.Drawing.Size(13, 19);
            visibleRowsToolStripStatusLabel.Text = "0";
            visibleRowsToolStripStatusLabel.ToolTipText = "Number of rows displayed";
            // 
            // toolStripProgressBar
            // 
            toolStripProgressBar.Name = "toolStripProgressBar";
            toolStripProgressBar.Size = new System.Drawing.Size(117, 18);
            // 
            // toolStripStatusLabelMetaData
            // 
            toolStripStatusLabelMetaData.Name = "toolStripStatusLabelMetaData";
            toolStripStatusLabelMetaData.Size = new System.Drawing.Size(59, 19);
            toolStripStatusLabelMetaData.Text = "No Cache";
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
            // importEnumValuesToolStripMenuItem
            // 
            importEnumValuesToolStripMenuItem.Name = "importEnumValuesToolStripMenuItem";
            importEnumValuesToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            importEnumValuesToolStripMenuItem.Text = "Import enum values...";
            importEnumValuesToolStripMenuItem.Click += importEnumValuesToolStripMenuItem_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(946, 748);
            Controls.Add(splitContainer1);
            Controls.Add(statusStrip1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MainWindow";
            Text = "Sql Studio";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tabControlMainDocs.ResumeLayout(false);
            tpMainInput.ResumeLayout(false);
            toolStripMainWindow.ResumeLayout(false);
            toolStripMainWindow.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            cmScriptTabs.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.ToolStripMenuItem copyConnectionStringToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generatePasswordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importEnumValuesToolStripMenuItem;
    }
}

