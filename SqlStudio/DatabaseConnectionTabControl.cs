using CfgDataStore;
using SqlStudio.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SqlStudio
{
	public class DatabaseConnectionTabControl : TabControl
	{
		ContextMenuStrip _dataTabContextMenu = null;
		public IDatabaseConnectionUserControl SelectedDatabaseConnectionUIControl
		{
			get
			{
				if (SelectedTab != null)
				{
					return SelectedTab.Controls[0] as IDatabaseConnectionUserControl;
				}
				return null;
			}
		}

		public IList<IDatabaseConnectionUserControl> DatabaseConnectionUIControls { get; } = new List<IDatabaseConnectionUserControl>();
		public ConfigDataStore ConfigDataStore { get; set; }

		public DatabaseConnectionTabControl()
        {
			_dataTabContextMenu = CreateDataTabsContextMenu();
        }

		private ContextMenuStrip CreateDataTabsContextMenu()
		{
			var cms = new ContextMenuStrip();
			var tsmiDataClose = new ToolStripMenuItem("Close", null, tsmiDataClose_Click);
			cms.Items.Add(tsmiDataClose);
			var tsmiDataCloseAll = new ToolStripMenuItem("Close All", null, tsmiDataCloseAll_Click);
			cms.Items.Add(tsmiDataCloseAll);
			var tsmiDataCloseAllButThis = new ToolStripMenuItem("Close All But This", null, tsmiDataCloseAllButThis_Click);
			cms.Items.Add(tsmiDataCloseAllButThis);
			return cms;
		}

		void tsmiDataClose_Click(object sender, EventArgs e)
		{
			DatabaseConnectionUIControls.Remove(SelectedDatabaseConnectionUIControl);
			TabPages.Remove(SelectedTab);
		}

		private void tsmiDataCloseAll_Click(object sender, EventArgs e)
		{
			if (TabPages.Count > 2)
			{
				for (int i = TabPages.Count - 1; i >= 0; i--)
				{
					var dbControl = TabPages[i].Controls[0] as IDatabaseConnectionUserControl;
					DatabaseConnectionUIControls.Remove(dbControl);
					TabPages.RemoveAt(i);
				}
			}
		}

		private void tsmiDataCloseAllButThis_Click(object sender, EventArgs e)
		{
			if (TabPages.Count > 2)
			{
				var currentTab = SelectedTab;
				for (int i = TabPages.Count - 1; i >= 0; i--)
				{
					if (TabPages[i] != currentTab)
					{
						var dbControl = TabPages[i].Controls[0] as IDatabaseConnectionUserControl;
						DatabaseConnectionUIControls.Remove(dbControl);
						TabPages.RemoveAt(i);
					}
				}
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				for (int i = 0; i < TabPages.Count; i++)
				{
					if (GetTabRect(i).Contains(e.Location))
					{
						SelectedTab = TabPages[i];
						_dataTabContextMenu.Show(this, e.Location);
						return;
					}
				}
			}
		}

		public IDatabaseConnectionUserControl CreateNewDatabaseConnectionTab(string tabText)
		{
			var tabPage = new TabPage();
			tabPage.Text = tabText;
			var databaseConnection = new DatabaseConnectionUserControl(ConfigDataStore);
			tabPage.Controls.Add(databaseConnection);
			databaseConnection.Dock = DockStyle.Fill;
			Controls.Add(tabPage);
			SelectedTab = tabPage;

			DatabaseConnectionUIControls.Add(databaseConnection);

			return databaseConnection;
		}
	}
}
