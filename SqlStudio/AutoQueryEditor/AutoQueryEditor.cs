using CfgDataStore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlStudio.AutoQueryEditor
{
    public partial class AutoQueryEditor : Form
    {
        private DataTable _dtAutoQueries;
        private ConfigDataStore _configDataStore;
        private List<AutoQuery> _autoQueries;

        public AutoQueryEditor()
        {
            InitializeComponent();
        }

        public void Init(ConfigDataStore configDataStore)
        {
            _configDataStore = configDataStore;

            _dtAutoQueries = new DataTable();
            _dtAutoQueries.Columns.Add(nameof(AutoQuery.Description), typeof(string));
            _dtAutoQueries.Columns.Add(nameof(AutoQuery.TableName), typeof(string));
            _dtAutoQueries.Columns.Add(nameof(AutoQuery.ColumnName), typeof(string));
            _dtAutoQueries.Columns.Add(nameof(AutoQuery.Command), typeof(string));
            _dtAutoQueries.Columns.Add(nameof(AutoQuery.AutoQueryId), typeof(long));

            dgwAutoQueries.DataSource = _dtAutoQueries;
            dgwAutoQueries.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dgwAutoQueries.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dgwAutoQueries.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dgwAutoQueries.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgwAutoQueries.Columns[4].Visible = false;
            dgwAutoQueries.RowHeadersWidth = 15;

            _autoQueries = _configDataStore.GetAutoQueries();
            foreach(var query in _autoQueries)
            {
                DataRow dr = _dtAutoQueries.NewRow();
                dr[nameof(AutoQuery.AutoQueryId)] = query.AutoQueryId;
                dr[nameof(AutoQuery.Description)] = query.Description;
                dr[nameof(AutoQuery.TableName)] = query.TableName;
                dr[nameof(AutoQuery.ColumnName)] = query.ColumnName;
                dr[nameof(AutoQuery.Command)] = query.Command;
                _dtAutoQueries.Rows.Add(dr);
            }
            _dtAutoQueries.AcceptChanges();
        }

        private void PersistChanges()
        {
            foreach (DataRow row in _dtAutoQueries.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                {
                    row.RejectChanges();
                    var autoQueryItem = _autoQueries.FirstOrDefault(x => x.AutoQueryId == (long)row[nameof(AutoQuery.AutoQueryId)]);
                    if (autoQueryItem != null)
                    {
                        _configDataStore.RemoveAutoQuery(autoQueryItem);
                    }
                }
                else if (row.RowState == DataRowState.Added)
                {
                    _configDataStore.AddAutoQuery(new AutoQuery
                    {
                        Description = row[nameof(AutoQuery.Description)] as string,
                        TableName = row[nameof(AutoQuery.TableName)] as string,
                        ColumnName = row[nameof(AutoQuery.ColumnName)] as string,
                        Command = row[nameof(AutoQuery.Command)] as string
                    });
                }
                else if (row.RowState == DataRowState.Modified)
                {
                    var autoQueryItem = _autoQueries.FirstOrDefault(x => x.AutoQueryId == (long)row[nameof(AutoQuery.AutoQueryId)]);
                    if (autoQueryItem != null)
                    {
                        autoQueryItem.Description = row[nameof(AutoQuery.Description)] as string;
                        autoQueryItem.TableName = row[nameof(AutoQuery.TableName)] as string;
                        autoQueryItem.ColumnName = row[nameof(AutoQuery.ColumnName)] as string;
                        autoQueryItem.Command = row[nameof(AutoQuery.Command)] as string;
                    }
                }
            }

            _configDataStore.Save();
        }

        private void OnOkButtonClick(object sender, EventArgs e)
        {
            PersistChanges();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
