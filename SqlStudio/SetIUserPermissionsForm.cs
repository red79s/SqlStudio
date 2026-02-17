using SqlExecute;
using System;
using System.Windows.Forms;

namespace SqlStudio
{
    public partial class SetUserPermissionsForm : Form
    {
        private readonly SqlExecuter _sqlExecuter;

        public SetUserPermissionsForm(SqlExecute.SqlExecuter sqlExecuter)
        {
            InitializeComponent();
            _sqlExecuter = sqlExecuter;
            InitializeUserListView();
        }

        private void InitializeUserListView()
        {

        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            var userId = GetSelectedUserId();
            if (userId == null)
            {
                MessageBox.Show("Please select a user.");
                return;
            }
            
            long groupId = 0;

            var res = _sqlExecuter.ExecuteSql("select Id from [Group] where Name = 'GroupAdminCanAssignAllRoles'");
            if (res.DataTable.Rows.Count > 0)
            {
                groupId = (long)res.DataTable.Rows[0][0];
            }
            else
            {
                _sqlExecuter.ExecuteSql("INSERT INTO [Group] (Name) VALUES('GroupAdminCanAssignAllRoles')");
                res = _sqlExecuter.ExecuteSql("select Id from [Group] where Name = 'GroupAdminCanAssignAllRoles'");
                if (res.DataTable.Rows.Count > 0)
                {
                    groupId = (long)res.DataTable.Rows[0][0];
                }
            }

            if (groupId == 0)
            {
                MessageBox.Show("Failed to create group");
                return;
            }

            _sqlExecuter.ExecuteSql($"INSERT INTO Role (GroupId,Name) VALUES({groupId},'GroupAdminCanAssignAllRoles');");

            _sqlExecuter.ExecuteSql($"INSERT INTO PersonGroup (GroupId,PersonId) VALUES({groupId},{userId});");
        }



        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void GetUsers_Click(object sender, EventArgs e)
        {
            try
            {
                checkedListBoxUsers.Items.Clear();

                var sql = "select id, name from Person";
                var res = _sqlExecuter.ExecuteSql(sql);

                if (res.Success && res.DataTable != null)
                {
                    foreach (System.Data.DataRow row in res.DataTable.Rows)
                    {
                        var item = new UserInfo { Id = (long)row["id"], Name = row["name"].ToString() };
                        checkedListBoxUsers.Items.Add(item);
                    }
                }
                else
                {
                    MessageBox.Show($"Failed to load users: {res.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper method to get the selected user ID
        public long? GetSelectedUserId()
        {
            if (checkedListBoxUsers.CheckedItems.Count > 0)
            {
                var userInfo = checkedListBoxUsers.CheckedItems[0] as UserInfo;
                return userInfo?.Id;
            }

            return null;
        }

        // Helper method to get the selected user name
        public string GetSelectedUserName()
        {
            if (checkedListBoxUsers.SelectedItems.Count > 0)
            {
                var userInfo = checkedListBoxUsers.SelectedItems[0] as UserInfo;
                return userInfo?.Name ?? "";
            }
            return "";
        }

        private class UserInfo
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public override string ToString()
            {
                return $"{Name}";
            }
        }
    }
}
