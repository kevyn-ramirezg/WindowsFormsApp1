using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Forms;

namespace WindowsFormsApp1
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var login = new FormLogin())
            {
                if (login.ShowDialog() != DialogResult.OK)
                    return; // si cancela, salir
            }

            Application.Run(new Form1());
        }
    }
}
