using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHide : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.A)) {
            GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
        }
    }
}
