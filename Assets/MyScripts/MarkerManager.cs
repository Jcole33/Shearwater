using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class MarkerManager : MonoBehaviour {
    public PositionARMarker[] markers;
    String fileName = "Markers.txt";
    FlightDataStorage dataStream;
    PositionARMarker homeMarker;
    GoogleMap map;
    bool initialized = false;
    public Material homeMaterial;
    // Start is called before the first frame update
    void Start()
    {
        ImportMarkerList();
        GameObject gameObject = GameObject.Find("FlightDataStorage");
        dataStream = gameObject.GetComponent<FlightDataStorage>();
        map = GameObject.Find("MapPlane").GetComponent<GoogleMap>();
    }

    // Update is called once per frame
    void Update() {
        if (!initialized && dataStream.initialPositionRecieved) Initialize();
    }
    private void ImportMarkerList() {
        var markerObject = GameObject.Find("Marker");
        homeMarker = markerObject.GetComponent<PositionARMarker>();
        if (System.IO.File.Exists(Application.persistentDataPath + "/" + fileName)) {
            var sr = new StreamReader(Application.persistentDataPath + "/" + fileName);
            var fileContents = sr.ReadToEnd();
            sr.Close();
            var lines = fileContents.Split("\n"[0]);
            markers = new PositionARMarker[lines.Length]; //home point must also be added
            for (var i = 0; i < lines.Length - 1; ++i) {
                var data = lines[i].Split("\t"[0]);
                //home point is on index 0
                markers[i + 1] = SetupMarker(Convert.ToDouble(data[0]), Convert.ToDouble(data[1]), 50, data[2], GameObject.Instantiate(markerObject).GetComponent<PositionARMarker>());
            } 
        } else {
            markers = new PositionARMarker[1];
        }
        markerObject.GetComponent<MeshRenderer>().material = homeMaterial;
    }
    public void Initialize() {
        markers[0] = SetupMarker(Convert.ToDouble(dataStream.get("latitude")), Convert.ToDouble(dataStream.get("longitude")), 0, "Home", homeMarker);
        map.Initialize();
        initialized = true;
    }
    private PositionARMarker SetupMarker(double latitude, double longitude, double altitude, string name, PositionARMarker markerComponent) {
        markerComponent.latitude = latitude;
        markerComponent.longitude = longitude;
        markerComponent.altitude = altitude;
        markerComponent.name = name;
        return markerComponent;
    }
}
