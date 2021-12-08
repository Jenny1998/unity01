using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class Main : MonoBehaviour
{
    public GameObject coin;
    public GameObject grass;
    private float movementSpeed = 1f;
    public GameObject weatherCoin;
    public GameObject trafficCoin;

    public float currentWeather;
    public float traffiCondition; 

    void Awake()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("example");
        var coinNum = Convert.ToInt32(Convert.ToString(data[0]["number"]));
        var grassNum = Convert.ToInt32(Convert.ToString(data[1]["number"]));

        for (var i = 0; i < coinNum; i++)
        {
            Instantiate(coin, new Vector3(0, i * 3, 0), Quaternion.identity);
        }

        for (var i = 0; i < grassNum; i++)
        {
            Instantiate(grass, new Vector3(i * 3, i, 0), Quaternion.identity);
        }
    }

    // Use this for initialization
    void Start()
    {
        StartCoroutine(GetTrafficData("https://api.tomtom.com/traffic/services/4/flowSegmentData/relative0/10/json?point=33.772600%2C-84.395857&key=SzYpTfs2ASwQ58mGyIF0GBZgLXOOP1Yd"));
        StartCoroutine(GetWeatherData("http://localhost:8000/"));
        weatherCoin = Instantiate(coin, new Vector3(0, 3, 0), Quaternion.identity);
        trafficCoin = Instantiate(coin, new Vector3(0, 1, 0), Quaternion.identity);


    }

    IEnumerator GetWeatherData(string address)
    {
        UnityWebRequest www = UnityWebRequest.Get(address);
        Debug.Log(www);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong: " + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            ProcessWeatherCondition(www.downloadHandler.text);

        }
    }

    IEnumerator GetTrafficData(string address)
    {
        UnityWebRequest www = UnityWebRequest.Get(address);
        Debug.Log(www);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong: " + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            ProcessTrfficCondition(www.downloadHandler.text);
        }
    }

    void ProcessTrfficCondition(string rawResponse)
    {
        JSONNode node = JSON.Parse(rawResponse);
        var currenctSpeed = node["flowSegmentData"]["currentSpeed"];
        var freeFlowSpeed = node["flowSegmentData"]["freeFlowSpeed"];
        traffiCondition = currenctSpeed / freeFlowSpeed;
    }

    void ProcessWeatherCondition(string rawResponse)
    {
        JSONNode node = JSON.Parse(rawResponse);
        currentWeather = node["current"]["temp_c"];
        Debug.Log(currentWeather);
    }

    // Update is called once per frame
    void Update()
    {
        //get the Input from Horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");
        //get the Input from Vertical axis
        float verticalInput = Input.GetAxis("Vertical");
        Debug.Log(verticalInput);
        Debug.Log(horizontalInput);

        //update the position based on the temperature 
        weatherCoin.transform.position = weatherCoin.transform.position + new Vector3((horizontalInput + 1) * (currentWeather / 10) * Time.deltaTime, verticalInput * (currentWeather / 10) * Time.deltaTime, 0);

        //update the position based on the traffic condition  
        trafficCoin.transform.position = trafficCoin.transform.position + new Vector3((horizontalInput + 1) * (traffiCondition ) * Time.deltaTime, verticalInput * (traffiCondition) * Time.deltaTime, 0);


    }
}