using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
public class MazeLoader : MonoBehaviour {
	public int mazeRows, mazeColumns;
    public int randomDeletesDivider;
	public GameObject wall;
	public float size = 2f;
    public float timeLimit;
    private float timeLeft;
    private float initialIntensity;
	public Material materialFinish;
	public Material materialPath;

    public Material materialNormal;
    public GameObject player;
    public GameObject light;

	public MazeCell[,] mazeCells;
    public GameObject tilesCounterFieldObject;
    public Text tilesCounterField;
	// Use this for initialization
	void Start () {
        timeLeft = timeLimit;
        Light lightComponent = light.GetComponent<Light>();
        initialIntensity = lightComponent.intensity;
		InitializeMaze ();
        tilesCounterField = tilesCounterFieldObject.GetComponent<Text>();
		MazeAlgorithm ma = new HuntAndKillMazeAlgorithm (mazeCells);
		ma.CreateMaze ();
        deleteRandomWalls();
        ma.addShortestPaths(materialPath);
	}

	// Update is called once per frame
	void Update () {
        timeLeft -= Time.deltaTime;
        Light lightComponent = light.GetComponent<Light>();
        lightComponent.intensity = initialIntensity * (timeLeft/timeLimit);
	}

    public void deleteRandomWalls() {
        int randomDeletes = Convert.ToInt32((mazeColumns*mazeRows)/randomDeletesDivider);
        for (int i = 0; i < randomDeletes; i++) {
            int r = UnityEngine.Random.Range(0, mazeRows-1);
            int c = UnityEngine.Random.Range(0, mazeColumns-1);
            int direction = UnityEngine.Random.Range(0,2);

            if (direction == 0) {
                if (!DestroyWallIfItExists(mazeCells[r,c].southWall)) {
                    i--;
                }
            }
            else {
                if (!DestroyWallIfItExists(mazeCells[r,c].eastWall)) {
                    i--;
                }
            }
        }
    }
	private bool DestroyWallIfItExists(GameObject wall) {
		if (wall != null) {
			GameObject.Destroy (wall);
            wall.SetActive(false);
            return true;
		}
        return false;
	}
    public void clearPath() {
		for (int r = 0; r < mazeRows; r++) {
			for (int c = 0; c < mazeColumns; c++) {
				if(c != mazeColumns -1 || r != mazeRows - 1) {
					MeshRenderer meshRenderer = mazeCells[r, c].floor.GetComponent<MeshRenderer>();
					meshRenderer.material = materialNormal;
                }
            }
        }
    }
	private void InitializeMaze() {

		mazeCells = new MazeCell[mazeRows,mazeColumns];
		for (int r = 0; r < mazeRows; r++) {
			for (int c = 0; c < mazeColumns; c++) {
                mazeCells [r, c] = new MazeCell (c,r);

				// For now, use the same wall object for the floor!
				mazeCells [r, c] .floor = Instantiate (wall, new Vector3 (r*size, -(size/2f), c*size), Quaternion.identity) as GameObject;
				mazeCells [r, c] .floor.name = "Floor " + r + "," + c;
				mazeCells [r, c] .floor.transform.Rotate (Vector3.right, 90f);

				mazeCells [r, c] .roof = Instantiate (wall, new Vector3 (r*size, (size/2f), c*size), Quaternion.identity) as GameObject;
				mazeCells [r, c] .roof.name = "Roof " + r + "," + c;
				mazeCells [r, c] .roof.transform.Rotate (Vector3.right, 90f);


				if (c == 0) {
					mazeCells[r,c].westWall = Instantiate (wall, new Vector3 (r*size, 0, (c*size) - (size/2f)), Quaternion.identity) as GameObject;
					mazeCells [r, c].westWall.name = "West Wall " + r + "," + c;
				}

				mazeCells [r, c].eastWall = Instantiate (wall, new Vector3 (r*size, 0, (c*size) + (size/2f)), Quaternion.identity) as GameObject;
				mazeCells [r, c].eastWall.name = "East Wall " + r + "," + c;

				if (r == 0) {
					mazeCells [r, c].northWall = Instantiate (wall, new Vector3 ((r*size) - (size/2f), 0, c*size), Quaternion.identity) as GameObject;
					mazeCells [r, c].northWall.name = "North Wall " + r + "," + c;
					mazeCells [r, c].northWall.transform.Rotate (Vector3.up * 90f);
				}

				mazeCells[r,c].southWall = Instantiate (wall, new Vector3 ((r*size) + (size/2f), 0, c*size), Quaternion.identity) as GameObject;
				mazeCells [r, c].southWall.name = "South Wall " + r + "," + c;
				mazeCells [r, c].southWall.transform.Rotate (Vector3.up * 90f);

				if(c == mazeColumns -1 && r == mazeRows - 1)
				{

						//materialFinish
					MeshRenderer meshRenderer = mazeCells[r, c].southWall.GetComponent<MeshRenderer>();
					meshRenderer.material = materialFinish;


					MeshRenderer meshRenderer2 = mazeCells[r, c].eastWall.GetComponent<MeshRenderer>();
					meshRenderer2.material = materialFinish;
				}
			}
		}


	}
}


