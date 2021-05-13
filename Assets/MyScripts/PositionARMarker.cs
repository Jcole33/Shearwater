using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionARMarker : MonoBehaviour
{
    private FlightDataStorage dataStream;
    public double latitude = 0;
    public double longitude = 0;
    public double altitude = 0;
    public string name = "";
    public double distanceToMarker = 0;
    // Start is called before the first frame update
    void Start()
    {
        GameObject streamObject = GameObject.Find("FlightDataStorage");
        dataStream = streamObject.GetComponent<FlightDataStorage>();
    }
    public double GetDistanceFromLatLonInKm(double lat1, double lon1, double lat2, double lon2) {
        var R = 6371; // Radius of the earth in km
        var dLat = ConvertToRadian(lat2-lat1);  
        var dLon = ConvertToRadian(lon2-lon1); 
        var a = Math.Sin(dLat/2) * Math.Sin(dLat/2) + Math.Cos(ConvertToRadian(lat1)) * Math.Cos(ConvertToRadian(lat2)) * Math.Sin(dLon/2) * Math.Sin(dLon/2); 
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a)); 
        var d = R * c; // Distance in km
        return d;
    }

    double ConvertToRadian(double deg) {
        return deg * (Math.PI/180);
    }
    // Update is called once per frame
    void Update() {
        if (dataStream.get("altitude") != "") {
            //find intersection between line from true marker location and origin and the sphere surrounding the origin
            var radius = 10;
            var droneLatitude = Convert.ToDouble(dataStream.get("latitude"));
            var droneLongitude = Convert.ToDouble(dataStream.get("longitude"));
            var relativeLatitude = (latitude < droneLatitude ? -1 : 1) * GetDistanceFromLatLonInKm(droneLatitude, 0, latitude, 0) * 1000;
            var relativeLongitude = (longitude < droneLongitude ? -1 : 1) * GetDistanceFromLatLonInKm(0, droneLongitude, 0, longitude) * 1000;
            var relativeAltitude = altitude - Convert.ToDouble(dataStream.get("altitude"));
            var t = Math.Sqrt(Math.Pow(radius, 2)/(Math.Pow(relativeLongitude, 2) + Math.Pow(relativeAltitude, 2) + Math.Pow(relativeLatitude, 2)));
            var x = relativeLongitude * t;
            var y = relativeAltitude * t;
            var z = relativeLatitude * t;
            
            //adjust based on pitch
            var pitch = Convert.ToDouble(dataStream.get("roll"));
            (z, y) = Rotate(z, y, pitch);
            //adjust based on roll
            var roll = Convert.ToDouble(dataStream.get("roll"));
            (x, y) = Rotate(x, y, roll - 360);
            //adjust displayed marker based on heading
            var heading = Convert.ToDouble(dataStream.get("heading"));
            (x, z) = Rotate(x, y, roll - 360);
            //update location
            transform.position = new Vector3((float) x, (float) y, (float) z);
            distanceToMarker = GetDistanceFromLatLonInKm(droneLatitude, droneLongitude, latitude, longitude);
        }
    }
    public (double, double) Rotate(double unadjustedX, double unadjustedY, double degree) {
        var adjustRad = ConvertToRadian(degree);
        var cos = Math.Cos(adjustRad);
        var sin = Math.Sin(adjustRad);
        var x = unadjustedX*cos - unadjustedY*sin;
        var y = unadjustedY*cos + unadjustedX*sin;
        return (x, y);
    }
}
