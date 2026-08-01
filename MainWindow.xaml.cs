using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Wpf.Ui.Controls;

namespace Web2AppLauncher
{
    public partial class MainWindow : FluentWindow
    {
        private string _selectedIconPath = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TxtUrl_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Автоматически подставляем URL в качестве названия, если имя еще не введено или совпадает
            if (string.IsNullOrWhiteSpace(TxtName.Text) || TxtName.Text == TxtUrl.Text)
            {
                TxtName.Text = TxtUrl.Text;
            }
        }

        private void BtnBrowseIcon_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Иконки (*.ico;*.png)|*.ico;*.png"
            };

            if (dlg.ShowDialog() == true)
            {
                _selectedIconPath = dlg.FileName;
                TxtIcon.Text = _selectedIconPath;
            }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            string url = TxtUrl.Text.Trim();
            string appName = TxtName.Text.Trim();
            string userAgent = TxtUserAgent.Text.Trim();

            if (string.IsNullOrEmpty(url))
            {
                // Явно указываем System.Windows, чтобы избежать конфликта с Wpf.Ui.Controls
                System.Windows.MessageBox.Show(
                    "Введите URL!", 
                    "Ошибка", 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Warning);
                return;
            }

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
            }

            try
            {
                // Формируем безопасное имя папки
                string safeName = string.Join("_", appName.Split(Path.GetInvalidFileNameChars()));
                if (string.IsNullOrEmpty(safeName)) safeName = "Web2App";

                // Создаем изолированную директорию для приложения
                string appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Web2Apps", safeName);
                Directory.CreateDirectory(appDir);

                // Сохраняем файл конфигурации
                string configPath = Path.Combine(appDir, "config.json");
                string json = $"{{\"Url\":\"{url}\",\"UserAgent\":\"{userAgent}\",\"Title\":\"{appName}\",\"AppId\":\"Web2App.{safeName}\"}}";
                File.WriteAllText(configPath, json);

                // Обработка иконки (.png конвертируется в .ico для Windows)
                string iconDestination = Path.Combine(appDir, "app.ico");
                if (!string.IsNullOrEmpty(_selectedIconPath) && File.Exists(_selectedIconPath))
                {
                    if (Path.GetExtension(_selectedIconPath).ToLower() == ".png")
                    {
                        ConvertPngToIco(_selectedIconPath, iconDestination);
                    }
                    else
                    {
                        File.Copy(_selectedIconPath, iconDestination, true);
                    }
                }

                // Создаем ярлык на рабочем столе
                CreateShortcut(safeName, appDir, iconDestination);

                // Явно указываем System.Windows
                System.Windows.MessageBox.Show(
                    $"Приложение '{appName}' успешно создано и добавлено на Рабочий стол!", 
                    "Успех", 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Явно указываем System.Windows
                System.Windows.MessageBox.Show(
                    $"Ошибка при создании: {ex.Message}", 
                    "Ошибка", 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Error);
            }
        }

        private void ConvertPngToIco(string pngPath, string icoPath)
        {
            using (var bitmap = new System.Drawing.Bitmap(pngPath))
            {
                using (var resized = new System.Drawing.Bitmap(bitmap, new System.Drawing.Size(256, 256)))
                {
                    IntPtr hIcon = resized.GetHicon();
                    using (var icon = System.Drawing.Icon.FromHandle(hIcon))
                    {
                        using (FileStream fs = new FileStream(icoPath, FileMode.Create))
                        {
                            icon.Save(fs);
                        }
                    }
                }
            }
        }

        private void CreateShortcut(string appName, string appDir, string iconPath)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string shortcutPath = Path.Combine(desktopPath, $"{appName}.lnk");

            Type shellType = Type.GetTypeFromProgID("WScript.Shell")!;
            dynamic shell = Activator.CreateInstance(shellType)!;
            var shortcut = shell.CreateShortcut(shortcutPath);

            shortcut.TargetPath = System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName;
            shortcut.Arguments = $"\"{appDir}\"";
            shortcut.WorkingDirectory = appDir;
            
            if (File.Exists(iconPath))
            {
                shortcut.IconLocation = iconPath;
            }
            
            shortcut.Save();
        }
    }
}