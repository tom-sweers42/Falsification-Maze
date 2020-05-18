using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class MazeLoader : MonoBehaviour {
	public int mazeRows, mazeColumns;
    public int randomDeletesDivider;
	public int randomAdditionsDivider;
	public GameObject wall;
	public float size = 2f;
    public float timeLimit;
    private float timeLeft;
    private float initialIntensity;
	public Material materialFinish;
	public Material materialPath;

	public Material materialMarkerFar;
	public Material materialMarkerNear;
	public Material materialMarkerCorrect;
    public int threshold = 5;

    public Material[] symbols = new Material[4];
	public String[] symbolNames = new String[4];

    public Material materialNormal;
    public GameObject player;
    public new GameObject light;

	public MazeCell[,] mazeCells;
    public GameObject tilesCounterFieldObject;
    public Text tilesCounterField;

	public int initPathLength;
    private int cellPathLength;

    // Use this for initialization
    void Start () {
        timeLeft = timeLimit;
        Light lightComponent = light.GetComponent<Light>();
        initialIntensity = lightComponent.intensity;
		InitializeMaze ();
        tilesCounterField = tilesCounterFieldObject.GetComponent<Text>();
		MazeAlgorithm ma = new HuntAndKillMazeAlgorithm (mazeCells);
		ma.CreateMaze ();
		DeleteRandomWalls();
        AddRandomSymbols();
        initPathLength = ma.addShortestPaths(materialPath, 0, 0);
        AddPathBasedColor(ma);
	}

	// Update is called once per frame
	void Update () {
        timeLeft -= Time.deltaTime;
        Light lightComponent = light.GetComponent<Light>();
        lightComponent.intensity = initialIntensity * (timeLeft/timeLimit);
	}

	public void AddRandomSymbols() 
	{
		int randomAdditions = Convert.ToInt32((mazeColumns*mazeRows)/randomAdditionsDivider);
		for (int i =0; i < randomAdditions; i++) {
            int r = UnityEngine.Random.Range(0, mazeRows-1);
            int c = UnityEngine.Random.Range(0, mazeColumns-1);
			int direction = UnityEngine.Random.Range(0,2);
			int symbol = UnityEngine.Random.Range(0,4);

			if (direction == 0)
			{
				MeshRenderer meshRenderer = mazeCells[r, c].southWall.GetComponent<MeshRenderer>();
				meshRenderer.material = symbols[symbol];
				//mazeCells[r, c].floor.name = symbolNames[symbol];
			}		
			else
			{
				MeshRenderer meshRenderer = mazeCells[r, c].eastWall.GetComponent<MeshRenderer>();
				meshRenderer.material = symbols[symbol];
				//mazeCells[r, c].floor.name = symbolNames[symbol];				
			}
		}
	}

    void AddPathBasedColor(MazeAlgorithm ma)
    {
        int randomAdditions = Convert.ToInt32((mazeColumns * mazeRows) / randomAdditionsDivider);
        for (int i = 0; i < randomAdditions; i++)
        {
            int r = UnityEngine.Random.Range(0, mazeRows - 1);
            int c = UnityEngine.Random.Range(0, mazeColumns - 1);
            cellPathLength = ma.addShortestPaths(materialPath, r, c);
            ColorObject(r, c, initPathLength, cellPathLength);
        }
    }

    void ColorObject(int r, int c, int initPathLength, int cellPathLength)
    {
        int direction = UnityEngine.Random.Range(0, 3);
        if (direction == 0)
        {
            MeshRenderer meshRenderer = mazeCells[r, c].southWall.GetComponent<MeshRenderer>();

            if (initPathLength < cellPathLength && cellPathLength <= initPathLength + threshold)
            {
                meshRenderer.material = materialMarkerNear;
            }
            else if (initPathLength + threshold < cellPathLength)
            {
                meshRenderer.material = materialMarkerFar;

            }
            else if (cellPathLength <= initPathLength)
            {
                meshRenderer.material = materialMarkerCorrect;
            }
            else { }
        }

        else if (direction == 1)
        {
            MeshRenderer meshRenderer = mazeCells[r, c].eastWall.GetComponent<MeshRenderer>();

            if (initPathLength < cellPathLength && cellPathLength <= initPathLength + threshold)
            {
                meshRenderer.material = materialMarkerNear;
            }
            else if (initPathLength + threshold < cellPathLength)
            {
                meshRenderer.material = materialMarkerFar;

            }
            else if (cellPathLength <= initPathLength)
            {
                meshRenderer.material = materialMarkerCorrect;
            }
            else { }

        }

        else
        {
            MeshRenderer meshRenderer = mazeCells[r, c].roof.GetComponent<MeshRenderer>();

            if (initPathLength < cellPathLength && cellPathLength <= initPathLength + threshold)
            {
                meshRenderer.material = materialMarkerNear;
            }
            else if (initPathLength + threshold < cellPathLength)
            {
                meshRenderer.material = materialMarkerFar;

            }
            else if (cellPathLength <= initPathLength)
            {
                meshRenderer.material = materialMarkerCorrect;
            }
            else { }

        }
    }

    public void DeleteRandomWalls() {
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

    public void ClearPath() {
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

				// MeshRenderer meshRendererRight = mazeCells[r, c].northWall.GetComponent<MeshRenderer>();
				// meshRendererRight.material = materialRight;

				mazeCells[r,c].southWall = Instantiate (wall, new Vector3 ((r*size) + (size/2f), 0, c*size), Quaternion.identity) as GameObject;
				mazeCells [r, c].southWall.name = "South Wall " + r + "," + c;
				mazeCells [r, c].southWall.transform.Rotate (Vector3.up * 90f);

				if(c == mazeColumns -1 && r == mazeRows - 1)
				{
					mazeCells[r, c].floor.name = "END";

					MeshRenderer meshRenderer = mazeCells[r, c].southWall.GetComponent<MeshRenderer>();
					meshRenderer.material = materialFinish;

					MeshRenderer meshRenderer2 = mazeCells[r, c].eastWall.GetComponent<MeshRenderer>();
					meshRenderer2.material = materialFinish;

					MeshRenderer meshRenderer3 = mazeCells[r, c].roof.GetComponent<MeshRenderer>();
					meshRenderer3.material = materialFinish;
				}
			}
		}


	}
}


