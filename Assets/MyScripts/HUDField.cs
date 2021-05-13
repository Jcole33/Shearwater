using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class HUDField : MonoBehaviour
{
    public String fieldText;
    public String fieldProperty;
    public String units = "";
    Text text;
    FlightDataStorage dataStream;
    // Start is called before the first frame update
    void Start()
    {
        GameObject gameObject = GameObject.Find("FlightDataStorage");
        dataStream = gameObject.GetComponent<FlightDataStorage>();
        text = GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        text.text = fieldText + ": " + dataStream.get(fieldProperty) + ( units == "" ? "" : " (" + units + ")" );
    }
}
