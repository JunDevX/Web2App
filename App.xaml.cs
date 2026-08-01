using System;
using System.IO;
using System.Windows;

namespace Web2AppLauncher
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.ShutdownMode = ShutdownMode.OnLastWindowClose;

            // Если ярлык передал аргументы пути к папке приложения
            if (e.Args.Length > 0)
            {
                string configDir = e.Args[0].Trim('"');
                if (Directory.Exists(configDir))
                {
                    AppWindow appWin = new AppWindow(configDir);
                    appWin.Show();
                    return;
                }
            }

            // Если запустили генератор напрямую — открываем его
            MainWindow mainWin = new MainWindow();
            mainWin.Show();
        }
    }
}