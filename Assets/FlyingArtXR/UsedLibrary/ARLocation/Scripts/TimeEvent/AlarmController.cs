using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class AlarmController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TMP_InputField _hoursIput, _minuteInput, _secondInput;
    [SerializeField] private TMP_Dropdown _Dropdown;

    //[SerializeField] private string eventTime;

    private bool isAlarmSet = false;
    private DateTime _alarmTime = DateTime.Today;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int hours = DateTime.Now.Hour; 
        int minutes = DateTime.Now.Minute;
        int seconds = DateTime.Now.Second;

        bool isAM = hours < 12;
        _timeText.text = $"{hours % 12 :D2}:{minutes:D2}:{seconds:D2}{(isAM ? "AM" : "PM")}";


        if(isAlarmSet && DateTime.Now > _alarmTime)
        {
            Debug.Log("ALARM");
        }

        Debug.Log(DateTime.Now);

        Debug.Log($"AlrmTime = {_alarmTime}");
        Debug.Log($"today = {DateTime.Today}");
    }

    public void SetAlam()
    {
        int hours;
        if (_Dropdown.value == 0)
        {

            hours = int.Parse(_hoursIput.text);
        }
        else
        {
            hours = int.Parse(_hoursIput.text) + 12;

        }

        TimeSpan ts = TimeSpan.Parse($"{hours}:{_minuteInput.text}:{_secondInput.text}");
        _alarmTime += ts;

        isAlarmSet = true; ;


    }

}
