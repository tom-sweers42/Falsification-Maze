using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

using TMPro;

public class PlayerCollider : MonoBehaviour
{
    // GameManager
    public MazeLoader gameManager;
    // Path
    private int pathFinishLength = 0;

    // FM
    private bool folllowedGreenPath = false;
    private bool wrongTurn = false;
    private int wrongCounter = 0;
    private MazeCell curCheckCell;
    private float time = 0;

    // FM Texts
    private String[] fmTexts = new String [] {
        "Did you notice the markings on the ceiling?",
        "Have you noticed the green dot on the ceiling?",
        "Did you consider the dots on the ceiling for your decision?",
        "Have you seen the markings?"
    };

    private List<String> fmTextsType1 = new List<String>(){
        "Did you notice the markings on the ceiling?",
        "Have you noticed the green dot on the ceiling?",
        "Did you consider the dots on the ceiling for your decision?",
        "Have you seen the markings?"
    };

    private List<String> fmTextsType2 = new List<String>(){
        "Did you see the middle dots turning more red?",
        "Have you looked at the middle dots on the ceiling?"
    };

    private List<String> fmTextsType3 = new List<String>(){
        "Did you look at the floor?",
        "Did you look at the green tiles?"
    };

    // Finishing
    private bool won = false;

    // Data
    public int tilesCounter;
    public int totalWrongCounter;
    public int totalCorrectCounter;
    public int fmCounter;
    private int prevR, prevC = 0;

    public List<float> fmTimeStamps = new List<float>();

    void Start() {
        transform.position = new Vector3(0, -gameManager.heigth/2f + 1f, 0);
    }
    void Update() {

        Transform transform = GetComponent<Transform>();
        (int r, int c) = getCoordinates(transform);
        if (r != prevR || c != prevC) {
            tilesCounter += 1;
            prevC = c;
            prevR = r;
        }
        if (!gameManager.correctPath) {
            if (!folllowedGreenPath && gameManager.checkCell != null && r == gameManager.checkCell.r && c == gameManager.checkCell.c) {
                if (gameManager.fm != null && gameManager.fm.text == "") {
                    System.Random random = new System.Random();
                    gameManager.fm.text = fmTexts[random.Next(fmTextsType1.Count)];
                    folllowedGreenPath = true;
                    Debug.Log("You Followed the greenpath");
                    time = 0;
                    fmCounter += 1;
                    fmTimeStamps.Add(gameManager.start);
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
                if (gameManager.fm != null && gameManager.fm.text == "") {
                    Debug.Log("curCheckcel: " + curCheckCell.r + ", " + curCheckCell.c);
                    System.Random random = new System.Random();
                    gameManager.fm.text = fmTextsType2[random.Next(fmTextsType2.Count)];
                    curCheckCell = null;
                    Debug.Log("You Followed the greenpath");
                    time = 0;
                    fmCounter += 1;
                    fmTimeStamps.Add(gameManager.start);
                }
            }

            if (folllowedGreenPath) {
                time += Time.deltaTime;
                if (time >= 5f) {
                    gameManager.fm.text = "";
                }
            }
        }

        if (pathFinishLength != 0 && gameManager.copyMazeCells[r,c].drawRoute(gameManager.materialPath,0) > pathFinishLength ) {
            Debug.Log("Wrong Path!!");
            wrongCounter += 1;
            totalWrongCounter += 1;
            if (gameManager.fm != null && gameManager.fm.text == "" && wrongCounter >= 5) {
                System.Random random = new System.Random();
                List<String> combinedList = fmTextsType1.Concat(fmTextsType2).ToList().Concat(fmTextsType3).ToList();
                gameManager.fm.text = combinedList[random.Next(combinedList.Count)];
                wrongTurn = true;
                time = 0;
                fmCounter += 1;
                fmTimeStamps.Add(gameManager.start);
                wrongCounter = 2;
            }
        }
        if (gameManager.copyMazeCells[r,c].drawRoute(gameManager.materialPath,0) < pathFinishLength ) {
            Debug.Log("Correct Path!");
            totalCorrectCounter += 1;
            wrongCounter = 0;
        }
        if (wrongTurn) {
            time += Time.deltaTime;
            if (time >= 5f) {
                gameManager.fm.text = "";
                wrongTurn = false;
            }
        }
        pathFinishLength = gameManager.copyMazeCells[r,c].drawRoute(gameManager.materialPath,0);
        gameManager.ClearPath();
        int currPathLength = gameManager.mazeCells[r, c].drawRoute(gameManager.materialPath, 0);
        if (r == gameManager.mazeRows - 1 && c == gameManager.mazeColumns-1 && !won) {
            gameManager.gameWon();
            won = true;
        }
    }

    (int, int) getCoordinates(Transform transform) {
        int c = (int) ((transform.position.z + gameManager.size/2)/gameManager.size);
        int r = (int) ((transform.position.x + gameManager.size/2)/gameManager.size);
        return (r,c);
    }
}
