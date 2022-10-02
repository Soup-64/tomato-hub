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

        t = new(60000); //periodic data check
        t.Elapsed += doWeatherUpdate;
        t.Enabled = false;
    }

    private void doWeatherUpdate(object? sender, ElapsedEventArgs e)
    {
        if (sender != null)
        {
            weatherData = downloadWeather();
            weatherData.Wait(); //wait so code isn't immediately trying to load data from incomplete data
        }

        Dispatcher.UIThread.Post(() =>
        {
            TextBox bigTemp = this.Find<TextBox>("bigTemp");

            if (weatherData.IsCompletedSuccessfully)
            {
                try //because of one weird random crash that was probably a corrupted upload
                {
                    updateInterface();
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                bigTemp.Text = "waiting for stuff...";
            }
        }, DispatcherPriority.Background);
    }

    private void updateInterface()
    {
        TextBox bigTemp = this.Find<TextBox>("bigTemp");
        TextBox forecastNow = this.Find<TextBox>("Forecast");
        TextBox[] forecast = new TextBox[7];
        for (int i = 0; i < 7; i ++)
        {
            forecast[i] = this.Find<TextBox>($"{i}");
            forecast[i].Text = weatherData.Result[0].properties.periods[i * 2].name + "\n";
            forecast[i].Text += weatherData.Result[0].properties.periods[i * 2].temperature + "\u00B0 \n";
            forecast[i].Text += weatherData.Result[0].properties.periods[i * 2].windSpeed + " ";
            forecast[i].Text += weatherData.Result[0].properties.periods[i * 2].windDirection;
        }

        forecastNow.Text = weatherData.Result[0].properties.periods[0].detailedForecast;
        bigTemp.Text = weatherData.Result[1].properties.periods[0].temperature + "\u00B0";
    }


    private static async Task<Weather[]> downloadWeather()
    {
        //houghton: "https://api.weather.gov/gridpoints/MQT/114,95/forecast (/hourly)
        //currently just grabs weather data for this hard coded location until I figure 
        //out a location selection menu, which means more api calls to noaa to translate coordinates to a gridpoint station

#if DEBUG
        Console.WriteLine("Downloading forecast data");
#endif
        string detailedJson = await getjsonStream("https://api.weather.gov/gridpoints/MQT/114,95/forecast");
        string simpleJson = await getjsonStream("https://api.weather.gov/gridpoints/MQT/114,95/forecast/hourly");

        //waits are done in the same area so both grabs occur at the same time
        Weather? intervalWeather = JsonConvert.DeserializeObject<Weather>(detailedJson); //stores interval stuff
        Weather? hourlyWeather = JsonConvert.DeserializeObject<Weather>(simpleJson); //stores hourly stuff


        // if (intervalWeather != null) Console.WriteLine(intervalWeather.properties.periods[0].temperature);
        // if (hourlyWeather != null) Console.WriteLine(hourlyWeather.properties.periods[0].temperature);


        //TODO: put obs downloads here

        if (hourlyWeather == null || intervalWeather == null)
        {
            throw new HttpRequestException("Failed to retrieve weatherData data from NOAA API");
        }

#if DEBUG
        Console.WriteLine("Completed Data fetch!");
#endif
        
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
