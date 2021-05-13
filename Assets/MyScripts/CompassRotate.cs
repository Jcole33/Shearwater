using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class CompassRotate : MonoBehaviour
{
    FlightDataStorage dataStream;
    RectTransform compass;
    // Start is called before the first frame update
    void Start()
    {
        GameObject gameObject = GameObject.Find("FlightDataStorage");
        dataStream = gameObject.GetComponent<FlightDataStorage>();
        compass = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        compass.transform.eulerAngles = new Vector3( compass.transform.eulerAngles.x, compass.transform.eulerAngles.y, float.Parse(dataStream.get("heading") ) );
    }
}

