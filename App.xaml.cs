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

            // Закрывать приложение только когда закрыты ВСЕ его окна
            this.ShutdownMode = ShutdownMode.OnLastWindowClose;

            // Если ярлык передал аргументы пути к папке приложения
            if (e.Args.Length > 0)
            {
                string configDir = e.Args[0].Trim('"');
                if (Directory.Exists(configDir))
                {
                    // Запускаем ИСКЛЮЧИТЕЛЬНО окно веб-приложения
                    AppWindow appWin = new AppWindow(configDir);
                    appWin.Show();
                    return;
                }
            }

            // Если запустили генератор напрямую — открываем ТОЛЬКО генератор
            MainWindow mainWin = new MainWindow();
            mainWin.Show();
        }
    }
}