using System;
using UnityEngine;



public class SpecifyTime : MonoBehaviour
{

    private GUIStyle guiStyle = new GUIStyle();

    public double totalGameSeconds;
    [Space(10)]
    public string time;

    //[HideInInspector]
    public double _hours;
    public double _minutes;
    public double _seconds;
    public double _days;


    private double secondsPerSecond = 1;
    [Space(10)]
    public float speed = 1;


    private void Awake()
    {
        totalGameSeconds += _seconds + (_minutes * 60) + (_hours * 60 * 60);
        //Debug.Log("현재게임 초"+totalGameSeconds);
    }
    void Update()
    {

        Time.timeScale = speed;

        totalGameSeconds += (secondsPerSecond * Time.deltaTime);//*speed

        int currentSeconds = (int)totalGameSeconds;




        _seconds = currentSeconds % 60;
        _minutes = currentSeconds / 60 % 60;
        _hours = (currentSeconds / 60) / 60 % 24;
        _days = currentSeconds / (60 * 60 * 24) % 30;
        TimeSpan ts = TimeSpan.Parse($"{(int)_hours}:{(int)_minutes}:{(int)_seconds}");
        time = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)_hours, (int)_minutes, (int)_seconds);


    }

}
