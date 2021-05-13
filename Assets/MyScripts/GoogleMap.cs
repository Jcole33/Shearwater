using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class GoogleMap : MonoBehaviour {
    private FlightDataStorage dataStream;
    private string APIKey = "AIzaSyBONX6_IxRXkEMp0OG7gHw9qWyIWOm5qro";
    private Renderer myRenderer;
    string url;
    private string lat;
    private string lon;
    LocationInfo li;

    public int zoom = 5;
    public int mapWidth = 640;
    public int mapHeight = 640;

    public enum MapType { roadmap, satellite, hybrid, terrain }
    public MapType mapSelected;
    public int scale = 1;
    MarkerManager markerManager;
    String urlMarkers;
    String urlEnding;
    IEnumerator Map()
    {
        lat = dataStream.get("latitude");
        lon = dataStream.get("longitude");
        url = "https://maps.googleapis.com/maps/api/staticmap?center=" + lat + "," + lon
        + "&markers=color:blue%7Clabel:A%7C" + lat + "," + lon
        + urlMarkers + "&zoom=" + zoom + urlEnding;
        Debug.Log(url);  
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        myRenderer = GetComponent<Renderer>();
        myRenderer.material.mainTexture = DownloadHandlerTexture.GetContent(www);
    }
    void UpdateMap() {
        StartCoroutine(Map());
    }
    void Start()
    {
        GameObject streamObject = GameObject.Find("FlightDataStorage");
        dataStream = streamObject.GetComponent<FlightDataStorage>();
        GameObject markerObject = GameObject.Find("MarkerManager");
        markerManager = markerObject.GetComponent<MarkerManager>();
    }

    // Update is called once per frame
    void Update() {
        if (OVRInput.GetDown(OVRInput.RawButton.X) && zoom > 1) {
            --zoom;
            UpdateMap();
        } else if (OVRInput.GetDown(OVRInput.RawButton.Y) && zoom < 22) {
            ++zoom;
            UpdateMap();
        }
    }
    public void Initialize() {
        urlMarkers = "&markers=color:purple%7Clabel:H%7C" + markerManager.markers[0].latitude + "," + markerManager.markers[0].longitude;
        for (var i = 1; i < markerManager.markers.Length; ++i) {
            urlMarkers += "&markers=color:red%7Clabel:" + markerManager.markers[i].name.Substring(0,1).ToUpper() + "%7C" + markerManager.markers[i].latitude + "," + markerManager.markers[i].longitude;
        }
        urlEnding = "&size=" + mapWidth + "x" + mapHeight + "&scale=" + scale + "&maptype=" + mapSelected + "&key=" + APIKey;
        InvokeRepeating("UpdateMap", 5.0f, 5.0f);
    }
}
