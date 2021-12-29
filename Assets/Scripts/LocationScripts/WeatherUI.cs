using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WeatherUI : MonoBehaviour
{

    public Button[] uiButtons = new Button[6];
    public GameObject errorText;
    public TextMeshProUGUI timeText;

    private WeatherNetwork weatherNetwork;


    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        WeatherNetwork.OnWeatherDataError += OnWeatherError;
        WeatherNetwork.OnWeatherProccessingDone += OnWeatherDone;
        weatherNetwork = new WeatherNetwork();
        ChangeButtonsInteract(false);
        errorText.SetActive(false);
        StartCoroutine(weatherNetwork.ProcessWeather());
        timeText.text = "Current time: " + System.DateTime.Now.ToString("dd-MM-yy HH:mm:ss");
    }

    // Update is called once per frame
    void Update()
    {
        timeText.text = "Current time: " + System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
    }
    /// <summary>
    /// Button click event
    /// </summary>
    /// <param name="locationNumber"></param>
    public void SelectLocation(int locationNumber)
    {
        ChangeButtonsInteract(false);
        errorText.SetActive(false);
        if (locationNumber == 0)
        {
            StartCoroutine(weatherNetwork.ProcessWeather());
        }
        else if (locationNumber == 1)
            StartCoroutine(weatherNetwork.ProcessWeather(2267056));
        else if (locationNumber == 2)
            StartCoroutine(weatherNetwork.ProcessWeather(2267094));
        else if (locationNumber == 3)
            StartCoroutine(weatherNetwork.ProcessWeather(2740636));
        else if (locationNumber == 4)
            StartCoroutine(weatherNetwork.ProcessWeather(2735941));
        else if (locationNumber == 5)
            StartCoroutine(weatherNetwork.ProcessWeather(2268337));
    }

    private void OnWeatherError(string message)
    {
        errorText.SetActive(true);
        ChangeButtonsInteract(true);
        errorText.GetComponent<TextMeshProUGUI>().text = "Error: " + message;
    }

    private void OnWeatherDone()
    {
        ChangeButtonsInteract(true);
    }

    private void ChangeButtonsInteract(bool state)
    {
        for (int i = 0; i < 6; i++)
        {
            uiButtons[i].interactable = state;
        }
    }
    private void OnSceneUnloaded(Scene current)
    {
        WeatherNetwork.OnWeatherDataError -= OnWeatherError;
        WeatherNetwork.OnWeatherProccessingDone -= OnWeatherDone;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
}
