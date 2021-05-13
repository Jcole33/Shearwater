using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.TestTools;

public class MyTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void DataStreamGetTest()
    {
        FlightDataStorage dataStream = new FlightDataStorage();
        Debug.Assert(dataStream.get("BLAH") == "DATA NOT FOUND");
    }
    [Test]
    public void MarkerManagerTest() {
        PositionARMarker marker = new PositionARMarker();
        Debug.Assert(marker.GetDistanceFromLatLonInKm(0, 0, 0, 0) == 0);
        Debug.Log(marker.Rotate(5, 0, 90));
        (double, double) markerTuple = marker.Rotate(5, 0, 90);
        Debug.Assert(Math.Round(markerTuple.Item1) == 0);
        Debug.Assert(Math.Round(markerTuple.Item2) == 5);
    }
    [Test]
    public void IntegrationTest() {
        PositionARMarker marker = new PositionARMarker();
        FlightDataStorage dataStream = new FlightDataStorage();
        dataStream.set("latitude", "5");
        dataStream.set("longitude", "5");
        Debug.Assert(marker.GetDistanceFromLatLonInKm(Convert.ToDouble(dataStream.get("latitude")), Convert.ToDouble(dataStream.get("longitude")), 5, 5) == 0);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator MyTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
