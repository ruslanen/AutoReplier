using System;
using System.Drawing;
using System.Windows.Forms;
using AutoReplier.WinForm.Properties;

namespace AutoReplier.WinForm
{
    /// <summary>
    /// Контекст приложения без формы
    /// (иконка в трее)
    /// </summary>
    public class TrayOnlyApplicationContext : ApplicationContext
    {
        private NotifyIcon _trayIcon;

        public TrayOnlyApplicationContext()
        {
            _trayIcon = new NotifyIcon()
            {
                Icon = new Icon(Resource1.TrayIcon),
                ContextMenu = new ContextMenu(new MenuItem[] { new MenuItem("Выход", Exit) }),
                Visible = true,
            };
        }

        private void Exit(object sender, EventArgs e)
        {
            _trayIcon.Visible = false;
            Application.Exit();
        }
    }
}
