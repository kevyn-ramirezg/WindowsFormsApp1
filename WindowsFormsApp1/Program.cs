using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var login = new WindowsFormsApp1.Forms.FormLogin())
            {
                if (login.ShowDialog() == DialogResult.OK)
                    Application.Run(new Form1());
            }
        }
    }
}
