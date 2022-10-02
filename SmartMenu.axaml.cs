using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace avalonia_rider_test;

public partial class SmartMenu : UserControl, ActiveControl
{
    public SmartMenu()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        
#if !DEBUG
        runUdp();
#endif
    }

    private async void runUdp()
    {
        await Task.Run(function: () =>
        {
            var Server = new UdpClient(8984);
            IPEndPoint client = new(IPAddress.Any, 0);

            Byte[] buffer;
            Byte[] response = Encoding.ASCII.GetBytes("0,110");
            Byte[] response2 = Encoding.ASCII.GetBytes("hi other one");
            
            String data;

            Console.WriteLine("starting listen loop");
            while (true)
            {
                buffer = Server.Receive(ref client);
                data = Encoding.ASCII.GetString(buffer);

                Console.WriteLine($"{data} from {client.Address}");

                if (data.Equals("a"))
                {
                    Console.WriteLine("replying to 1");
                    for (int i = 10; i <= 110; i+= 10)
                    {
                        Thread.Sleep(100);
                        Server.Send(Encoding.ASCII.GetBytes("0," + i), client.Address.ToString(), 8984);
                    }
                }
                if (data.Equals("b"))
                {
                    Console.WriteLine("replying to 2");
                    for (int i = 0; i < 10; i++)
                    {
                        Server.Send(response2, client.Address.ToString(), 8984);
                    }
                }
            }
        });
    }
    
    
    
    public void changeActive(bool active)
    {
        this.IsVisible = active;
        //whatever else needs to be done here
    }

    private void buttonOff(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void buttonOn(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}