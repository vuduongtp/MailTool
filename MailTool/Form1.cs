using MailTool.Properties;
using System;
using System.Linq;
using System.Windows.Forms;
using WebApi.Helpers.Services;

namespace MailTool
{
    public partial class Form1 : Form
    {
        public void WriteLog(string log)
        {
            this.txtLog.AppendText(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + " : " + log + "\n");
            this.txtLog.SelectionStart = this.txtLog.Text.Length;
            this.txtLog.ScrollToCaret();
        }
        public Form1()
        {
            InitializeComponent();
            FileMenuItem.DropDownItems.Add("Import HTML", Program.ImportImage, ImportFile_ItemClicked);
            ConfigMenuItem.DropDownItems.Add("Email Sender", Program.ImportImage, ConfigEmail_ItemClicked);
        }

        private void ImportFile_ItemClicked(object sender, EventArgs e)
        {
            OpenFileDialog FileDialog = new OpenFileDialog();
            FileDialog.RestoreDirectory = true;
            FileDialog.Title = "Browse HTML Files";
            FileDialog.Filter = "HTML file (*.html)|*.html|All file (*.*)|*.*";
            FileDialog.CheckFileExists = true;
            FileDialog.CheckPathExists = true;
            FileDialog.ShowDialog();
            if (string.IsNullOrEmpty(FileDialog.FileName))
            {
                MessageBox.Show("Hãy chọn file!");
                return;
            }
            string textHTML = System.IO.File.ReadAllText(FileDialog.FileName);
            txtNoiDung.Text = textHTML;
        }

        private void ConfigEmail_ItemClicked(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<ConfigEmail>().Count() == 1) Application.OpenForms.OfType<ConfigEmail>().First().Close();
            ConfigEmail frm = new ConfigEmail();
            frm.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int cboIndex = int.Parse(Settings.Default["contentType"].ToString());
            cboType.SelectedIndex = cboIndex;
            txtTo.Text = Settings.Default["to"].ToString().Trim();
            txtCC.Text = Settings.Default["cc"].ToString().Trim();
            txtNoiDung.Text = Settings.Default["noiDung"].ToString().Trim();
            txtTieuDe.Text = Settings.Default["tieuDe"].ToString().Trim();
            txtLog.Text = Settings.Default["log"].ToString();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            btnSend.Enabled = false;
            lblSending.Visible = true;
            loading.Visible = true;
            string to = txtTo.Text.Trim();
            string cc = txtCC.Text.Trim();
            string[] toEmails = to.Split(";", StringSplitOptions.RemoveEmptyEntries).Length > 0 ? to.Split(";", StringSplitOptions.RemoveEmptyEntries) : new string[] { to };
            string[] ccEmails = cc.Split(";", StringSplitOptions.RemoveEmptyEntries).Length > 0 ? cc.Split(";", StringSplitOptions.RemoveEmptyEntries) : (string.IsNullOrEmpty(cc)) ? new string[] { } : new string[] { cc };
            string tieuDe = txtTieuDe.Text.Trim();
            string noiDung = txtNoiDung.Text.Trim();
            string contentType = cboType.SelectedItem.ToString().Trim();
            SaveInputSetting(to, cc, tieuDe, noiDung, cboType.SelectedIndex);
            if (!CheckEmailList(toEmails) || !CheckEmailList(ccEmails))
            {
                loading.Visible = false;
                lblSending.Visible = false;
                btnSend.Enabled = true;
                return;
            }
            if (string.IsNullOrEmpty(tieuDe) || string.IsNullOrEmpty(noiDung) || string.IsNullOrEmpty(to))
            {
                MessageBox.Show("Hãy nhập đủ thông tin!");
                loading.Visible = false;
                lblSending.Visible = false;
                btnSend.Enabled = true;
                return;
            }
            try
            {
                EmailService.SendMail(toEmails, ccEmails, tieuDe, noiDung, contentType, null, null, null);
                WriteLog("Gửi thành công tới: " + to + " và cc: " + cc);
            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString());
                MessageBox.Show(ex.Message, "Lỗi");
            }
            loading.Visible = false;
            lblSending.Visible = false;
            btnSend.Enabled = true;
        }

        private void SaveInputSetting(string to, string cc, string tieuDe, string noiDung, int contentType)
        {
            Settings.Default["to"] = to;
            Settings.Default["cc"] = cc;
            Settings.Default["tieuDe"] = tieuDe;
            Settings.Default["noiDung"] = noiDung;
            Settings.Default["contentType"] = contentType;
            Settings.Default.Save();
        }

        private bool CheckEmailList(string[] emails)
        {
            if (emails.Length == 0)
            {
                return true;
            }
            foreach (string email in emails)
            {
                if (!IsValidEmail(email))
                {
                    MessageBox.Show("Địa chỉ email sai: " + email);
                    return false;
                }
            }
            return true;
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default["log"] = txtLog.Text;
            Settings.Default.Save();
        }
    }
}
