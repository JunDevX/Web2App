using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
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

            // GPU-ускорение для WebView2
            string userDataFolder = Path.Combine(configDir, "WebViewData");
            var options = new CoreWebView2EnvironmentOptions(
                additionalBrowserArguments: "--enable-gpu --enable-gpu-rasterization --enable-zero-copy --ignore-gpu-blocklist --enable-features=UseSkiaRenderer,CanvasOopRasterization --enable-accelerated-2d-canvas --disable-features=CalculateNativeWinOcclusion"
            );

            var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder, options);
            await webView.EnsureCoreWebView2Async(env);

            if (string.IsNullOrEmpty(userAgent))
            {
                userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36";
            }

            webView.CoreWebView2.Settings.UserAgent = userAgent;
            webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            webView.CoreWebView2.Settings.IsWebMessageEnabled = true;

            webView.CoreWebView2.PermissionRequested += (s, args) =>
            {
                args.State = CoreWebView2PermissionState.Allow;
                args.Handled = true;
            };

            webView.SourceChanged += (s, args) =>
            {
                if (TxtUrl != null && webView.Source != null)
                {
                    TxtUrl.Text = webView.Source.ToString();
                }
            };

            if (!string.IsNullOrEmpty(url))
            {
                webView.Source = new Uri(url);
            }
        }

        #region Управление окном (macOS Traffic Lights)

        private void BtnClose_Click(object sender, RoutedEventArgs e) => this.Close();

        private void BtnMinimize_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        #endregion

        #region Навигация WebView2

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (webView != null && webView.CanGoBack) webView.GoBack();
        }

        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (webView != null && webView.CanGoForward) webView.GoForward();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => webView?.Reload();

        private void BtnGo_Click(object sender, RoutedEventArgs e) => NavigateToUrl();

        private void TxtUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) NavigateToUrl();
        }

        private void NavigateToUrl()
        {
            if (webView == null || string.IsNullOrWhiteSpace(TxtUrl.Text)) return;

            string targetUrl = TxtUrl.Text.Trim();
            if (!targetUrl.StartsWith("http://") && !targetUrl.StartsWith("https://"))
            {
                targetUrl = "https://" + targetUrl;
            }

            try
            {
                webView.Source = new Uri(targetUrl);
            }
            catch (UriFormatException) { }
        }

        #endregion

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