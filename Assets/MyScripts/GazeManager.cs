using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Timers;
using TMPro;


public class GazeManager : MonoBehaviour {
    public Camera viewCamera;
    private PositionARMarker lastGazedUpon;
    private Text text;
    private static System.Timers.Timer aTimer;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGaze();
        if (OVRInput.GetDown(OVRInput.RawButton.B)) {
            text.text = "";
            lastGazedUpon = null;
        } else if (lastGazedUpon)
            text.text = lastGazedUpon.name + "\n" + (lastGazedUpon.distanceToMarker * 1000) + " (m)";
    }
    private void CheckGaze() {
        Ray gazeRay = new Ray(viewCamera.transform.position, viewCamera.transform.rotation * Vector3.forward);
        RaycastHit hit;
        if (Physics.Raycast(gazeRay, out hit, Mathf.Infinity)) {
            lastGazedUpon = hit.transform.gameObject.GetComponent<PositionARMarker>();
        }
    }
}
