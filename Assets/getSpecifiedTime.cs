using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class getSpecifiedTime : MonoBehaviour
{
    public TMPro.TextMeshProUGUI _hours,_minutes,_seconds;
    public TMPro.TextMeshProUGUI s_hours,s_minutes,s_seconds;
    //public Time_Ininitalize TI;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        _hours.text = s_hours.text;
        _minutes.text = s_minutes.text;
        _seconds.text = s_seconds.text;
    }
}
