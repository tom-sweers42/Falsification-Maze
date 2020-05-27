using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame ()
    {
        CrossSceneInformationClass.level += 1;
        Cursor.visible = false;
        if (CrossSceneInformationClass.level < 5) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + CrossSceneInformationClass.level);
        }
        else {
            QuitGame();
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