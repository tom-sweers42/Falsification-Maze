using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollider : MonoBehaviour
{
    private GameObject curFloor;
    public MazeLoader gameManager;
    public Material materialPath;
    public Material materialRight;
    public Material materialLeft;

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    void OnTriggerEnter(Collider other) {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.name == "END")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("Menu");
        }

        if (other.gameObject.name.StartsWith("Floor") && curFloor != other.gameObject) {
            curFloor = other.gameObject;
            string pair = curFloor.name.Split(' ')[1];
            int r = Int32.Parse(pair.Split(',')[0]);
            int c = Int32.Parse(pair.Split(',')[1]);
            gameManager.ClearPath();
            int currPathLength = gameManager.mazeCells[r, c].drawRoute(materialPath, 0);
            Debug.Log("Current Path Length: " + currPathLength.ToString());
            gameManager.tilesCounterField.text = currPathLength.ToString();

        }
    }
}
