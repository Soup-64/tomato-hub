using System;

namespace avalonia_rider_test;

[Serializable]
public class Weather
{
    public properties properties {get; set;}
}

[Serializable]
public class properties
{
    public period[] periods;
}

[Serializable]
public class period
{
    public int number { get; set; }
    public string name { get; set; }
    public bool isDaytime { get; set; }
    public int temperature { get; set; }
    public string temperatureTrend { get; set; }
    public string windSpeed { get; set; }
    public string windDirection { get; set; }
    public string shortForecast { get; set; }
    public string detailedForecast { get; set; }
}