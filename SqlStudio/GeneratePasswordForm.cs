using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlStudio
{
    public partial class GeneratePasswordForm : Form
    {
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
    }
}
