using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;

namespace TrueClouds
{
    public class FPSDisplay : MonoBehaviour
    {
        private string _text;
        //private Stopwatch _stopwatch;
        private float _delta;

        private GUIStyle textStyle = new GUIStyle();

        private void OnEnable()
        {
            textStyle.normal.textColor = Color.yellow;
        }
        private void Update()
        {
            _delta = Mathf.Lerp(_delta, Time.unscaledDeltaTime, 1.0f);
            float fps = 1.0f / _delta;
            _text = string.Format("{0:0.0} ms ({1:0.} fps)", _delta * 1000, fps);
        }

        private void OnGUI()
        {
            textStyle.fontSize = 40;
            GUILayout.BeginArea(new Rect(100, 20, 300, 40));
            GUILayout.Label(_text,textStyle);
            GUILayout.EndArea();
        }
    }
}