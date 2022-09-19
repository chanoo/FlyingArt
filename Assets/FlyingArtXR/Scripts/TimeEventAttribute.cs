using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[RequireComponent(typeof(timeEventController))]
public class TimeEventAttribute : MonoBehaviour
{

    [Header("시작타임설정")]
    public int hours;
    public int minutes;
    public int seconds;
    [Header("오브젝트 지정")]
    public GameObject eventPrefab;
    [Header("종료시간설정")]
    public int DueMinute;
    public int DueSecond;

    //[HideInInspector]
    public int secondForEvent;

    //[HideInInspector]
    public int secondForEndEvent;

  

    private void Start()
    {
        timeEventController tc = GetComponentInParent<timeEventController>();
        SpecifyTime sT = GetComponentInParent<SpecifyTime>();
        int TcTime = GetComponentInParent<timeEventController>().totalCurrentSeconds;
        //print($"tc 타임{tc.totalCurrentSeconds}");
        if (tc.standard ==StandardTimeEvent.hybrid)
        {
            print("hybrid select");

            int data_hours = PlayerPrefs.GetInt("savedHours");
           int data_minutes = PlayerPrefs.GetInt("savedMinutes");
           int data_seconds = PlayerPrefs.GetInt("savedSeconds");
            int data_totalSecond = data_seconds + (data_minutes * 60) + (data_hours * 60 * 60);
            //print($" data_hours = { data_hours}");
            //print($" data_minutes = { data_minutes}");
            //print($" data_seconds = { data_seconds}");
            //print($" data_totalSecond = { data_totalSecond}");

            #region before
            //secondForEvent =  seconds + (minutes * 60) + (hours * 60 * 60)+TcTime+ (int)sT.totalGameSeconds;
            //secondForEndEvent = secondForEvent + (DueMinute * 60) + DueSecond;
            #endregion

            secondForEvent = (seconds + (minutes * 60) + (hours * 60 * 60))+ data_totalSecond;
            secondForEndEvent = secondForEvent + (DueMinute * 60) + DueSecond;
            //print($" secondForEndEvent = { secondForEvent}");
            //print($" secondForEndEvent = { secondForEndEvent}");


        }
        //else if (tc.standard == StandardTimeEvent.specified)
        //{
        //    print("specified select");
        //    secondForEvent = seconds + (minutes * 60) + (hours * 60 * 60) + TcTime + (int)sT.totalGameSeconds;
        //    secondForEndEvent = secondForEvent + (DueMinute * 60) + DueSecond;
        //}
        else
        {
            print("systemp");
            secondForEvent += seconds + (minutes * 60) + (hours * 60 * 60);
            secondForEndEvent = secondForEvent + (DueMinute * 60) + DueSecond;
        }

        
    }

}
