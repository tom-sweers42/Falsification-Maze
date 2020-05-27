using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TimeSystem : MonoBehaviour
{
    public TMP_Text timeText;
    private float start = 0;
    private float diff;
    private float maxTime = 300; //seconds
    private bool timeStarted = false;

    // Update is called once per frame
    void Update()
    {
        start += Time.deltaTime;
        diff = maxTime - start;
        if (diff <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("Menu");
        }
        string minutes = Mathf.Floor(diff / 60).ToString("00");
        string seconds = (diff%60).ToString("00");
        timeText.text = "TIME " + minutes + ":" + seconds;
    }

}
