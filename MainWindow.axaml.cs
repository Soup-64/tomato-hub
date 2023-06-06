using System;
using System.Diagnostics;
using System.Globalization;
using System.Timers;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace avalonia_rider_test
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            Timer t = new Timer(1000);
            t.Elapsed += doTime;
            t.Enabled = true;
            Switcher.PageTransition = new CrossFade(TimeSpan.FromSeconds(0.25));
        }
        
        private void doTime(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.UIThread.Post(() => { DateTime.Text = e.SignalTime.ToString(CultureInfo.CurrentCulture); },
                DispatcherPriority.Background);
        }

        private void backClick(object? sender, RoutedEventArgs e)
        {
            ((ActiveControl) this.Switcher.SelectedItem!).changeActive(false);
            this.Switcher.SelectedIndex = 0;
            ((ActiveControl) this.Switcher.SelectedItem!).changeActive(true);
        }

        private void sleepClick(object? sender, RoutedEventArgs e)
        {
            Process p = new();
            p.StartInfo.FileName = "/home/auto/sleep.sh";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = "sleep";
            p.Start();
        }
    }
}