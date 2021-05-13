using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

public class getRicohStream : MonoBehaviour
{

    static WebCamTexture ricohStream;
    string camName = "RICOH THETA V FullHD"; // Name of your camera. 
    public Material camMaterial;  // Skybox material
    MjpegProcessor mjpeg;

    void Start()
    {
        /*
        WebCamDevice[] devices = WebCamTexture.devices;
        //printing out all video sources for debugging purposes
		Debug.Log("Number of web cams connected: " + devices.Length);
        for (int i = 0; i < devices.Length; i++)
		{
			Debug.Log(i + " " + devices[i].name);
		}

        if (ricohStream == null)
            ricohStream = new WebCamTexture(camName, 3840, 1920); // Resolution you want

        if (!ricohStream.isPlaying)
            ricohStream.Play();

	    if (camMaterial != null)
            camMaterial.mainTexture = ricohStream;
        */
        //StartStreamThread();
    }/*
    IEnumerator AcquireStream()
    {
        url = "http://10.0.0.7/osc/commands/execute";
        UnityWebRequest www = UnityWebRequest.Post(url);
        www.downloadHandler = new DownloadHandlerBuffer();
        var body = "{'name': 'camera.getLivePreview'}";
        www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonUtility.ToJson(body)));
        var authorization = "Digest username='THETAYL00230674', realm='RICOH THETA V', nonce='" + nonce + "', uri='/osc/commands/execute', cnonce='" + cnonce + "', nc=00000001, qop=auth, response='" + response + "', algorithm='MD5'";
        www.SetRequestHeader("AUTHORIZATION", authorization);
        yield return www.SendWebRequest();
        camMaterial.mainTexture = www.downloadHandler.data;
    }*/
    void StartStreamThread() {
        //StartCoroutine(AcquireStream());
    }

}
//Authorization: Digest username="THETAYL00230674", realm="RICOH THETA V", nonce="y2CT7YWjSHWZIRKBsMhy8A==", uri="/osc/info", cnonce="MGI2MDMzZDYyZGRiMWNjZDVlZTk5YzI1OGVkMDExOWY=", nc=00000001, qop=auth, response="f2a370e79c80203912d74f1f33b13451", algorithm="MD5"