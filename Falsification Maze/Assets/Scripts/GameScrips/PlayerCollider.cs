using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;

public class PlayerCollider : MonoBehaviour
{
    private GameObject curFloor;
    private int pathFinishLength = 0;
    public MazeLoader gameManager;
    public Material materialPath;
    public Material materialRight;
    public Material materialLeft;
    private bool folllowedGreenPath = false;

    private String[] fmTexts = new String [] {
        "Did you notice the green dot pointing in the other direction?",
        "Did you notice that you followed the blue dot?",
        "Did you consider the dots on the roof for your decision?",
        "Perhaps you should not only use the green path!"
    };

    private String [] fmTextsType2 = new String [] {
        "Did you see the middle dots turning more red?",
        "Have you looked at the middle dots on the roof?"
    };
    private MazeCell curCheckCell;
    private float time = 0;
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
    void Update() {

        // start += Time.deltaTime;
        // diff = maxTime - start;

        Transform transform = GetComponent<Transform>();
        (int r, int c) = getCoordinates(transform);
        // Debug.Log(gameManager.mazeCells[r, c].hasMoreThanOneOpening(gameManager.mazeCells));
        if (r == gameManager.mazeRows - 1 && c == gameManager.mazeColumns-1) {
            gameManager.gameWon();
        }
        if (!gameManager.correctPath) {
            if (!folllowedGreenPath && gameManager.checkCell != null && r == gameManager.checkCell.r && c == gameManager.checkCell.c) {
                if (gameManager.fm.text == "") {
                    System.Random random = new System.Random();
                    gameManager.fm.text = fmTexts[random.Next(fmTexts.Length)];
                    folllowedGreenPath = true;
                    Debug.Log("You Followed the greenpath");
                    time = 0;
                }
                curCheckCell = gameManager.checkCell;
                bool done = false;
                while (!done) {
                    Debug.Log("curCheckcel: " + curCheckCell.r + ", " + curCheckCell.c);
                    if (curCheckCell.hasMoreThanOneOpeningNullVersion(gameManager.mazeCells)) {
                        done = true;
                    }
                    else {
                        curCheckCell = curCheckCell.next;
                        if (curCheckCell == null) {
                            done = true;
                        }
                    }

                }




            }

            if (folllowedGreenPath && curCheckCell != null && r == curCheckCell.r && c == curCheckCell.c) {
                if (gameManager.fm.text == "") {
                    Debug.Log("curCheckcel: " + curCheckCell.r + ", " + curCheckCell.c);
                    System.Random random = new System.Random();
                    gameManager.fm.text = fmTextsType2[random.Next(fmTextsType2.Length)];
                    curCheckCell = null;
                    Debug.Log("You Followed the greenpath");
                    time = 0;
                }
            }

            if (pathFinishLength != 0 && gameManager.copyMazeCells[r,c].drawRoute(materialPath,0) > pathFinishLength ) {
                Debug.Log("Wrong Path!!");
            }
            if (folllowedGreenPath) {
                time += Time.deltaTime;
                if (time >= 5f) {
                    gameManager.fm.text = "";
                }
            }
            // Debug.Log(pathFinishLength);
            pathFinishLength = gameManager.copyMazeCells[r,c].drawRoute(materialPath,0);

        }
        gameManager.ClearPath();
        int currPathLength = gameManager.mazeCells[r, c].drawRoute(materialPath, 0);
        gameManager.tilesCounterField.text = currPathLength.ToString();
    }

    (int, int) getCoordinates(Transform transform) {
        int c = (int) ((transform.position.z + gameManager.size/2)/gameManager.size);
        int r = (int) ((transform.position.x + gameManager.size/2)/gameManager.size);
        return (r,c);
    }
    // void OnTriggerEnter(Collider other) {

    //     if (other.gameObject.name.StartsWith("Floor") && curFloor != other.gameObject) {
    //         curFloor = other.gameObject;
    //         string pair = curFloor.name.Split(' ')[1];
    //         int r = Int32.Parse(pair.Split(',')[0]);
    //         int c = Int32.Parse(pair.Split(',')[1]);
    //         // gameManager.ClearPath();
    //         // // Debug.Log(gameManager.mazeCells[r, c].hasMoreThanOneOpening(gameManager.mazeCells));
    //         // int currPathLength = gameManager.mazeCells[r, c].drawRoute(materialPath, 0);
    //         // gameManager.tilesCounterField.text = currPathLength.ToString();

    //     }

    // }
}
