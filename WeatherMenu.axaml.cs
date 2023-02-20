using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Newtonsoft.Json;
using Timer = System.Timers.Timer;

namespace avalonia_rider_test;

public partial class WeatherMenu : UserControl, ActiveControl
{
    static HttpClient client = new();

    public WeatherMenu()
    {
        this.InitializeComponent();

        //download immediately at start, later downloads are by timer events only
        weatherData = downloadWeather();
        t = new(TimeSpan.FromMinutes(5).TotalMilliseconds); //periodic NodeData check

        t.Elapsed += doWeatherUpdate;
        t.Enabled = true;
        client.Timeout = TimeSpan.FromSeconds(10);
    }

    private Task<Weather[]> weatherData;
    private Timer t;
    private TextBox statusBox;

    public void changeActive(bool active)
    {
        t.Enabled = active;
        this.IsVisible = active;
        if (active) doWeatherUpdate(null, null);
    }

    private void doWeatherUpdate(object? sender, ElapsedEventArgs e)
    {
        //download data if sent from timer, where there would be proper sender args
        if (sender != null) weatherData = downloadWeather();
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            statusBox = desktop.MainWindow.Find<TextBox>("StatusTxt");
        }

#if DEBUG
        Console.WriteLine("updating ui");
#endif
        Dispatcher.UIThread.Post(() =>
        {
            statusBox.Text = "working";
            try
            {
                //weatherData.Wait((int) TimeSpan.FromSeconds(10).TotalMilliseconds);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                statusBox.Text = "request err";
                return;
            }

            try //since the api is iffy and unreliable
            {
                updateInterface();
                statusBox.Text = "ok";
            }
            catch (NullReferenceException e)
            {
#if DEBUG
                Console.WriteLine(e.Message);
                Console.WriteLine($"trace: {e.StackTrace}");
                Console.WriteLine(weatherData.Result[0]);
                Console.WriteLine(weatherData.Result[1]);
#endif
                statusBox.Text = "api returned bad data";
            }
        }, DispatcherPriority.Background);
    }

    private void updateInterface()
    {
        TextBlock[] forecast = new TextBlock[7];

        for (int i = 0; i < 7; i++)
        {
            forecast[i] = this.Find<TextBlock>($"d{i}");
            forecast[i].Text = weatherData.Result[0].properties.periods[i * 2].name + "\n";
            forecast[i].Text += weatherData.Result[0].properties.periods[i * 2].temperature + "\u00B0 \n";
            forecast[i].Text += weatherData.Result[0].properties.periods[i * 2].windSpeed + " ";
            forecast[i].Text += weatherData.Result[0].properties.periods[i * 2].windDirection;
        }

        this.Forecast.Text = weatherData.Result[0].properties.periods[0].detailedForecast;
        this.bigTemp.Text = weatherData.Result[1].properties.periods[0].temperature + "\u00B0";
    }


    private async Task<Weather[]> downloadWeather()
    {
        Weather? intervalWeather;
        Weather? hourlyWeather;

        Weather[] w = await Task.Run(async () =>
        {
            //houghton: "https://api.weather.gov/gridpoints/MQT/114,95/forecast (/hourly)
            //currently just grabs weather NodeData for this hard coded location until I figure 
            //out a location selection menu, which means more api calls to noaa to translate coordinates to a gridpoint station

#if DEBUG
            Console.WriteLine("Downloading forecast data");
#endif
            //waits are done in the same area so both grabs occur at the same time
            string detailedJson = await getjsonStream("https://api.weather.gov/gridpoints/MQT/114,95/forecast");
            string simpleJson = await getjsonStream("https://api.weather.gov/gridpoints/MQT/114,95/forecast/hourly");

            intervalWeather = JsonConvert.DeserializeObject<Weather>(detailedJson); //stores interval stuff
            hourlyWeather = JsonConvert.DeserializeObject<Weather>(simpleJson); //stores hourly stuff

            //put obs downloads here

            if (hourlyWeather == null || intervalWeather == null)
            {
                throw new HttpRequestException("Failed to retrieve weatherData NodeData from NOAA API");
            }
#if DEBUG
            Console.WriteLine("data download complete");
#endif
            //TODO: run downloads for observation NodeData
            //TODO: add some conf ability for setting location, could be presets or from IP
            return new[] {intervalWeather, hourlyWeather}; //by time of day interval, and hourly
        });
        Console.WriteLine("done async");
        doWeatherUpdate(null, null);
        return w;
    }

    private static async Task<string> getjsonStream(string url)
    {
        client.DefaultRequestHeaders.Add("User-Agent", "Rpi-weatherData-station");
        HttpResponseMessage response = await client.GetAsync(url);
        string content = await response.Content.ReadAsStringAsync();
        return content;
    }
}