using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClickMain : MonoBehaviour
{
    public Button mainButton;

    void Start()
    {
        Button btn = mainButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        Debug.Log("You have clicked the button!");
        SceneManager.LoadScene("Main");
    }

}


