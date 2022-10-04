using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddFuntion : MonoBehaviour
{
    public string objectName;
    public GameObject targetObject;
    [Space(20)]
    public GameObject StandardObject;

    //임의변수//
    private float nextTime = 0;
    private float interval = 1.0f;

    private void Start()
    {

    }
    private void Update()
    {
        if (Time.time >= nextTime)
        {

            nextTime += interval;
            StandardObject = GameObject.Find(objectName);
            if (StandardObject != null)
                targetObject.SetActive(StandardObject.transform.GetChild(0).gameObject.activeSelf);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }


}
