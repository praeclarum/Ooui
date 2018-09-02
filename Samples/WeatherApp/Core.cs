using System;
using System.Threading.Tasks;

namespace WeatherApp
{
    public class Core
    {
        public static async Task<Weather> GetWeather(string zipCode, string units = "kelvin")
        {
            //Sign up for a free API key at http://openweathermap.org/appid
            string key = "fc9f6c524fc093759cd28d41fda89a1b";
            string queryString = "http://api.openweathermap.org/data/2.5/weather?zip="
                + zipCode + "&appid=" + key + "&units=" + units;

            var results = await DataService.getDataFromService(queryString).ConfigureAwait(false);

            if (results["weather"] != null)
            {
                string tempUnit = GetTempUnit(units);
                string speedUnit = GetSpeedUnit(units);
                Weather weather = new Weather
                {
                    Title = (string)results["name"],
                    Temperature = (string)results["main"]["temp"] + tempUnit,
                    Wind = (string)results["wind"]["speed"] + speedUnit,
                    Humidity = (string)results["main"]["humidity"] + " %",
                    Visibility = (string)results["weather"][0]["main"]
                };

                DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                DateTime sunrise = time.AddSeconds((double)results["sys"]["sunrise"]);
                DateTime sunset = time.AddSeconds((double)results["sys"]["sunset"]);
                weather.Sunrise = sunrise.ToString() + " UTC";
                weather.Sunset = sunset.ToString() + " UTC";
                return weather;
            }
            else
            {
                return null;
            }
        }

        private static string GetSpeedUnit(string units)
        {
            switch (units)
            {
                case "imperial":
                    return " mph";
                default:
                    return " kph";
            }
        }

        private static string GetTempUnit(string units)
        {
            switch (units)
            {
                case "metric":
                    return " °C";
                case "imperial":
                    return " °F";
                default:
                    return " °K";
            }
        }
    }
}
