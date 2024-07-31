using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_Tasks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
                "Html",
                typeof(string),
                typeof(MainWindow),
                new FrameworkPropertyMetadata(OnHtmlChanged)
            );
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MyButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Debug.WriteLine($"Thread number: {Thread.CurrentThread.ManagedThreadId}");
                HttpClient webClient = new HttpClient();
                string html = webClient.GetStringAsync("https://google.com").Result;
                MyButton.Dispatcher.Invoke(() =>
                {
                    Debug.WriteLine($"Thread number: {Thread.CurrentThread.ManagedThreadId} owns mybutton");
                    MyButton.Content = "Done";
                });
            });
        }
        private async void MyButton_Click2(object sender, RoutedEventArgs e)
        {
            string myHtml = "CHAMA";

            Debug.WriteLine($"Thread number: {Thread.CurrentThread.ManagedThreadId} before await task");
            await Task.Run(async () =>
            {
                Debug.WriteLine($"Thread number: {Thread.CurrentThread.ManagedThreadId} during await task");
                HttpClient webClient = new HttpClient();
                string html = webClient.GetStringAsync("https://www.google.com/search?q=honda+prelude+bb4+wallpaper&sca_esv=c434643757074884&udm=2&biw=1745&bih=835&sxsrf=ADLYWIJrsUnRUuAb_m4lkELQmBa9AW_pLA%3A1722420622473&ei=jg2qZoLDHOasxc8PkaaliQU&ved=0ahUKEwiCwuzqhNGHAxVmVvEDHRFTKVEQ4dUDCBA&uact=5&oq=honda+prelude+bb4+wallpaper&gs_lp=Egxnd3Mtd2l6LXNlcnAiG2hvbmRhIHByZWx1ZGUgYmI0IHdhbGxwYXBlckjhEVDNAljgEXABeACQAQCYAY0BoAGzCaoBAzAuObgBA8gBAPgBAZgCAqAChwHCAgcQABiABBgTwgIGEAAYExgemAMAiAYBkgcDMS4xoAeGBQ&sclient=gws-wiz-serp#vhid=8ybFDzyNW6RQAM&vssid=mosaic").Result;
                myHtml = html;
            });

            Debug.WriteLine($"Thread number: {Thread.CurrentThread.ManagedThreadId} after await task");
            MyButton.Content = "Done Downloading";
            MyWebBrowser.SetValue(HtmlProperty, myHtml);
        }

        private static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser webBrowser = dependencyObject as WebBrowser;
            if (webBrowser != null)
            {
                webBrowser.NavigateToString(e.NewValue as string);
            }
        }

    }
}
