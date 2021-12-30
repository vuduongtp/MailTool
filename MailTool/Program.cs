using MailTool.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MailTool
{
    static class Program
    {
        public static string WorkingPath = Path.GetDirectoryName(Application.ExecutablePath).Replace("\\", "/");
        public static Image ImportImage = null;
        public static string Host = "smtp.gmail.com";
        public static int Port = 587;
        public static string Username = "";
        public static string Password = "";
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Host = string.IsNullOrEmpty(Settings.Default["Host"].ToString().Trim())? Host : Settings.Default["Host"].ToString().Trim();
                Port = (int.Parse(Settings.Default["Port"].ToString().Trim()) == 0)? Port : int.Parse(Settings.Default["Port"].ToString().Trim());
                Username = Settings.Default["Username"].ToString().Trim();
                Password = Settings.Default["Password"].ToString().Trim();
                using FileStream fs = new FileStream(Program.WorkingPath + "/Resource/import.png", FileMode.Open, FileAccess.Read);
                ImportImage = Image.FromStream(fs);
                fs.Close();
            }
            catch {}
            Application.Run(new Form1());
        }
    }
}
