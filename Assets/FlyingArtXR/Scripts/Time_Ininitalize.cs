using System.Collections;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class Time_Ininitalize : MonoBehaviour
{
    public TMP_InputField input_hours, input_minutes, input_seconds;

    public TMP_Dropdown input_timeStandard;

    public GameObject Warning;

    public TMPro.TextMeshProUGUI current_hours, current_minutes, current_seconds;


    [HideInInspector]
    public int data_hours;
    [HideInInspector]
    public int data_minutes;
    [HideInInspector]
    public int data_seconds;
    [HideInInspector]
    public int data_timeStandard;


    // Start is called before the first frame update

    private void Awake()
    {
        if (Warning != null)
            Warning.gameObject.SetActive(false);

        data_hours = PlayerPrefs.GetInt("savedHours");
        data_minutes = PlayerPrefs.GetInt("savedMinutes");
        data_seconds = PlayerPrefs.GetInt("savedSeconds");
        data_timeStandard = PlayerPrefs.GetInt("timeStandard");

    }


    public void Iniialize_eventStartTime()
    {
        if (int.TryParse(input_hours.text, out int result))
        {
            if (result > 24)
                StartCoroutine(showWarning());

            data_hours = result;
        }
        else
            StartCoroutine(showWarning());

        if (int.TryParse(input_minutes.text, out int result2))
        {
            if (result2 > 60)
                StartCoroutine(showWarning());

            data_minutes = result2;
        }
        else
            StartCoroutine(showWarning());


        if (int.TryParse(input_seconds.text, out int result3))
        {
            if (result3 > 60)
                StartCoroutine(showWarning());

            data_seconds = result3;
        }
        else
            StartCoroutine(showWarning());


        data_timeStandard = input_timeStandard.value;

        PlayerPrefs.SetInt("savedHours", data_hours);
        PlayerPrefs.SetInt("savedMinutes", data_minutes);
        PlayerPrefs.SetInt("savedSeconds", data_seconds);

        PlayerPrefs.SetInt("timeStandard", data_timeStandard);


    }

    IEnumerator showWarning()
    {
        Warning.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        Warning.gameObject.SetActive(false);

    }

    private void FixedUpdate()
    {
        current_hours.text = DateTime.Now.Hour.ToString();
        current_minutes.text = DateTime.Now.Minute.ToString();
        current_seconds.text = DateTime.Now.Second.ToString();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }
    public void LoadFirstScene()
    {
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }


    public void toggleObject(string name)
    {
        GameObject toggleObject1 = GameObject.Find(name);
        toggleObject1.transform.GetChild(0).gameObject.SetActive(!toggleObject1.transform.GetChild(0).gameObject.activeSelf);

    }
    public void speedUp()
    {
        SpecifyTime speedObject1 = GameObject.FindObjectOfType<SpecifyTime>();
        speedObject1.speed += 1;
    }
    public void speedDown()
    {
        SpecifyTime speedObject1 = GameObject.FindObjectOfType<SpecifyTime>();
        if (speedObject1.speed > 1)
            speedObject1.speed -= 1;
    }

    public void timeAdd()
    {
        timeEventController tc = GameObject.FindObjectOfType<timeEventController>();
        foreach (var item in tc.eventList)
        {
            item.transform.GetChild(0).gameObject.SetActive(false);
        }
        SpecifyTime speedObject1 = GameObject.FindObjectOfType<SpecifyTime>();

        speedObject1.totalGameSeconds += 60;
    }

    public void toggleDarkLight()
    {


        GameObject day = GameObject.Find("-Env Settings--dayLight");
        day.transform.GetChild(0).gameObject.SetActive(!day.transform.GetChild(0).gameObject.activeSelf);

    }

}
