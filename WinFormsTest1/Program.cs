using System;
using System.Windows.Forms;

namespace WinFormsTest1
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Models.NameGetter1());
        }
    }
}
