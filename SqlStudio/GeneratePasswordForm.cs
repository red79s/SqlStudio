using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Windows.Forms;

namespace SqlStudio
{
    public partial class GeneratePasswordForm : Form
    {
        public string PasswordClearText { get { return tbPassword.Text; } }
        public string PasswordHash { get { return tbHash.Text; } }

        public GeneratePasswordForm()
        {
            InitializeComponent();
        }

        private void GeneratePasswordHash_Click(object sender, EventArgs e)
        {
            try
            {
                var user = new Common.Model.User();
                var options = new PasswordHasherOptions { CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2 };

                var passwordHasher = new PasswordHasher<Common.Model.User>(Options.Create(options));
                var passwordHash = passwordHasher.HashPassword(user, tbPassword.Text);
                tbHash.Text = passwordHash;
                Clipboard.SetText(passwordHash);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Failed to generate password", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void CloseWriteButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CreatePasswordButton_Click(object sender, EventArgs e)
        {
            tbPassword.Text = GeneratePassword(18);
        }

        private string GeneratePassword(int length)
        {
            var random = new Random((int)DateTime.Now.Ticks);
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string result = "";
            for (int i = 0; i < length; i++)
            {
                result += chars[random.Next(chars.Length)];
            }
            
            return result;
        }
    }
}
