using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Newtonsoft.Json;

namespace avalonia_rider_test;

public partial class WeatherMenu : UserControl, ActiveControl
{
    public WeatherMenu()
    {
        initializeComponent();
    }

    public string someproperty = "hi";

    private Task<Weather[]> weatherData;
    private Timer t;

    public void changeActive(bool active)
    {
        t.Enabled = active;
        this.IsVisible = active;
        if (active) doWeatherUpdate(null, null);
    }
    
    private void initializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        weatherData = downloadWeather();


        //TODO: fill UI with weatherData stuff
        //TODO: periodically download new data

        t = new(5000); //periodic data check
        t.Elapsed += doWeatherUpdate;
        t.Enabled = false;
    }

    private void doWeatherUpdate(object? sender, ElapsedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            TextBox bigTemp = this.Find<TextBox>("bigTemp");

            if (weatherData.IsCompletedSuccessfully)
            {
                bigTemp.Text = weatherData.Result[1].properties.periods[0].temperature + "\u00B0";
            }
            else
            {
                bigTemp.Text = "waiting for stuff...";
            }
        }, DispatcherPriority.Background);
    }


    private static async Task<Weather[]> downloadWeather()
    {
        //houghton: "https://api.weather.gov/gridpoints/MQT/114,95/forecast (/hourly)
        //home: https://api.weather.gov/gridpoints/GRR/46,46/forecast
        //rockport: https://api.weather.gov/gridpoints/BOX/84,93/forecast

        Console.WriteLine("Downloading forecast data");
        
        string detailedJson = await getjsonStream("https://api.weather.gov/gridpoints/BOX/84,93/forecast");
        string simpleJson = await getjsonStream("https://api.weather.gov/gridpoints/BOX/84,93/forecast/hourly");

        //waits are done in the same area so both grabs occur at the same time
        Weather? intervalWeather = JsonConvert.DeserializeObject<Weather>(detailedJson); //stores interval stuff
        Weather? hourlyWeather = JsonConvert.DeserializeObject<Weather>(simpleJson); //stores hourly stuff


        // if (intervalWeather != null) Console.WriteLine(intervalWeather.properties.periods[0].temperature);
        // if (hourlyWeather != null) Console.WriteLine(hourlyWeather.properties.periods[0].temperature);

        Console.WriteLine("Downloading observation data");
        
        //TODO: put obs downloads here

        if (hourlyWeather == null || intervalWeather == null)
        {
            throw new HttpRequestException("Failed to retrieve weatherData data from NOAA API");
        }

        Console.WriteLine("Completed Data fetch!");

        return new[] {intervalWeather, hourlyWeather}; //by time of day interval, and hourly
        //TODO: write class and run downloads for observation data
        //TODO: add some conf ability for setting location, could be presets or from IP
    }

    private static async Task<string> getjsonStream(string url)
    {
        HttpClient client = new();
        client.DefaultRequestHeaders.Add("User-Agent", "Rpi-weatherData-station");
        HttpResponseMessage response = await client.GetAsync(url);
        string content = await response.Content.ReadAsStringAsync();
        return content;
    }

    //UI EVENTS
}