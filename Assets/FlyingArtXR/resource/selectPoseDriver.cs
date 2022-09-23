using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SpatialTracking;

public class selectPoseDriver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        this.gameObject.AddComponent<ARPoseDriver>();
#elif PLATFORM_ANDROID
        this.gameObject.AddComponent<TrackedPoseDriver>();
        GetComponent<TrackedPoseDriver>().SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRDevice,TrackedPoseDriver.TrackedPose.ColorCamera);
#endif

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
