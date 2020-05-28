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
    void Update() {
        Transform transform = GetComponent<Transform>();
        (int r, int c) = getCoordinates(transform);
        gameManager.ClearPath();
        // Debug.Log(gameManager.mazeCells[r, c].hasMoreThanOneOpening(gameManager.mazeCells));
        int currPathLength = gameManager.mazeCells[r, c].drawRoute(materialPath, 0);
        gameManager.tilesCounterField.text = currPathLength.ToString();
        if (gameManager.checkCell != null && r == gameManager.checkCell.r && c == gameManager.checkCell.c) {
            Debug.Log("Wrong Path!!!");
        }
    }

    (int, int) getCoordinates(Transform transform) {
        int c = (int) ((transform.position.z + gameManager.size/2)/gameManager.size);
        int r = (int) ((transform.position.x + gameManager.size/2)/gameManager.size);
        return (r,c);
    }
    void OnTriggerEnter(Collider other) {
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
            // gameManager.ClearPath();
            // // Debug.Log(gameManager.mazeCells[r, c].hasMoreThanOneOpening(gameManager.mazeCells));
            // int currPathLength = gameManager.mazeCells[r, c].drawRoute(materialPath, 0);
            // gameManager.tilesCounterField.text = currPathLength.ToString();

        }

    }
}
