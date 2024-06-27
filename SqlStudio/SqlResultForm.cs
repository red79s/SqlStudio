using Common.Model;
using System;
using System.Windows.Forms;

namespace SqlStudio
{
    public partial class SqlResultForm : Form
    {
        public SqlResultForm(IServiceProvider serviceProvider, SqlResult sqlResult)
        {
            InitializeComponent();

            Text = sqlResult.TableName;

            var tabDataGridContainer = new TabDataGridContainer(serviceProvider);
            tabDataGridContainer.Dock = DockStyle.Fill;
            tabDataGridContainer.SqlResult = sqlResult;
            Controls.Add(tabDataGridContainer);
        }
    }
}
