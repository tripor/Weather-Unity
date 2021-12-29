using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;

public class WeatherNetwork
{

    public enum WeatherTypes
    {
        Clear = 0,
        Strom = 1,
        Drizzle = 2,
        LightRain = 3,
        Rain = 4,
        HeavyRain = 5,
        Snow = 6,
        FewClouds = 7,
        Clouds = 8,
    }

    private ExternalIPResponse currentIP;
    private ExternalLocationInformation externalLocationInformation;
    private Dictionary<int, ExternalWeatherInformation> weatherDictionary;

    public delegate void WeatherData(WeatherTypes type, float currentTemp, float minTemp, float maxTemp);
    public static event WeatherData OnWeatherDataReceived;
    public delegate void WeatherError(string message);
    public static event WeatherError OnWeatherDataError;
    public delegate void WeatherLocationName(string location);
    public static event WeatherLocationName OnWeatherLocationName;
    public delegate void WeatherProccessingDone();
    public static event WeatherProccessingDone OnWeatherProccessingDone;

    public WeatherNetwork()
    {
        weatherDictionary = new Dictionary<int, ExternalWeatherInformation>();
    }




    public IEnumerator ProcessWeather()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("https://api.ipify.org?format=json");
        string text;
        ExternalWeatherInformation externalWeatherInformation;
        bool problems = true;
#if PLATFORM_ANDROID
        problems = false;
        if (!Input.location.isEnabledByUser)
        {
            OnWeatherDataError("Not enough device permissions, using IP location");
            problems = true;
        }
        if (!problems)
        {

            Input.location.Start();

            // Waits until the location service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }
            // If the service didn't initialize in 20 seconds this cancels location service use.
            if (maxWait < 1)
            {
                OnWeatherDataError("Location service not initialized, using IP location");
                problems = true;
            }
            if (!problems)
            {
                if (Input.location.status == LocationServiceStatus.Failed)
                {
                    // If device localion is unavailable fallback to ip location
                    OnWeatherDataError("Unable to determine device location, using IP location");
                    problems = true;
                }
                else
                {
                    webRequest = UnityWebRequest.Get("http://www.geoplugin.net/extras/location.gp?lat=" + Input.location.lastData.latitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture) + "&lon=" + Input.location.lastData.longitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture) + "&format=json");
                    yield return webRequest.SendWebRequest();
                    try
                    {
                        text = GetResult(webRequest);
                        var externalLocationInformation = JsonUtility.FromJson<ExternalLocationInformationSimpler>(text);
                        if (externalLocationInformation == null || externalLocationInformation.geoplugin_place == null) throw new DataProcessingError("There was a problem processing weather data");

                        OnWeatherLocationName(externalLocationInformation.geoplugin_place);

                        webRequest = UnityWebRequest.Get("http://api.openweathermap.org/data/2.5/weather?lat=" + Input.location.lastData.latitude + "&lon=" + Input.location.lastData.longitude + "&appid=d67b3b963691d6ea4b8f646ac3fb3337");
                    }
                    catch (Exception)
                    {
                        OnWeatherDataError("Unable to determine device location, please retry");
                        yield break;
                    }


                }
            }
            Input.location.Stop();
        }

