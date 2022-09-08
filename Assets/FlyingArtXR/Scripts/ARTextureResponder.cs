using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(RawImage))]
//[RequireComponent(typeof(ARCameraBackground))]


public class ARTextureResponder : MonoBehaviour
{
    public ARCameraBackground ab;
    void Start()
    {
        var renderer = this.GetComponent<Renderer>();
        var rawImage = GetComponent<RawImage>();
        var camBack = ab.material;
        rawImage.material = ab.material;
        renderer.material = ab.material;
    }
}