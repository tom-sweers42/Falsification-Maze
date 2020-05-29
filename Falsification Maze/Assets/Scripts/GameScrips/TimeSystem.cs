using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TimeSystem : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text levelText;
    public GameObject timeOver;
    private float start = 0;
    private float diff;
    public float maxTime = 5; //seconds
    private bool noTime = false;

    // Update is called once per frame
    void Update()
    {
        start += Time.deltaTime;
        diff = maxTime - start;
        if (diff <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
            timeOver.SetActive(true);
            levelText.text = "LEVEL " + CrossSceneInformationClass.level;
            noTime = true;
        }
        if (noTime & (Input.GetKeyDown(KeyCode.Space)))
        {
            noTime = false;
            SceneManager.LoadScene("Menu");
            CrossSceneInformationClass.level = 0;
        }
        string minutes = Mathf.Floor(diff / 60).ToString("00");
        string seconds = (diff%60).ToString("00");
        timeText.text = "TIME " + minutes + ":" + seconds;
    }

}