#endif
        if (problems)
        {
            // Get device IP
            webRequest = UnityWebRequest.Get("https://api.ipify.org?format=json");
            yield return webRequest.SendWebRequest();
            try
            {
                text = GetResult(webRequest);
                // Tranform response to class
                currentIP = JsonUtility.FromJson<ExternalIPResponse>(text);

                if (currentIP == null) throw new DataProcessingError("There was a problem processing weather data");
                // Get the geo location by ip
                webRequest = UnityWebRequest.Get("http://www.geoplugin.net/json.gp?ip=" + currentIP);
            }
            catch (Exception ex)
            {
                OnWeatherDataError(ex.ToString());
                yield break;
            }
            yield return webRequest.SendWebRequest();
            try
            {
                text = GetResult(webRequest);
                // Transform geo location json to class
                externalLocationInformation = JsonUtility.FromJson<ExternalLocationInformation>(text);
                if (externalLocationInformation == null || externalLocationInformation.geoplugin_city == null || externalLocationInformation.geoplugin_latitude == null || externalLocationInformation.geoplugin_longitude == null) throw new DataProcessingError("There was a problem processing weather data");
                OnWeatherLocationName(externalLocationInformation.geoplugin_city);


                // Get weather forecast
                webRequest = UnityWebRequest.Get("http://api.openweathermap.org/data/2.5/weather?lat=" + externalLocationInformation.geoplugin_latitude + "&lon=" + externalLocationInformation.geoplugin_longitude + "&appid=d67b3b963691d6ea4b8f646ac3fb3337");
            }
            catch (Exception ex)
            {
                OnWeatherDataError(ex.ToString());
                yield break;
            }
        }

        yield return webRequest.SendWebRequest();
        try
        {
            text = GetResult(webRequest);

            // Transform weather information from json to class
            externalWeatherInformation = JsonUtility.FromJson<ExternalWeatherInformation>(text);
            ProcessWeatherDataTypes(externalWeatherInformation);
        }
        catch (Exception ex)
        {
            OnWeatherDataError(ex.ToString());
            yield break;
        }
        OnWeatherProccessingDone();

    }
    public IEnumerator ProcessWeather(int cityID)
    {
        if (weatherDictionary.ContainsKey(cityID))
        {
            var info = weatherDictionary[cityID];
            OnWeatherLocationName(info.name);
            ProcessWeatherDataTypes(info);
        }
        else
        {
            UnityWebRequest webRequest = UnityWebRequest.Get("http://api.openweathermap.org/data/2.5/weather?id=" + cityID + "&appid=d67b3b963691d6ea4b8f646ac3fb3337");
            yield return webRequest.SendWebRequest();
            try
            {
                string text = GetResult(webRequest);

                // Transform weather information from json to class
                ExternalWeatherInformation externalWeatherInformation = JsonUtility.FromJson<ExternalWeatherInformation>(text);
                if (externalWeatherInformation == null || externalWeatherInformation.name == null) throw new DataProcessingError("There was a problem processing weather data");
                OnWeatherLocationName(externalWeatherInformation.name);
                ProcessWeatherDataTypes(externalWeatherInformation);
                weatherDictionary.Add(cityID, externalWeatherInformation);
            }
            catch (Exception ex)
            {
                OnWeatherDataError(ex.ToString());
                yield break;
            }
        }
        OnWeatherProccessingDone();
    }


    private void ProcessWeatherDataTypes(in ExternalWeatherInformation info)
    {
        if (info == null || info.weather == null || info.main == null)
        {
            throw new DataProcessingError("There was a problem processing weather data");
        }
        else
        {
            if (info.cod == 200 && info.weather?.Length >= 1)
            {
                foreach (ExternalWeather weather in info.weather)
                {
                    if (weather.id == 800)
                    {
                        OnWeatherDataReceived(WeatherTypes.Clear, info.main.temp, info.main.temp_min, info.main.temp_max);
                    }
                    else if (weather.id == 801 || weather.id == 802)
                    {
                        OnWeatherDataReceived(WeatherTypes.FewClouds, info.main.temp, info.main.temp_min, info.main.temp_max);
                    }
                    else if (weather.id == 803 || weather.id == 804)
                    {
                        OnWeatherDataReceived(WeatherTypes.Clouds, info.main.temp, info.main.temp_min, info.main.temp_max);
                    }
                    else if (weather.id >= 200 && weather.id < 300)
                    {
                        OnWeatherDataReceived(WeatherTypes.HeavyRain, info.main.temp, info.main.temp_min, info.main.temp_max);
                    }
                    else if (weather.id >= 300 && weather.id < 400)
                    {
                        OnWeatherDataReceived(WeatherTypes.Drizzle, info.main.temp, info.main.temp_min, info.main.temp_max);
                    }
                    else if (weather.id == 500 || weather.id == 511 || weather.id == 520 || weather.id == 531)
                    {
                        OnWeatherDataReceived(WeatherTypes.LightRain, info.main.temp, info.main.temp_min, info.main.temp_max);
                    }
                    else if (weather.id == 501 || weather.id == 521)
                    {
                        OnWeatherDataReceived(WeatherTypes.Rain, info.main.temp, info.main.temp_min, info.main.temp_max);
                    }
                    else if (weather.id == 502 || weather.id == 503 || weather.id == 504 || weather.id == 522)
                    {
                        OnWeatherDataReceived(WeatherTypes.HeavyRain, info.main.temp, info.main.temp_min, info.main.temp_max);
                    }
                    else if (weather.id >= 600 && weather.id < 700)
                    {
                        OnWeatherDataReceived(WeatherTypes.Snow, info.main.temp, info.main.temp_min, info.main.temp_max);
                    }
                    else if (weather.id >= 700 && weather.id < 800)
                    {
                        OnWeatherDataReceived(WeatherTypes.Rain, info.main.temp, info.main.temp_min, info.main.temp_max);
                    }
                }
            }
        }

    }

    private string GetResult(UnityWebRequest webRequest)
    {
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            throw new NetworkError(webRequest.result);
        }
        return webRequest.downloadHandler.text;
    }




    [Serializable]
    private class ExternalIPResponse
    {
        public string ip = null;
    }
    [Serializable]
    private class ExternalLocationInformation
    {
        public string geoplugin_request = null;
        public int geoplugin_status = 0;
        public string geoplugin_delay = null;
        public string geoplugin_credit = null;
        public string geoplugin_city = null;
        public string geoplugin_region = null;
        public string geoplugin_regionCode = null;
        public string geoplugin_regionName = null;
        public string geoplugin_areaCode = null;
        public string geoplugin_dmaCode = null;
        public string geoplugin_countryCode = null;
        public string geoplugin_countryName = null;
        public string geoplugin_place = null;
        public int geoplugin_inEU = 0;
        public int geoplugin_euVATrate = 0;
        public string geoplugin_continentCode = null;
        public string geoplugin_continentName = null;
        public string geoplugin_latitude = null;
        public string geoplugin_longitude = null;
        public string geoplugin_locationAccuracyRadius = null;
        public string geoplugin_timezone = null;
        public string geoplugin_currencyCode = null;
        public string geoplugin_currencySymbol = null;
        public string geoplugin_currencySymbol_UTF8 = null;
        public float geoplugin_currencyConverter = 0;
    }
    [Serializable]
    private class ExternalLocationInformationSimpler
    {
        public string geoplugin_place = null;
        public string geoplugin_countryCode = null;
        public string geoplugin_region = null;
        public string geoplugin_regionAbbreviated = null;
        public string geoplugin_county = null;
        public string geoplugin_latitude = null;
        public string geoplugin_longitude = null;
        public float geoplugin_distanceMiles = 0;
        public float geoplugin_distanceKilometers = 0;
    }
    [Serializable]
    private class ExternalWeatherInformation
    {
        public ExternalCoord coord = null;
        public ExternalWeather[] weather = null;
        public ExternalMain main = null;
        public int visibility = 0;
        public ExternalWind wind = null;
        public ExternalClouds clouds = null;
        public int dt = 0;
        public ExternalSys sys = null;
        public int timezone = 0;
        public int id = 0;
        public string name = null;
        public int cod = 200;
    }
    [Serializable]
    private class ExternalCoord
    {
        public float lon = 0;
        public float lat = 0;
    }
    [Serializable]
    private class ExternalWeather
    {
        public int id = 0;
        public string main = null;
        public string description = null;
        public string icon = null;
    }
    [Serializable]
    private class ExternalMain
    {
        public float temp = 0;
        public float feels_like = 0;
        public float temp_min = 0;
        public float temp_max = 0;
        public int pressure = 0;
        public int humidity = 0;
        public string main = null;
        public string description = null;
        public string icon = null;
    }
    [Serializable]
    private class ExternalWind
    {
        public float speed = 0;
        public int deg = 0;
    }
    [Serializable]
    private class ExternalClouds
    {
        public int all = 0;
    }
    [Serializable]
    private class ExternalSys
    {
        public int type = 0;
        public int id = 0;
        public float message = 0;
        public string country = null;
        public int sunrise = 0;
        public int sunset = 0;
    }
}
