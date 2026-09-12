using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Web2AppLauncher
{
    public partial class MainWindow : Window
    {
        private string _selectedIconPath = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => this.Close();

        private void BtnMinimize_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
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
                string safeName = string.Join("_", appName.Split(Path.GetInvalidFileNameChars()));
                if (string.IsNullOrEmpty(safeName)) safeName = "Web2App";

                string appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Web2Apps", safeName);
                Directory.CreateDirectory(appDir);

                string configPath = Path.Combine(appDir, "config.json");
                string json = $"{{\"Url\":\"{url}\",\"UserAgent\":\"{userAgent}\",\"Title\":\"{appName}\",\"AppId\":\"Web2App.{safeName}\"}}";
                File.WriteAllText(configPath, json);

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

                CreateShortcut(safeName, appDir, iconDestination);

                System.Windows.MessageBox.Show(
                    $"Приложение '{appName}' успешно создано и добавлено на Рабочий стол!", 
                    "Успех", 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
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