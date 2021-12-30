using MailTool.Properties;
using System;
using System.Linq;
using System.Windows.Forms;

namespace MailTool
{
    public partial class ConfigEmail : Form
    {
        public ConfigEmail()
        {
            InitializeComponent();
        }

        private void ConfigEmail_Load(object sender, EventArgs e)
        {
            txtHost.Text = Program.Host;
            txtPort.Text = Program.Port.ToString();
            txtUsername.Text = Program.Username;
            txtPassword.PasswordChar = '*';
            txtPassword.Text = Program.Password;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtHost.Text) || string.IsNullOrEmpty(txtUsername.Text)
                || string.IsNullOrEmpty(txtPassword.Text) || string.IsNullOrEmpty(txtPort.Text))
            {
                MessageBox.Show("Hãy nhập đủ thông tin!");
                return;
            }
            if (!IsValidEmail(txtUsername.Text.Trim()))
            {
                MessageBox.Show("Địa chỉ email không hợp lệ!");
                return;
            }
            Settings.Default["Host"] = txtHost.Text.Trim();
            Settings.Default["Port"] = int.Parse(txtPort.Text.Trim());
            Settings.Default["Username"] = txtUsername.Text.Trim();
            Settings.Default["Password"] = txtPassword.Text.Trim();
            Settings.Default.Save();
            Program.Host = txtHost.Text.Trim();
            Program.Port = int.Parse(txtPort.Text.Trim());
            Program.Username = txtUsername.Text.Trim();
            Program.Password = txtPassword.Text.Trim();
            if (Application.OpenForms.OfType<Form1>().Count() == 1)
            {
                Application.OpenForms.OfType<Form1>().First().WriteLog("Config Email thành công!");
            }
            this.Close();
        }

        bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();
            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }
}
