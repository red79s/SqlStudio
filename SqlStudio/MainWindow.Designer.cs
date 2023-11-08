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
            toolStripMainWindow = new System.Windows.Forms.ToolStrip();
            menuStripTop = new System.Windows.Forms.MenuStrip();
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
            logSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            generatePasswordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            generateDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            importEnumValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            windowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            dataTabsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            newDataTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            cmScriptTabs = new System.Windows.Forms.ContextMenuStrip(components);
            closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tabControlDatabaseConnections = new DatabaseConnectionTabControl();
            formatTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            menuStripTop.SuspendLayout();
            cmScriptTabs.SuspendLayout();
            SuspendLayout();
            // 
            // toolStripMainWindow
            // 
            toolStripMainWindow.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStripMainWindow.Location = new System.Drawing.Point(0, 24);
            toolStripMainWindow.Name = "toolStripMainWindow";
            toolStripMainWindow.Size = new System.Drawing.Size(946, 25);
            toolStripMainWindow.TabIndex = 2;
            toolStripMainWindow.Text = "toolStrip1";
            // 
            // menuStripTop
            // 
            menuStripTop.ImageScalingSize = new System.Drawing.Size(20, 20);
            menuStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, viewToolStripMenuItem, toolsToolStripMenuItem, windowsToolStripMenuItem, helpToolStripMenuItem });
            menuStripTop.Location = new System.Drawing.Point(0, 0);
            menuStripTop.Name = "menuStripTop";
            menuStripTop.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            menuStripTop.Size = new System.Drawing.Size(946, 24);
            menuStripTop.TabIndex = 1;
            menuStripTop.Text = "menuStrip1";
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
            editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { cutToolStripMenuItem, copyToolStripMenuItem, pasteToolStripMenuItem, formatQueryToolStripMenuItem, formatTextToolStripMenuItem, copyConnectionStringToolStripMenuItem });
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
            toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { aliasesToolStripMenuItem, autoQueriesToolStripMenuItem, logSearchToolStripMenuItem, generatePasswordToolStripMenuItem, generateDataToolStripMenuItem, importEnumValuesToolStripMenuItem });
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
            // logSearchToolStripMenuItem
            // 
            logSearchToolStripMenuItem.Name = "logSearchToolStripMenuItem";
            logSearchToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            logSearchToolStripMenuItem.Text = "LogSearch...";
            logSearchToolStripMenuItem.Click += logSearchToolStripMenuItem_Click;
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
            // importEnumValuesToolStripMenuItem
            // 
            importEnumValuesToolStripMenuItem.Name = "importEnumValuesToolStripMenuItem";
            importEnumValuesToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            importEnumValuesToolStripMenuItem.Text = "Import enum values...";
            importEnumValuesToolStripMenuItem.Click += importEnumValuesToolStripMenuItem_Click;
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
            dataTabsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { newDataTabToolStripMenuItem });
            dataTabsToolStripMenuItem.Name = "dataTabsToolStripMenuItem";
            dataTabsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            dataTabsToolStripMenuItem.Text = "Data tabs";
            // 
            // newDataTabToolStripMenuItem
            // 
            newDataTabToolStripMenuItem.Name = "newDataTabToolStripMenuItem";
            newDataTabToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            newDataTabToolStripMenuItem.Text = "New";
            newDataTabToolStripMenuItem.Click += NewDataTabToolStripMenuItem_Click;
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
            // tabControlDatabaseConnections
            // 
            tabControlDatabaseConnections.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tabControlDatabaseConnections.ConfigDataStore = null;
            tabControlDatabaseConnections.Location = new System.Drawing.Point(0, 52);
            tabControlDatabaseConnections.Name = "tabControlDatabaseConnections";
            tabControlDatabaseConnections.SelectedIndex = 0;
            tabControlDatabaseConnections.Size = new System.Drawing.Size(946, 697);
            tabControlDatabaseConnections.TabIndex = 3;
            // 
            // formatTextToolStripMenuItem
            // 
            formatTextToolStripMenuItem.Name = "formatTextToolStripMenuItem";
            formatTextToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            formatTextToolStripMenuItem.Text = "Format Text...";
            formatTextToolStripMenuItem.Click += formatTextToolStripMenuItem_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(946, 748);
            Controls.Add(tabControlDatabaseConnections);
            Controls.Add(toolStripMainWindow);
            Controls.Add(menuStripTop);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStripTop;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MainWindow";
            Text = "Sql Studio";
            menuStripTop.ResumeLayout(false);
            menuStripTop.PerformLayout();
            cmScriptTabs.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStripMainWindow;
        private System.Windows.Forms.MenuStrip menuStripTop;
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
        private System.Windows.Forms.ToolStripMenuItem newDataTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem newScriptToolStripMenuItem;
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
        private System.Windows.Forms.ToolStripMenuItem runScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatQueryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSqlCEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cancelExecutionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem openConfigDbToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoQueriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logSearchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openCvsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyConnectionStringToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generatePasswordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importEnumValuesToolStripMenuItem;
        private DatabaseConnectionTabControl tabControlDatabaseConnections;
        private System.Windows.Forms.ToolStripMenuItem formatTextToolStripMenuItem;
    }
}

