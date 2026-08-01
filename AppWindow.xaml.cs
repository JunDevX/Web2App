using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace Web2AppLauncher
{
    public partial class AppWindow : Window
    {
        [DllImport("shell32.dll", SetLastError = true)]
        private static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

        public AppWindow(string configDir)
        {
            InitializeComponent();
            InitWebView(configDir);
        }

        private async void InitWebView(string configDir)
        {
            string configPath = Path.Combine(configDir, "config.json");
            if (!File.Exists(configPath)) return;

            string json = File.ReadAllText(configPath);
            
            string url = ExtractJsonValue(json, "Url");
            string userAgent = ExtractJsonValue(json, "UserAgent");
            string title = ExtractJsonValue(json, "Title");
            string appId = ExtractJsonValue(json, "AppId");

            this.Title = title;

            // Разделяем процесс на панели задач
            if (!string.IsNullOrEmpty(appId))
            {
                try { SetCurrentProcessExplicitAppUserModelID(appId); } catch { }
            }

            // Устанавливаем иконку окна
            string iconPath = Path.Combine(configDir, "app.ico");
            if (File.Exists(iconPath))
            {
                this.Icon = new System.Windows.Media.Imaging.BitmapImage(new Uri(iconPath));
            }

            // ОПТИМИЗАЦИЯ СКОРОСТИ: Включаем GPU-ускорение и аппаратный рендеринг Chromium
            string userDataFolder = Path.Combine(configDir, "WebViewData");
            
            var options = new CoreWebView2EnvironmentOptions(
                additionalBrowserArguments: "--enable-gpu-rasterization --enable-zero-copy --ignore-gpu-blocklist --enable-features=UseSkiaRenderer"
            );

            var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder, options);
            await webView.EnsureCoreWebView2Async(env);

            // Если User-Agent не был передан — ставим современный дефолтный Chrome
            if (string.IsNullOrEmpty(userAgent))
            {
                userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36";
            }
            webView.CoreWebView2.Settings.UserAgent = userAgent;

            // Настройки для тяжелых сервисов (Discord, Telegram и т.д.)
            webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            webView.CoreWebView2.Settings.IsWebMessageEnabled = true;

            // Автоматически разрешаем микрофон/камеру/уведомления для Discord
            webView.CoreWebView2.PermissionRequested += (s, args) =>
            {
                args.State = CoreWebView2PermissionState.Allow;
            };

            webView.Source = new Uri(url);
        }

        private string ExtractJsonValue(string json, string key)
        {
            string search = $"\"{key}\":\"";
            int start = json.IndexOf(search);
            if (start == -1) return "";
            start += search.Length;
            int end = json.IndexOf("\"", start);
            return end == -1 ? "" : json.Substring(start, end - start);
        }
    }
}