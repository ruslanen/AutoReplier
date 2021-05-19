using System;
using System.Threading;
using System.Windows.Forms;

namespace AutoReplier.WinForm
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool result;
            using (var mutex = new System.Threading.Mutex(true, "8b5c1741-4948-4983-a87b-2492adb9a5df", out result))
            {
                if (!result)
                {
                    MessageBox.Show("Экземпляр программы уже запущен.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var worker = new Thread(() =>
                {
                    AutoReplier.Program.Main(new string[0]);
                });
                worker.IsBackground = true;
                worker.Start();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new TrayOnlyApplicationContext());
            }
        }
    }
}
