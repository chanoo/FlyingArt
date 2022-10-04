using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
public class UI_Action : MonoBehaviour
{
    public Button bt_show, bt_event, bt_home, bt_tour, bt_create;

    private VideoPlayer vid;
    public VideoClip[] myclip;
    public GameObject[] screenList;



    //버튼 처음 활성화를 위한 //
    [Space(20)]
    [SerializeField]
    private Sprite pressed_sprite;
    [SerializeField]
    private Sprite normal_sprite;



    // Start is called before the first frame update
    void Start()
    {
        vid = this.gameObject.GetComponent<VideoPlayer>();
        vid.isLooping = true;
        vid.Play();


        PressHome();


        bt_show.onClick.AddListener(PressShow);
        bt_event.onClick.AddListener(PressEvent);
        bt_home.onClick.AddListener(PressHome);
        bt_tour.onClick.AddListener(PressTour);
    bt_create.onClick.AddListener(PressCreate);
    }

    // Update is called once per frame
    void PressShow()
    {
        bt_home.image.sprite = normal_sprite;
        vid.clip = myclip[1];

        print("show");
        for (int i = 0; i < screenList.Length; i++)
        {
            if (i == 0)
                screenList[i].SetActive(true);
            else
                screenList[i].SetActive(false);
        }
    }
    void PressEvent()
    {
        vid.clip = myclip[1];
        bt_home.image.sprite = normal_sprite;
        print("event");
        for (int i = 0; i < screenList.Length; i++)
        {
            if (i == 1)
                screenList[i].SetActive(true);
            else
                screenList[i].SetActive(false);
        }
    }
    void PressHome()
    {
       
        print("home");
        vid.clip = myclip[0];
        for (int i = 0; i < screenList.Length; i++)
        {
            if (i == 2)
                screenList[i].SetActive(true);
            else
                screenList[i].SetActive(false);
        }

        bt_home.image.sprite = pressed_sprite;

    }
    void PressTour()
    {
        vid.clip = myclip[1];
        bt_home.image.sprite = normal_sprite;
        print("tour");
        for (int i = 0; i < screenList.Length; i++)
        {
            if (i == 3)
                screenList[i].SetActive(true);
            else
                screenList[i].SetActive(false);
        }
    }
    void PressCreate()
    {
        vid.clip = myclip[1];
        bt_home.image.sprite = normal_sprite;
        for (int i = 0; i < screenList.Length; i++)
        {
            if (i == 4)
                screenList[i].SetActive(true);
            else
                screenList[i].SetActive(false);
        }
        print("create");

    }

    public void openScreen(int index)
    {
        for (int i = 0; i < screenList.Length; i++)
        {
            if (i == index)
                screenList[i].SetActive(true);
            else
                screenList[i].SetActive(false);
        }
    }
    public void GoURL(string _url)
    {
        Application.OpenURL(_url);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }
}
