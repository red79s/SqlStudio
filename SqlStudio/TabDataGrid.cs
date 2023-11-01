using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using SqlExecute;
using System.Drawing;
using System.Data.Common;
using System.IO;
using CfgDataStore;
using System.Text.RegularExpressions;
using Common;
using System.Linq;
using SqlStudio.AutoLayoutForm;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SqlStudio
{
    public class TabDataGrid : DataGridView
    {
        public delegate void UpdatedResultsDelegate(object sender, int rows, string message);
        public event UpdatedResultsDelegate UpdatedResults;

        private SqlResult _sqlResult = null;
        private DataView _view = null;
        private ToolStripMenuItem _dynamicDataMenuItem;
        private ToolStripMenuItem _generatedDataMenuItem;
        private ConfigDataStore _configDataStore;
        private readonly IExecuteQueryCallback _executeQueryCallback;
        private readonly IDatabaseSchemaInfo _databaseSchemaInfo;
        private readonly IDatabaseKeywordEscape _databaseKeywordEscape;
        private readonly IColumnValueDescriptionProvider _columnMetadataInfo;

        public TabDataGrid(ConfigDataStore configDataStore, 
            IExecuteQueryCallback executeQueryCallback, 
            IDatabaseSchemaInfo databaseSchemaInfo, 
            IDatabaseKeywordEscape databaseKeywordEscape, 
            IColumnValueDescriptionProvider columnMetadataInfo)
        {
            _configDataStore = configDataStore;
            _executeQueryCallback = executeQueryCallback;
            _databaseSchemaInfo = databaseSchemaInfo;
            _databaseKeywordEscape = databaseKeywordEscape;
            _columnMetadataInfo = columnMetadataInfo;
            BackgroundColor = Color.WhiteSmoke;
            ContextMenuStrip = new ContextMenuStrip();

            ToolStripMenuItem miCopy = new ToolStripMenuItem("Copy");
            miCopy.Click += miCopy_Click;
            ContextMenuStrip.Items.Add(miCopy);

            var miEdit = new ToolStripMenuItem("Edit");
            miEdit.Click += MiEdit_Click;
            ContextMenuStrip.Items.Add(miEdit);

            ToolStripMenuItem miSave = new ToolStripMenuItem("Save");
            miSave.Click += miSave_Click;
            ContextMenuStrip.Items.Add(miSave);

            ToolStripMenuItem miFill = new ToolStripMenuItem("Fill");
            miFill.ShortcutKeys = Keys.F7;
            miFill.Click += miFill_Click;
            ContextMenuStrip.Items.Add(miFill);

            ToolStripMenuItem miNewRow = new ToolStripMenuItem("New Row");
            miNewRow.Click += miNewRow_Click;
            ContextMenuStrip.Items.Add(miNewRow);

            ToolStripMenuItem miGetTitles = new ToolStripMenuItem("Get Titles");
            miGetTitles.Click += miGetTitles_Click;
            ContextMenuStrip.Items.Add(miGetTitles);

            ToolStripMenuItem miGetColumnInfo = new ToolStripMenuItem("Get Column Info");
            miGetColumnInfo.Click += GetColumnInfo_MenuItemClick;
            ContextMenuStrip.Items.Add(miGetColumnInfo);

            var miBlob = new ToolStripMenuItem("Blob");
            ContextMenuStrip.Items.Add(miBlob);

            ToolStripMenuItem miGetBlobAsText = new ToolStripMenuItem("Get Blob As Text");
            miGetBlobAsText.Click += miGetBlobAsText_Click;
            miBlob.DropDownItems.Add(miGetBlobAsText);

            ToolStripMenuItem miGetBlobAsImage = new ToolStripMenuItem("Get Blob As Image");
            miGetBlobAsImage.Click += miGetBlobAsImage_Click;
            miBlob.DropDownItems.Add(miGetBlobAsImage);

            ToolStripMenuItem miGetBlobAsBytes = new ToolStripMenuItem("Get Blob As Bytes");
            miGetBlobAsBytes.Click += miGetBlobAsBytes_Click;
            miBlob.DropDownItems.Add(miGetBlobAsBytes);

            ToolStripMenuItem miGetBlobAsFile = new ToolStripMenuItem("Get Blob As File");
            miGetBlobAsFile.Click += miGetBlobAsFile_Click;
            miBlob.DropDownItems.Add(miGetBlobAsFile);

            ToolStripMenuItem miUploadFileToBlob = new ToolStripMenuItem("Upload File To Blob");
            miUploadFileToBlob.Click += miUploadFileToBlob_Click;
            miBlob.DropDownItems.Add(miUploadFileToBlob);

            var miGraph = new ToolStripMenuItem("Graph");
            ContextMenuStrip.Items.Add(miGraph);

            var miCreateLineGraphFromData = new ToolStripMenuItem("Create line graph");
            miCreateLineGraphFromData.Click += MiCreateLineGraphFromData_Click;
            miGraph.DropDownItems.Add(miCreateLineGraphFromData);

            var scriptDropdownMenu = new ToolStripMenuItem("Script");
            ContextMenuStrip.Items.Add(scriptDropdownMenu);

            var miCreateTableScript = new ToolStripMenuItem("Create Table Script");
            miCreateTableScript.Click += miCreateTableScript_Click;
            scriptDropdownMenu.DropDownItems.Add(miCreateTableScript);

            ToolStripMenuItem miCreateScript = new ToolStripMenuItem("Create Script");
            miCreateScript.Click += miCreateScript_Click;
            scriptDropdownMenu.DropDownItems.Add(miCreateScript);

            ToolStripMenuItem miCreateScriptToFile = new ToolStripMenuItem("Create Script To File...");
            miCreateScriptToFile.Click += miCreateScriptToFile_Click;
            scriptDropdownMenu.DropDownItems.Add(miCreateScriptToFile);

            ToolStripMenuItem miCreateScriptWithCode = new ToolStripMenuItem("Create Script Code");
            miCreateScriptWithCode.Click += miCreateScriptWithCode_Click;
            scriptDropdownMenu.DropDownItems.Add(miCreateScriptWithCode);

            _generatedDataMenuItem = new ToolStripMenuItem("Generated");
            ContextMenuStrip.Items.Add(_generatedDataMenuItem);

            _dynamicDataMenuItem = new ToolStripMenuItem("Auto Queries");

            if (_configDataStore != null)
            {
                foreach (var autoQuery in _configDataStore.GetAutoQueries())
                {
                    var menuItem = new ToolStripMenuItem(autoQuery.Description);
                    menuItem.Click += AutoQueryMenuItem_Click;
                    menuItem.Tag = autoQuery;
                    _dynamicDataMenuItem.DropDownItems.Add(menuItem);
                }
            }
            ContextMenuStrip.Items.Add(_dynamicDataMenuItem);

            var miCreateCrudCode = new ToolStripMenuItem("Create CRUD Template Code");
            miCreateCrudCode.Click += MiCreateCrudCode_Click;
            ContextMenuStrip.Items.Add(miCreateCrudCode);

            var miShowText = new ToolStripMenuItem("Show Text");
            miShowText.Click += MiShowTextOnClick;
            ContextMenuStrip.Items.Add(miShowText);

            var miFindText = new ToolStripMenuItem("Find...");
            miFindText.Click += MiFindTextOnClick;
            ContextMenuStrip.Items.Add(miFindText);

            var miFindColumn = new ToolStripMenuItem("Find Column...");
            miFindColumn.Click += MiFindColumnOnClick;
            ContextMenuStrip.Items.Add(miFindColumn);

            var miFindTimeDiff = new ToolStripMenuItem("Get Time diffs");
            miFindTimeDiff.Click += MiFindTimeDiffOnClick;
            ContextMenuStrip.Items.Add(miFindTimeDiff);

            ContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(ContextMenuStrip_Opening);
        }

        private void MiEdit_Click(object sender, EventArgs e)
        {
            var cells = SelectedCells;
            var fieldCells = new Dictionary<FieldInfo, DataGridViewCell>();
            var fieldInfos = new SortedDictionary<int, List<FieldInfo>>();
            
            foreach (DataGridViewCell cell in cells)
            {
                var fieldInfo = new FieldInfo
                {
                    ColumnIndex = cell.ColumnIndex,
                    Name = cell.OwningColumn.Name,
                    Value = cell.Value,
                    ValueType = cell.ValueType
                };

                if (fieldInfos.ContainsKey(cell.RowIndex))
                {
                    var rowColumns = fieldInfos[cell.RowIndex];
                    rowColumns.Add(fieldInfo);
                    fieldInfos[cell.RowIndex] = rowColumns.OrderBy(x => x.ColumnIndex).ToList();
                }
                else
                {
                    fieldInfos.Add(cell.RowIndex, new List<FieldInfo> { fieldInfo });
                }
                fieldCells.Add(fieldInfo, cell);
            }

            var form = new AutoLayoutForm.AutoLayoutForm(fieldInfos);
            if (form.ShowDialog() == DialogResult.OK)
            {
                foreach(var fieldCell in fieldCells)
                {
                    fieldCell.Value.Value = fieldCell.Key.Value;
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.N))
            {
                ProcessDownKey(Keys.Down);

                if (SelectedCells != null && SelectedCells.Count == 1)
                {
                    SelectedCells[0].Value = DateTime.Now;
                }

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public SqlResult GetLastSqlResult()
        {
            return _sqlResult;
        }

        private void MiCreateLineGraphFromData_Click(object sender, EventArgs e)
        {
            var rows = GetSelectedRows();
            if (rows.Count < 1)
                return;

            var data = new GraphData("abc");
            var defCells = rows[0];
            defCells.Sort(delegate (DataGridViewCell c1, DataGridViewCell c2) { return c1.ColumnIndex.CompareTo(c2.ColumnIndex); });
            for (int i = 0; i < defCells.Count; i++)
            {
                DataGridViewCell cell = defCells[i];
                if (i < 1)
                {
                    data.XLabel = cell.OwningColumn.Name;
                    data.XType = cell.ValueType;
                }
                else
                {
                    data.YLabels.Add(cell.OwningColumn.Name);
                    data.YTypes.Add(cell.ValueType);
                }
            }

            data.YMin = double.MaxValue;
            data.YMax = double.MinValue;
            double autoXValue = 0;
            var onlyOneColumn = defCells.Count < 2;

            foreach (List<DataGridViewCell> cells in rows)
            {
                cells.Sort(delegate (DataGridViewCell c1, DataGridViewCell c2) { return c1.ColumnIndex.CompareTo(c2.ColumnIndex); });
                var dataPoint = new GraphDataPoint();
                bool haveData = true;
                for (int i = 0; i < cells.Count; i++)
                {
                    DataGridViewCell cell = cells[i];
                    if (i < 1)
                    {
                        if (onlyOneColumn)
                        {
                            dataPoint.XData = autoXValue;
                            autoXValue++;
                            dataPoint.YData.Add(cell.Value);
                        }
                        else
                        { 
                            dataPoint.XData = cell.Value; 
                        }
                    }
                    else
                    {
                        if (cell.Value != null && 
                            cell.ValueType != typeof(DateTime))
                        {
                            var val = Convert.ToDouble(cell.Value);
                            if (val > data.YMax)
                                data.YMax = val;
                            if (val < data.YMin)
                                data.YMin = val;
                        }

                        dataPoint.YData.Add(cell.Value);
                    }

                    if (cell.Value == null)
                        haveData = false;
                }
                if (haveData)
                {
                    data.Data.Add(dataPoint);
                }
            }

            SqlOutputTabContainer tabContainer = (SqlOutputTabContainer)Parent.Parent.Parent;
            var label = $"Graph: {data.XLabel} vs ";
            for (int i = 0; i < data.YLabels.Count; i++)
            {
                label += data.YLabels[i];
                if (i < data.YLabels.Count - 1)
                {
                    label += ", ";
                }
            }
            tabContainer.CreateNewGraphTab(label, data);
        }

        private void GeneratedQueryMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedRows.Count < 1 && SelectedCells.Count < 1)
                return;

            var menuItem = sender as ToolStripMenuItem;
            if (menuItem == null)
                return;

            var autoQuery = menuItem.Tag as AutoQuery;
            if (autoQuery == null)
                return;

            var query = autoQuery.Command;
            var substitutedQuery = SubstitueColumnValues(query);
            _executeQueryCallback.ExecuteQuery(substitutedQuery, true, autoQuery.Description);
        }

        private void AutoQueryMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedRows.Count < 1 && SelectedCells.Count < 1)
                return;

            var menuItem = sender as ToolStripMenuItem;
            if (menuItem == null)
                return;

            var autoQuery = menuItem.Tag as AutoQuery;
            if (autoQuery == null)
                return;

            var query = autoQuery.Command;
            var substitutedQuery = SubstitueColumnValues(query);
            _executeQueryCallback.ExecuteQuery(substitutedQuery, true, autoQuery.Description);
        }

        private string SubstitueColumnValues(string query)
        {
            var matchEvalutator = new MatchEvaluator(ColumnMatchEveluator);
            var substQuery = Regex.Replace(query, "{[a-zA-Z0-9_]+}", matchEvalutator);
            return substQuery;
        }

        private string ColumnMatchEveluator(Match m)
        {
            if (SelectedRows.Count < 1 && SelectedCells.Count < 1)
                return "";

            if (m.Value == null || m.Value.Length < 2)
                return "";

            var columnName = m.Value.Substring(1, m.Value.Length - 2);

            var dbStrValues = new List<string>();
            foreach (DataGridViewCell selectedCell in SelectedCells)
            {
                DataGridViewRow row = selectedCell.OwningRow;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.OwningColumn.Name.Equals(columnName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var dbStrValue = GetDbStringValue(cell.ValueType, cell.Value, false);
                        if (!dbStrValues.Contains(dbStrValue))
                        {
                            dbStrValues.Add(dbStrValue);
                        }
                    }
                }
            }

            if (dbStrValues.Count == 0)
            {
                return "is null";
            }
            else if (dbStrValues.Count == 1)
            {
                return $"{dbStrValues[0]}";
            }
            else
            {
                return $"{String.Join(", ", dbStrValues)}";
            }
        }
        private void MiFindTimeDiffOnClick(object sender, EventArgs eventArgs)
        {
            if (SelectedCells == null)
                return;
            List<long> diffs = new List<long>();
            double min = double.MaxValue, max = double.MinValue;
            DateTime prev = DateTime.MinValue;
            DataGridViewCell maxCell = null;
            if (SelectedCells.Count == 1)
            {
                var colIndex = SelectedCells[0].ColumnIndex;
                foreach (DataGridViewRow row in Rows)
                {
                    if (row.Cells[colIndex].ValueType != typeof(DateTime) ||
                        row.Cells[colIndex].Value == null)
                        continue;

                    var val = (DateTime)row.Cells[colIndex].Value;
                    if (prev == DateTime.MinValue)
                    {
                        prev = val;
                        continue;
                    }

                    var diff = val.Subtract(prev).TotalMilliseconds;
                    if (diff < min)
                        min = diff;
                    if (diff > max)
                    {
                        maxCell = row.Cells[colIndex];
                        max = diff;
                    }
                        

                    prev = val;
                }
            }
            else
            {
                for (int i = SelectedCells.Count - 1; i >= 0; i--)
                {
                    DataGridViewCell selectedCell = SelectedCells[i];
                    if (selectedCell.ValueType != typeof (DateTime))
                        continue;

                    var val = (DateTime)selectedCell.Value;
                    if (prev == DateTime.MinValue)
                    {
                        prev = val;
                        continue;
                    }

                    var diff = val.Subtract(prev).TotalMilliseconds;
                    if (diff < min)
                        min = diff;
                    if (diff > max)
                    {
                        maxCell = selectedCell;
                        max = diff;
                    }

                    prev = val;
                }
            }

            if (maxCell != null)
            {
                CurrentCell = maxCell;
                MessageBox.Show(string.Format("Min: {0}ms, Max: {1}ms", min, max));
            }
            else
            {
                MessageBox.Show("not able to do diff");
            }
        }

        public void FindText(string text, bool directionDown)
        {
            SearchText = text;
            FindNext(directionDown);
        }

        private void MiFindTextOnClick(object sender, EventArgs eventArgs)
        {
            FindDialog fd = new FindDialog();
            fd.StartPosition = FormStartPosition.CenterParent;
			if (fd.ShowDialog() == DialogResult.OK)
            {
                SearchText = fd.SearchText;
                FindNext(true);
            }
        }

		private void MiFindColumnOnClick(object sender, EventArgs eventArgs)
        {
			FindDialog fd = new FindDialog();
            fd.StartPosition = FormStartPosition.CenterParent;
			if (fd.ShowDialog() == DialogResult.OK)
			{
                foreach (DataGridViewColumn col in Columns)
                {
                    if (col.HeaderText.Contains(fd.SearchText, StringComparison.CurrentCultureIgnoreCase))
                    {
                        FirstDisplayedScrollingColumnIndex = col.Index;
                        return;
                    }
                }
				MessageBox.Show($"No column name containing: {fd.SearchText} is found", "Not found", MessageBoxButtons.OK);
			}
		}

		private string SearchText { get; set; }

        private void FindNext(bool directionDown)
        {
            if (string.IsNullOrEmpty(SearchText))
                return;
            if (CurrentCell == null)
            {
                if (Rows.Count > 0 && Rows[0].Cells.Count > 0)
                {
                    if (directionDown)
                    {
                        CurrentCell = Rows[0].Cells[0];
                    }
                    else
                    {
                        CurrentCell = Rows[Rows.Count - 1].Cells[0];
                    }
                }
                else
                {
                    return;
                }
            }

            if (directionDown)
            {
                for (int i = CurrentCell.RowIndex; i < Rows.Count; i++)
                {
                    for (int c = 0; c < Rows[i].Cells.Count; c++)
                    {
                        if (i == CurrentCell.RowIndex && c <= CurrentCell.ColumnIndex)
                            continue;
                        if (Rows[i].Cells[c].Value == null)
                            continue;

                        string colText = Rows[i].Cells[c].Value.ToString();
                        if (colText.IndexOf(SearchText, 0, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            CurrentCell = Rows[i].Cells[c];
                            if (i < FirstDisplayedScrollingRowIndex || i > (FirstDisplayedScrollingRowIndex + DisplayedRowCount(false)))
                            {
                                FirstDisplayedScrollingRowIndex = i;
                            }
                            FirstDisplayedScrollingColumnIndex = c;
                            return;
                        }
                    }
                }
                CurrentCell = null;
                MessageBox.Show("Search reached end, press F3 to start from top");
                return;
            }
            else
            {
                for (int i = CurrentCell.RowIndex; i >= 0; i--)
                {
                    for (int c = 0; c < Rows[i].Cells.Count; c++)
                    {
                        if (i == CurrentCell.RowIndex && c <= CurrentCell.ColumnIndex)
                            continue;
                        if (Rows[i].Cells[c].Value == null)
                            continue;

                        string colText = Rows[i].Cells[c].Value.ToString();
                        if (colText.IndexOf(SearchText, 0, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            CurrentCell = Rows[i].Cells[c];
                            if (i < FirstDisplayedScrollingRowIndex || i > (FirstDisplayedScrollingRowIndex + DisplayedRowCount(false)))
                            {
                                FirstDisplayedScrollingRowIndex = i;
                            }
                            FirstDisplayedScrollingColumnIndex = c;
                            return;
                        }
                    }
                }
                CurrentCell = null;
                MessageBox.Show("Search reached end, press F3 to start from bottom");
                return;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.D)
            {
                DateTime now = DateTime.Now;
                string value = string.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

                foreach (DataGridViewCell cell in SelectedCells)
                    cell.Value = value;
            }
            else if (e.Shift && e.KeyCode == Keys.F)
            {
                MiFindTextOnClick(this, null);
            }
            else if (e.KeyCode == Keys.F3)
            {
                FindNext(true);
            }
            else
                base.OnKeyDown(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            var ht = HitTest(e.X, e.Y);
            if (ht.RowIndex >= 0 && ht.ColumnIndex >= 0)
            {
                var cell = Rows[ht.RowIndex].Cells[ht.ColumnIndex];
                if (!cell.Selected)
                    cell.Selected = true;
            }
        }

        void miCopy_Click(object sender, EventArgs e)
        {
            if (GetCellCount(DataGridViewElementStates.Selected) > 0)
            {
                try
                {
                    Clipboard.SetDataObject(GetClipboardContent());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Failed to copy content to clipboard", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private List<ColumnDef> GetColumnDefs(List<DataGridViewCell> cells)
        {
            cells.Sort(delegate (DataGridViewCell c1, DataGridViewCell c2) { return c1.ColumnIndex.CompareTo(c2.ColumnIndex); });
            var colDefs = new List<ColumnDef>();
            foreach (var cell in cells)
            {
                var colDef = new ColumnDef { Index = cell.ColumnIndex, Name = cell.OwningColumn.Name, Type = cell.ValueType, Value = cell.Value };
                colDefs.Add(colDef);
            }
            return colDefs;
        }

        private void MiCreateCrudCode_Click(object sender, EventArgs e)
        {
            var rows = GetSelectedRows();
            if (rows.Count < 0)
                return;

            var colls = GetColumnDefs(rows[0]);

            var gen = new CrudGenerator(SqlResult.TableName, colls);

            var code = gen.GenerateEntity() + Environment.NewLine + 
                gen.GenerateSelect() + Environment.NewLine + 
                gen.GenerateInsert() + Environment.NewLine + 
                gen.GenerateUpdate() + Environment.NewLine + 
                gen.GenerateDelete();

            PrintScript(code);
        }
           
        void miCreateTableScript_Click(object sender, EventArgs e)
        {
            var rows = GetSelectedRows();
            if (rows.Count < 1)
            {
                MessageBox.Show("no cells selected");
                return;
            }

            var row = rows[0];

            var createTableStmt = $"CREATE TABLE {SqlResult.TableName} (";
            var firstCol = true;
            foreach (var col in row)
            {
                if (!firstCol)
                {
                    createTableStmt += ", ";
                }

                firstCol = false;
                createTableStmt += $"{col.OwningColumn.Name} {col.ValueType.Name}";
            }
            createTableStmt += ")";

            PrintScript(createTableStmt);
            Clipboard.SetText(createTableStmt);
        }

        void miCreateScript_Click(object sender, EventArgs e)
        {
            List<string> insertStatements = CreateScript();
            //PrintScripts(insertStatements, "{0};" + Environment.NewLine);

            var insertText = "";
            bool firstRow = true;
            foreach (var stmt in insertStatements)
            {
                if (!firstRow)
                {
                    insertText += Environment.NewLine;
                }
                insertText += $"{stmt};";
                firstRow = false;
            }

            if (string.IsNullOrEmpty(insertText))
            {
                MessageBox.Show("no cells selected");
                return;
            }

            Clipboard.SetText(insertText);
        }

        void miCreateScriptToFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Sql script (*.sql)|*.sql|All Files (*.*)|*.*";
            
            sfd.CheckFileExists = true;
            sfd.FileName = SqlResult.TableName + ".sql";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                CreateScript(sfd.FileName);
            }
        }

        void miCreateScriptWithCode_Click(object sender, EventArgs e)
        {
            List<string> insertStatements = CreateScript();
            StringBuilder sbInserts = new StringBuilder();
            for (int i = 0; i < insertStatements.Count; i++)
            {
                if (i > 0)
                {
                    sbInserts.Append("," + Environment.NewLine);
                }
                var index = insertStatements[i].IndexOf(" VALUES(");
                sbInserts.Append(string.Format("\"{0}\" +", insertStatements[i].Substring(0, index + 1)) + Environment.NewLine);
                sbInserts.Append(string.Format("\"{0}\"", insertStatements[i].Substring(index + 1)) + Environment.NewLine);
            }

            PrintScript(sbInserts.ToString());

            List<string> blobCode = CreateBlobInsertCode();
            PrintScripts(blobCode, "{0}" + Environment.NewLine);
        }

        private void PrintScript(string statement)
        {
            ((SqlOutputTabContainer)Parent.Parent.Parent).SetOutputText(statement);
        }

        private void PrintScripts(List<string> statements, string format)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string stmt in statements)
            {
                sb.Append(string.Format(format, stmt));
            }
            ((SqlOutputTabContainer)Parent.Parent.Parent).SetOutputText(sb.ToString());
        }

        private List<List<DataGridViewCell>> GetSelectedRows()
        {
            int currentRowIndex = -1;
            List<DataGridViewCell> selectedCells = new List<DataGridViewCell>();
            foreach (DataGridViewCell cell in SelectedCells)
                selectedCells.Add(cell);
            selectedCells.Sort(delegate(DataGridViewCell c1, DataGridViewCell c2) { return c1.RowIndex.CompareTo(c2.RowIndex); });

            List<List<DataGridViewCell>> rows = new List<List<DataGridViewCell>>();
            foreach (DataGridViewCell cell in selectedCells)
            {
                if (cell.RowIndex > currentRowIndex)
                {
                    rows.Add(new List<DataGridViewCell>());
                    currentRowIndex = cell.RowIndex;
                }

                rows[rows.Count - 1].Add(cell);
            }
            return rows;
        }

        private List<string> CreateScript()
        {
            var rows = GetSelectedRows();

            List<string> insertStatements = new List<string>();
            
            foreach (List<DataGridViewCell> row in rows)
            {
                string stmt = CreateInsertStatment(SqlResult.TableName, row);
                if (stmt != null)
                    insertStatements.Add(stmt);
            }

            return insertStatements;
        }

        private void CreateScript(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);

            int rowsAffected = 0;
            int blobFilesWritten = 0;
            using (StreamWriter file = new StreamWriter(fileName, true))
            {
                var rows = GetSelectedRows();
                foreach (List<DataGridViewCell> row in rows)
                {
                    string stmt = CreateInsertStatment(SqlResult.TableName, row, true);
                    if (stmt != null)
                    {
                        //blob handeling
                        DataGridViewCell blobImage = GetBlobImageCell(row);
                        if (blobImage != null)
                        {
                            byte[] data = GetBlobFromCell(blobImage);
                            string blobId = GetBlobId(GetBlobIdCell(row));
                            if (data != null && blobId != null)
                            {
                                string blobFile = fi.DirectoryName + @"\" + SqlResult.TableName + "_blob_id_" + blobId + ".raw";
                                if (File.Exists(blobFile))
                                    File.Delete(blobFile);

                                blobFilesWritten++;
                                File.WriteAllBytes(blobFile, data);
                            }
                        }

                        rowsAffected++;
                        file.WriteLine(stmt + ";");
                    }
                }
            }
            if (UpdatedResults != null)
            {
                string statusMsg = string.Format("Copyied {0} rows to output file", rowsAffected);
                if (blobFilesWritten > 0)
                    statusMsg += string.Format(", Wrote {0} blob files to same folder", blobFilesWritten);
                UpdatedResults(this, rowsAffected, statusMsg);
            }
        }

        private List<string> CreateBlobInsertCode()
        {
            var rows = GetSelectedRows();
            List<string> insertCode = new List<string>();

            foreach (List<DataGridViewCell> row in rows)
            {
                //blob handeling
                DataGridViewCell blobImage = GetBlobImageCell(row);
                if (blobImage != null)
                {
                    byte[] data = GetBlobFromCell(blobImage);
                    if (data != null)
                    {
                        string blobStrValue = Convert.ToBase64String(data);
                        string blobStrValueFormated = "";
                        int index = 0;
                        while (index < blobStrValue.Length)
                        {
                            if (index + 240 < blobStrValue.Length)
                            {
                                blobStrValueFormated += "\"" + blobStrValue.Substring(index, 240) + "\"";
                                index += 240;
                                if (index < blobStrValue.Length)
                                    blobStrValueFormated += "+" + Environment.NewLine;
                            }
                            else
                            {
                                blobStrValueFormated += "\"" + blobStrValue.Substring(index, blobStrValue.Length - index) + "\"";
                                index = blobStrValue.Length;
                            }
                        }

                        var blobId = GetBlobIdCell(row);
                        if (blobId != null)
                        {
                            string variableName = blobId.OwningColumn.Name + "_" + blobId.Value.ToString();

                            insertCode.Add(string.Format("string {0} = \"{1}\";", variableName, blobStrValueFormated));
                            insertCode.Add(string.Format("DatabaseTest.UpdateBlob(\"{0}\", \"{1}\", \"{2} = {3}\", Convert.FromBase64String({4}));",
                                SqlResult.TableName, blobImage.OwningColumn.Name, blobId.OwningColumn.Name, blobId.Value.ToString(), variableName));
                        }
                    }
                }
            }
            return insertCode;
        }
        
        private string CreateInsertStatment(string tableName, List<DataGridViewCell> cells, bool includeBlobColumns = false)
        {
            cells.Sort(delegate(DataGridViewCell c1, DataGridViewCell c2) { return c1.ColumnIndex.CompareTo(c2.ColumnIndex); });
            Dictionary<string, string> colValues = new Dictionary<string, string>();

            string blobId = null;
            if (includeBlobColumns)
            {
                blobId = GetBlobId(GetBlobIdCell(cells));
            }

            tableName = _databaseKeywordEscape.EscapeObject(tableName);

            for (int i = 0; i < cells.Count; i++)
            {
                string colName = cells[i].OwningColumn.Name;
                colName = _databaseKeywordEscape.EscapeObject(colName);
                Type type = cells[i].ValueType;
                Object value = cells[i].Value;

                if (ExcludeColumnInInsert(type, colName, includeBlobColumns))
                    continue;
                if (!includeBlobColumns && IsBlobColumn(type, colName))
                    continue;
                if (value == null)
                    continue;
                if (value == DBNull.Value)
                    continue;

                string strValue = GetDbStringValue(type, value, true);
                if (IsBlobColumn(type, colName))
                {
                    if (blobId == null)
                        continue;

                    strValue = "@" + tableName + "_blob_id_" + blobId;
                }

                colValues.Add(colName, strValue);
            }

            if (colValues.Count == 0)
                return null;

            StringBuilder insertStmt = new StringBuilder("INSERT INTO " + tableName + " (");
            StringBuilder valuesStmt = new StringBuilder(" VALUES(");

            bool firstCol = true;
            foreach (KeyValuePair<string, string> col in colValues)
            {
                if (!firstCol)
                {
                    insertStmt.Append(",");
                    valuesStmt.Append(",");
                }
                firstCol = false;

                insertStmt.Append(col.Key);
                valuesStmt.Append(col.Value);
            }
            insertStmt.Append(")");
            valuesStmt.Append(")");

            insertStmt.Append(valuesStmt);
            return insertStmt.ToString();
        }

        private string GetDbStringValue(Type type, object value, bool quoteStrings)
        {
            string strValue = value.ToString();
            if (quoteStrings)
            {
                strValue = "'" + value.ToString().Trim().Replace("'", "''") + "'";
            }
            
            if (type == typeof(bool))
            {
                strValue = (bool)value == true ? "1" : "0";
            }
            else if (type == typeof(DateTime))
            {
                DateTime d = DateTime.Now;
                if (value is DateTime)
                {
                    d = (DateTime)value;
                }
                else if (value is string)
                {
                    d = DateTime.Parse((string)value);
                }

                strValue = string.Format("'{0:00}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}'", d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second);
            }
            else if (type == typeof(float) || type == typeof(decimal) || type == typeof(double))
            {
                strValue = value.ToString().Replace(',', '.');
            }
            else if (type == typeof(byte) || type == typeof(int) || type == typeof(long) || type == typeof(short))
            {
                strValue = value.ToString();
            }

            return strValue;
        }

        private DataGridViewCell GetBlobImageCell(List<DataGridViewCell> cells)
        {
            foreach (var cell in cells)
            {
                if (cell.OwningColumn.Name.Equals("blob_image", StringComparison.CurrentCultureIgnoreCase))
                    return cell;
            }
            return null;
        }

        private DataGridViewCell GetBlobIdCell(List<DataGridViewCell> cells)
        {
            foreach (var cell in cells)
            {
                if (cell.OwningColumn.Name.Equals("blob_id", StringComparison.CurrentCultureIgnoreCase))
                    return cell;
                if (cell.OwningColumn.Name.Equals("blob_guid", StringComparison.CurrentCultureIgnoreCase))
                    return cell;
            }
            return null;
        }

        private string GetBlobId(DataGridViewCell cell)
        {
            if (cell != null && cell.Value != null)
                return cell.Value.ToString();
            return null;
        }

        private bool ExcludeColumnInInsert(Type columnType, string columnName, bool allowBlobColumns = false)
        {
            if (!allowBlobColumns && columnType == typeof(byte[]))
                return true;
            return false;
        }

        private bool IsBlobColumn(Type columnType, string columnName)
        {
            if (columnName.Equals("blob_image", StringComparison.CurrentCultureIgnoreCase))
                return true;
            return false;
        }

        void miGetTitles_Click(object sender, EventArgs e)
        {
            int numTitlesFetched = 0;

            DataGridViewCell cell = SelectedCells[0];
            for (int i = 0; i < Rows.Count; i++)
            {
                DataGridViewCell titleCell = Rows[i].Cells[cell.ColumnIndex];
                if (titleCell.Value == null)
                    continue;
                int titleNo = (int)titleCell.Value;
                if (titleNo < 1)
                    continue;

                string cmdText = string.Format("select title from asystitlesen where title_no = {0}", titleNo);
                DbCommand cmd = SqlResult.Connection.CreateCommand();
                cmd.CommandText = cmdText;
                object o = cmd.ExecuteScalar();
                titleCell.ToolTipText = o.ToString();
                numTitlesFetched++;
            }

            if (UpdatedResults != null)
                UpdatedResults(this, numTitlesFetched, "Fetched Agresso titles");
        }

        private void GetColumnInfo_MenuItemClick(object sender, EventArgs e)
        {
            if (SelectedCells == null || SelectedCells.Count == 0)
                return;

            var descriptions = _columnMetadataInfo.GetDescriptionForColumn(_sqlResult.TableName, SelectedCells[0].OwningColumn.Name);
            if (descriptions.Count == 0) 
                return;

            MessageBox.Show(string.Join(Environment.NewLine, descriptions), "Value - Description");
            //foreach (DataGridViewCell cell in SelectedCells)
            //{
            //    if (cell.Value == null)
            //        continue;

            //    cell.ToolTipText = _columnMetadataInfo.GetDescriptionForValue(_sqlResult.TableName, cell.OwningColumn.Name, cell.Value.ToString());
            //}
        }

        private byte[] GetBlobFromCell(DataGridViewCell cell)
        {
            if (cell.ValueType == typeof(byte[]) && cell.Value != DBNull.Value && cell.Value is byte[])
                return (byte[])cell.Value;
            return null;
        }

        List<byte[]> GetBlobsFromSelectedCells()
        {
            if (SelectedCells.Count < 1)
                return null;

            List<byte[]> ret = new List<byte[]>();
            foreach (DataGridViewCell cell in SelectedCells)
            {
                byte[] data = GetBlobFromCell(cell);
                if (data != null)
                    ret.Add((byte[])cell.Value);
            }
            return ret;
        }

        void miUploadFileToBlob_Click(object sender, EventArgs e)
        {
            if (SelectedCells.Count != 1)
                return;

            if (SelectedCells[0].ValueType != typeof(byte[]))
                return;

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                SelectedCells[0].Value = File.ReadAllBytes(ofd.FileName);
            }
        }

        void miGetBlobAsImage_Click(object sender, EventArgs e)
        {
            List<byte[]> blobs = GetBlobsFromSelectedCells();
            SqlOutputTabContainer tabContainer = (SqlOutputTabContainer)Parent.Parent.Parent;
            int i = 1;
            foreach (byte[] blob in blobs)
            {
                try
                {
                    Bitmap bm = (Bitmap)Bitmap.FromStream(new MemoryStream(blob));
                    tabContainer.CreateNewImageTab("Blob Image " + i, bm);
                    i++;
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to convert blob to Image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void miGetBlobAsBytes_Click(object sender, EventArgs e)
        {
            List<byte[]> blobs = GetBlobsFromSelectedCells();
            SqlOutputTabContainer tabContainer = (SqlOutputTabContainer)Parent.Parent.Parent;
            int i = 1;
            foreach (byte[] blob in blobs)
            {
                try
                {
                    string text = "";
                    for (int j = 0; j < blob.Length; j++)
                    {
                        if (j > 0)
                            text += " ";
                        text += blob[j].ToString();
                    }
                    tabContainer.CreateNewTextOutputTab("Blob Image " + i, text);
                    i++;
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to convert blob to Image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void miGetBlobAsFile_Click(object sender, EventArgs e)
        {
            List<byte[]> blobs = GetBlobsFromSelectedCells();
            foreach (byte[] blob in blobs)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.CheckFileExists = false;
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(ofd.FileName, blob);
                }
            }
        }

        void miGetBlobAsText_Click(object sender, EventArgs e)
        {
            List<byte[]> blobs = GetBlobsFromSelectedCells();
            SqlOutputTabContainer tabContainer = (SqlOutputTabContainer)Parent.Parent.Parent;
            int i = 1;
            foreach (byte[] blob in blobs)
            {
                try
                {
                    string text = null;
                    using (StreamReader reader = new StreamReader(new MemoryStream(blob), true))
                    {
                        text = reader.ReadToEnd();
                    }

                    tabContainer.CreateNewTextOutputTab("Blob Text " + i, text);
                    i++;
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to convert blob to String", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MiShowTextOnClick(object sender, EventArgs eventArgs)
        {
            if (SelectedCells.Count < 1)
                return;
            TextOutputDialog tod = new TextOutputDialog(SelectedCells[0].Value.ToString());
            tod.Text = SelectedCells[0].OwningColumn.Name;
            tod.StartPosition = FormStartPosition.CenterParent;
            tod.ShowDialog();
        }

        void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var columnName = "";
            if (SelectedCells != null && SelectedCells.Count == 1)
                columnName = SelectedCells[0].OwningColumn.Name;

            var tableName = SqlResult.TableName;

            bool bEnableGetTitles = false;
            if (SqlResult.Connection != null && SqlResult.Connection.State == ConnectionState.Open && 
                SelectedCells != null && SelectedCells.Count == 1)
            {
                DataGridViewCell cell = SelectedCells[0];
                if (cell.ValueType == typeof(Int32))
                {
                    bEnableGetTitles = true;
                }
            }
            ContextMenuStrip.Items[3].Enabled = bEnableGetTitles;

            bool bEnableCreateScript = false;
            if (SelectedCells.Count > 0)
                bEnableCreateScript = true;
            ContextMenuStrip.Items[4].Enabled = bEnableCreateScript;

            foreach (ToolStripMenuItem menuItem in _dynamicDataMenuItem.DropDownItems)
            {
                var autoQuery = menuItem.Tag as AutoQuery;
                if (autoQuery == null)
                    continue;

                if (!string.IsNullOrEmpty(autoQuery.TableName))
                {
                    menuItem.Enabled = autoQuery.TableName.Equals(tableName, StringComparison.CurrentCultureIgnoreCase);
                    if (!menuItem.Enabled)
                        continue;
                }

                if (!string.IsNullOrEmpty(autoQuery.ColumnName))
                {
                    menuItem.Enabled = autoQuery.ColumnName.Equals(columnName, StringComparison.CurrentCultureIgnoreCase);
                }
            }
        }

        private List<AutoQuery> GetGeneratedTableAutoQueries()
        {
            var res = new List<AutoQuery>();
            if (_sqlResult == null || _sqlResult.DataTable == null)
            {
                return res;
            }
            var sw = new Stopwatch();
            sw.Start();
            foreach (DataColumn column in _sqlResult.DataTable.Columns)
            {
                if (column.ColumnName.Equals("table", StringComparison.CurrentCultureIgnoreCase) ||
                    column.ColumnName.Equals("table_name", StringComparison.CurrentCultureIgnoreCase) ||
                    column.ColumnName.Equals("tablename", StringComparison.CurrentCultureIgnoreCase))
                {
                    res.Add(new AutoQuery { Description = "SELECT * FROM [table]", Command = "SELECT * FROM {" + column.ColumnName + "}", TableName = _sqlResult.TableName});
                }

                if (column.ColumnName.Equals("id", StringComparison.CurrentCultureIgnoreCase))
                {
                    foreach (var table in _databaseSchemaInfo.Tables)
                    {
                        if (table.Columns.FirstOrDefault(x => x.ColumnName.Equals($"{_sqlResult.TableName}id", StringComparison.CurrentCultureIgnoreCase)) != null)
                        {
                            
                            res.Add(new AutoQuery
                            {
                                Description = $"SELECT * FROM {_databaseKeywordEscape.EscapeObject(table.TableName)} WHERE {_sqlResult.TableName}Id = [colVal]",
                                Command = $"SELECT * FROM {_databaseKeywordEscape.EscapeObject(table.TableName)} WHERE {_sqlResult.TableName}Id = " + "{" + column.ColumnName + "}",
                                TableName = _sqlResult.TableName
                            });
                        }
                    }
                }
                else if (column.ColumnName.EndsWith("id", StringComparison.CurrentCultureIgnoreCase))
                {
                    var tableName = column.ColumnName.Substring(0, column.ColumnName.Length - 2);
                    foreach (var table in _databaseSchemaInfo.Tables)
                    {
                        if (table.TableName.Equals(tableName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            var col = table.Columns.FirstOrDefault(x => x.ColumnName.Equals("id", StringComparison.CurrentCultureIgnoreCase));
                            if (col == null)
                            {
                                col = table.Columns.FirstOrDefault(x => x.ColumnName.Equals(column.ColumnName, StringComparison.CurrentCultureIgnoreCase));
                            }

                            if (col != null)
                            {
                                res.Add(new AutoQuery
                                {
                                    Description = $"SELECT * FROM {table.TableName} WHERE {col.ColumnName} = [colVal]",
                                    Command = $"SELECT * FROM {table.TableName} WHERE {col.ColumnName} IN (" + "{" + column.ColumnName + "})",
                                    TableName = _sqlResult.TableName
                                });
                            }
                        }
                    }
                }
            }
            sw.Stop();
            return res;
        }

        void miFill_Click(object sender, EventArgs e)
        {
            if (SqlResult != null && SqlResult.DataAdapter != null)
            {
                try
                {
                    SqlResult.DataTable.Clear();
                    int rows = SqlResult.DataAdapter.Fill(SqlResult.DataTable); // Update(SqlResult.DataTable);
                    if (UpdatedResults != null)
                        UpdatedResults(this, rows, string.Format("Selected {0} rows", rows));
                }
                catch (Exception ex)
                {
                    if (UpdatedResults != null)
                        UpdatedResults(this, 0, ex.Message);
                }
            }
        }

        void miNewRow_Click(object sender, EventArgs e)
        {
            if (ReadOnly)
                return;

            DataGridViewSelectedRowCollection selRows = SelectedRows;
            foreach (DataGridViewRow dgRow in selRows)
            {
                DataRow drNew = _sqlResult.DataTable.NewRow();
                for (int i = 0; i < _sqlResult.DataTable.Columns.Count; i++)
                {
                    drNew[i] = _sqlResult.DataTable.Rows[dgRow.Index][i];
                }
                _sqlResult.DataTable.Rows.Add(drNew);
            }
        }

        protected override void OnDataError(bool displayErrorDialogIfNoHandler, DataGridViewDataErrorEventArgs e)
        {
            
        }

        public void Save()
        {
            miSave_Click(this, new EventArgs());
        }

        void miSave_Click(object sender, EventArgs e)
        {
            if (SqlResult != null && SqlResult.DataAdapter != null)
            {
                try
                {
                    int rows = SqlResult.DataAdapter.Update(SqlResult.DataTable);
                    if (UpdatedResults != null)
                        UpdatedResults(this, rows, string.Format("Updated {0} rows successfully", rows));
                }
                catch (Exception ex)
                {
                    if (UpdatedResults != null)
                        UpdatedResults(this, 0, ex.Message);
                }
            }
        }

        public SqlResult SqlResult
        {
            get { return _sqlResult; }
            set
            {
                _sqlResult = value;
                
                InitializeResults();
                ApplyDefaultFormating();
                CalculateColumnWidts();
            }
        }

        public void ApplyDefaultFormating()
        {
            foreach (DataGridViewColumn col in Columns)
            {
                if (col.ValueType == typeof(DateTime))
                    col.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss FFF";
            }
        }

        public string Filter
        {
            get { return _view.RowFilter; }
            set { _view.RowFilter = value; }
        }

        private void InitializeResults()
        {
            if (_sqlResult == null)
                return;

            _view = new DataView(_sqlResult.DataTable);
            DataSource = _view;
            
            if (_sqlResult.DataAdapter == null)
            {
                ReadOnly = true;
                AllowUserToAddRows = false;
            }

            _generatedDataMenuItem.DropDownItems.Clear();
            //var generatedAutoQueries = GetGeneratedTableAutoQueries();
            //foreach (var query in generatedAutoQueries)
            //{
            //    var menuItem = new ToolStripMenuItem(query.Description);
            //    menuItem.Click += GeneratedQueryMenuItem_Click;
            //    menuItem.Tag = query;
            //    _generatedDataMenuItem.DropDownItems.Add(menuItem);
            //}
        }

        private const int MaxRowsDetailedColumnWidthCalculationTreshold = 1000;

        private void CalculateColumnWidts()
        {
            if (Columns.Count < 1)
                return;

            int iTotalWIdeal = 0;
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Rows.Count > MaxRowsDetailedColumnWidthCalculationTreshold)
                {
                    iTotalWIdeal += Columns[i].GetPreferredWidth(DataGridViewAutoSizeColumnMode.DisplayedCells, true);
                }
                else
                {
                    iTotalWIdeal += Columns[i].GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true);
                }
                
            }

            if (iTotalWIdeal <= (Width - RowHeadersWidth))
            {
                for (int i = 0; i < (Columns.Count); i++)
                {
                    if (Rows.Count > MaxRowsDetailedColumnWidthCalculationTreshold)
                    {
                        Columns[i].Width = Columns[i].GetPreferredWidth(DataGridViewAutoSizeColumnMode.DisplayedCells, true);
                    }
                    else
                    {
                        Columns[i].Width = Columns[i].GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true);
                    }
                }
                    
            }
            else
            {
                List<int> columnsNotResized = new List<int>();
                int iRemainingWidth = (Width - RowHeadersWidth);

                int iAvgWidth = iTotalWIdeal / Columns.Count;
                for (int i = 0; i < (Columns.Count); i++)
                {
                    int iWidthIdeal = Columns[i].GetPreferredWidth(DataGridViewAutoSizeColumnMode.DisplayedCells, true);
                    if (iWidthIdeal < (iAvgWidth + 20))
                        Columns[i].Width = iWidthIdeal;
                    else
                        columnsNotResized.Add(i);
                    iRemainingWidth -= Columns[i].Width;
                }

                if (iRemainingWidth > 0)
                {
                    int iAddWidth = iRemainingWidth / columnsNotResized.Count;
                    foreach (int i in columnsNotResized)
                        Columns[i].Width = Columns[i].Width + iAddWidth;
                }
            }
        }
    }
}
