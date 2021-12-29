using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeatherController : MonoBehaviour
{
    public GameObject cloudParent;
    public GameObject cloudPrefab;
    public ParticleSystem rainSystem;
    public GameObject sunObject;
    public TextMeshPro locationText3D;
    public TextMeshProUGUI locationTextUI;
    public TextMeshProUGUI tempInfo;
    public TextMeshProUGUI weatherText;
    public Animator weatherAnimator;

    [SerializeField]
    private int totalNumberOfClouds = 20;

    private List<GameObject> clouds;
    private ParticleSystem.EmissionModule rainEmission;

    // Start is called before the first frame update
    void Start()
    {
        rainEmission = rainSystem.emission;
        WeatherNetwork.OnWeatherDataReceived += OnWeatherDataBroadcast;
        WeatherNetwork.OnWeatherLocationName += OnWeatherLocationName;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        GenerateClouds();
        sunObject.SetActive(false);
        rainEmission.rateOverTime = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Update the scene based on the type of weather
    /// </summary>
    /// <param name="type"></param>
    /// <param name="currentTemp"></param>
    /// <param name="minTemp"></param>
    /// <param name="maxTemp"></param>
    private void OnWeatherDataBroadcast(WeatherNetwork.WeatherTypes type, float currentTemp, float minTemp, float maxTemp)
    {
        float currentTemperature = currentTemp - 272.15f;
        float minimumTemperature = minTemp - 272.15f;
        float maximumTemperature = maxTemp - 272.15f;
        tempInfo.text = $"Current temperature: {currentTemperature.ToString("F1")}ºC\nMinimum temperature: {minimumTemperature.ToString("F1")}ºC\nMaximum temperature: {maximumTemperature.ToString("F1")}ºC";
        if (type == WeatherNetwork.WeatherTypes.Clear)
        {
            sunObject.SetActive(true);
            ActivateClouds(0f);
            rainEmission.rateOverTime = 0;
            SetCurrentWeatherText("Clear");
            weatherAnimator.SetInteger("WeatherType", 1);
        }
        else if (type == WeatherNetwork.WeatherTypes.Clouds)
        {
            sunObject.SetActive(true);
            ActivateClouds(1f);
            rainEmission.rateOverTime = 0;
            SetCurrentWeatherText("Clouds");
            weatherAnimator.SetInteger("WeatherType", 0);
        }
        else if (type == WeatherNetwork.WeatherTypes.Drizzle)
        {
            sunObject.SetActive(true);
            ActivateClouds(0.5f);
            rainEmission.rateOverTime = 5;
            SetCurrentWeatherText("Drizzle");
            weatherAnimator.SetInteger("WeatherType", 2);
        }
        else if (type == WeatherNetwork.WeatherTypes.FewClouds)
        {
            sunObject.SetActive(true);
            ActivateClouds(0.25f);
            rainEmission.rateOverTime = 0;
            SetCurrentWeatherText("Low Clouds");
            weatherAnimator.SetInteger("WeatherType", 0);
        }
        else if (type == WeatherNetwork.WeatherTypes.HeavyRain)
        {
            sunObject.SetActive(false);
            ActivateClouds(1f);
            rainEmission.rateOverTime = 50;
            SetCurrentWeatherText("Heavy Rain");
            weatherAnimator.SetInteger("WeatherType", 2);
        }
        else if (type == WeatherNetwork.WeatherTypes.LightRain)
        {
            sunObject.SetActive(true);
            ActivateClouds(1f);
            rainEmission.rateOverTime = 15;
            SetCurrentWeatherText("Light Rain");
            weatherAnimator.SetInteger("WeatherType", 2);
        }
        else if (type == WeatherNetwork.WeatherTypes.Rain)
        {
            sunObject.SetActive(false);
            ActivateClouds(1f);
            rainEmission.rateOverTime = 30;
            SetCurrentWeatherText("Rain");
            weatherAnimator.SetInteger("WeatherType", 2);
        }
        else if (type == WeatherNetwork.WeatherTypes.Snow)
        {
            sunObject.SetActive(false);
            ActivateClouds(1f);
            rainEmission.rateOverTime = 30;
            SetCurrentWeatherText("Snow");
            weatherAnimator.SetInteger("WeatherType", 2);
        }
        else if (type == WeatherNetwork.WeatherTypes.Strom)
        {
            sunObject.SetActive(false);
            ActivateClouds(1f);
            rainEmission.rateOverTime = 50;
            SetCurrentWeatherText("Storm");
            weatherAnimator.SetInteger("WeatherType", 2);
        }
        else
        {
            sunObject.SetActive(false);
            ActivateClouds(0f);
            rainEmission.rateOverTime = 0;
            SetCurrentWeatherText("No weather");
            weatherAnimator.SetInteger("WeatherType", 0);
        }
    }

    private void SetCurrentWeatherText(string weather)
    {
        weatherText.text = "Current weather: " + weather;
    }
    private void OnWeatherLocationName(string location)
    {
        locationText3D.text = location;
        locationTextUI.text = "Location: " + location;
    }
    private void OnSceneUnloaded(Scene current)
    {
        WeatherNetwork.OnWeatherDataReceived -= OnWeatherDataBroadcast;
        WeatherNetwork.OnWeatherLocationName -= OnWeatherLocationName;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
    /// <summary>
    /// Activate a percentage of the generated clouds
    /// </summary>
    /// <param name="amount">The percentage of cloud to be shown from the total generated</param>
    private void ActivateClouds(float amount)
    {
        var numberToActivate = Mathf.RoundToInt(totalNumberOfClouds * amount);
        foreach (GameObject cloud in clouds)
        {
            if (numberToActivate > 0)
                cloud.SetActive(true);
            else
                cloud.SetActive(false);
            numberToActivate--;
        }
    }
    /// <summary>
    /// Generate all the cloud to be used for the different types of weather
    /// </summary>
    private void GenerateClouds()
    {
        List<GameObject> cloudToRemove = new List<GameObject>();
        clouds = new List<GameObject>();
        foreach (Transform item in cloudParent.transform)
        {
            if (item.tag == "Cloud")
            {
                cloudToRemove.Add(item.gameObject);
            }
        }
        foreach (var cloud in cloudToRemove)
        {
            Destroy(cloud);
        }
        for (int i = 0; i < totalNumberOfClouds; i++)
        {
            var newCloud = Instantiate(cloudPrefab);
            newCloud.transform.parent = cloudParent.transform;
            newCloud.transform.position = new Vector3(Random.Range(-2f, 2f), cloudParent.transform.position.y, Random.Range(-2f, 2f));
            clouds.Add(newCloud);
            newCloud.SetActive(false);
        }
    }
}
