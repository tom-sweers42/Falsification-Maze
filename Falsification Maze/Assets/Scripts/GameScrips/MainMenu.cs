using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text idField;

    public void PlayGame ()
    {
        if (idField.text != "")
        {
            MazeData mazeData = GameObject.Find("Data").GetComponent<MazeData>();
            mazeData.playerId = idField.text;
            CrossSceneInformationClass.level += 1;
            Cursor.visible = false;
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + CrossSceneInformationClass.level);
        }

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}


public static class CrossSceneInformationClass {
    public static int level { get; set; } = 0;
}