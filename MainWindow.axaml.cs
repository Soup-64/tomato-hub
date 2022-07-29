using System.Globalization;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace avalonia_rider_test
{
    public class MainWindow : Window
    {
        private TextBox DateTime;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            DateTime = this.Find<TextBox>("DateTime");

            Timer t = new Timer(1000);
            t.Elapsed += doTime;
            t.Enabled = true;
        }

        
        
        private void doTime(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                DateTime.Text = e.SignalTime.ToString(CultureInfo.CurrentCulture);
            }, DispatcherPriority.Background);
        }

        private void backClick(object? sender, RoutedEventArgs e)
        {
            Carousel switcher = this.Find<Carousel>("Switcher");
            ((ActiveControl) switcher.SelectedItem!).changeActive(false);
            switcher.SelectedIndex = 0;
            ((ActiveControl) switcher.SelectedItem!).changeActive(true);
        }
    }
}