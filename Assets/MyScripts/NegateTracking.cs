using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class NegateTracking : MonoBehaviour
{
    UnityEngine.XR.InputDevice headset;
    void Start() {
        var headsets = new List<UnityEngine.XR.InputDevice>();
        var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeadMounted | UnityEngine.XR.InputDeviceCharacteristics.TrackedDevice;
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, headsets);
        foreach (var device in headsets) {
            if (device.name == "Oculus Rift CV1")
                headset = device;
        }
    }
    void Update()
    {
        Vector3 position;
        headset.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out position);
        transform.position = -position;
        //transform.rotation = Quaternion.Inverse(InputTracking.GetLocalRotation(VRNode.CenterEye));
    }
}