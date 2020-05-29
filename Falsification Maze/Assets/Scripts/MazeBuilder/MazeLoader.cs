using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

public class MazeLoader : MonoBehaviour {
	public int mazeRows, mazeColumns;
    public int randomDeletesDivider;
	public int randomAdditionsDivider;
	public GameObject wall;

    public GameObject roof;
    public GameObject dotWall;
	public float size = 2f;
    public float heigth = 2f;
    public float timeLimit;
    private float timeLeft;
    private float initialIntensity;

    public bool correctPath;
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
    public MazeCell[,] copyMazeCells;
    public GameObject tilesCounterFieldObject;
    public Text tilesCounterField;
    public TMP_Text fm;
    public MazeCell checkCell;
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
		// DeleteRandomWalls();
        // AddRandomSymbols();
        initPathLength = ma.addShortestPaths(materialPath, 0, 0, mazeRows-1, mazeColumns -1);
        copyMazeCells = mazeCells.Clone() as MazeCell[,];
        AddPathBasedColor();
        if (!correctPath) {
            MazeCell newFinish = null;
            (newFinish, checkCell) = ma.findWrongFinish(initPathLength);
            for (int r = 0; r < mazeRows; r++) {
                for (int c = 0; c < mazeColumns; c++) {
                    mazeCells[r,c] = mazeCells[r,c].DeepCopy();
                }
            }
            Debug.Log(newFinish.r + ", " + newFinish.c);
            ma.addShortestPaths(materialPath,0,0, newFinish.r, newFinish.c);
            checkCell = mazeCells[checkCell.r, checkCell.c];
            for (int i = 0; i <= 5; i++) {
                checkCell = checkCell.next;
            }
        }
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

    void AddPathBasedColor()
    {

		for (int r = 0; r < mazeRows; r++) {
			for (int c = 0; c < mazeColumns; c++) {
                MazeCell cell = mazeCells[r,c];
                if (cell.hasMoreThanOneOpening(mazeCells)) {
                    ColorCode(cell);
                    // ColorObject(r,c, initPathLength, pathLength);
                }
            }
        }

        // int randomAdditions = Convert.ToInt32((mazeColumns * mazeRows) / randomAdditionsDivider);
        // for (int i = 0; i < randomAdditions; i++)
        // {
        //     int r = UnityEngine.Random.Range(0, mazeRows - 1);
        //     int c = UnityEngine.Random.Range(0, mazeColumns - 1);
        //     cellPathLength = ma.addShortestPaths(materialPath, r, c, true);
        //     ColorObject(r, c, initPathLength, cellPathLength);
        // }
    }
    void ColorCode(MazeCell cell) {
        int maxLength = LongestPathLength(mazeCells[mazeRows-1, mazeColumns-1]);

        int pathLength = cell.drawRoute(materialPath, 0);
        float divisor = (float) pathLength/maxLength;
        var curDotWall = GameObject.Instantiate(dotWall, cell.roof.transform.position, cell.roof.transform.rotation);
        var middleDot = curDotWall.transform.Find("MiddleDot");
        var upDot = curDotWall.transform.Find("UpDot");
        var downDot = curDotWall.transform.Find("DownDot");
        var leftDot = curDotWall.transform.Find("LeftDot");
        var rightDot = curDotWall.transform.Find("RightDot");
        Destroy(cell.roof);
        MeshRenderer meshRenderer = middleDot.GetComponent<MeshRenderer>();
        meshRenderer.material = materialMarkerCorrect;
        meshRenderer.material.color = new Color((float) 1f-0.06f*(1f-divisor),(float) 1-divisor,0f,1f);


        if (!cell.southWall.activeSelf)
        {
            MazeCell southCell = mazeCells[cell.r+1, cell.c];
            int southCellPathLength = southCell.drawRoute(materialPath,0);

            var meshRendererSouth = leftDot.GetComponent<MeshRenderer>();
            meshRendererSouth.material = materialMarkerCorrect;
            meshRendererSouth.material.color =  colourDirDot(pathLength, southCellPathLength);
        }
        else
            Destroy(leftDot.gameObject);
        if (!cell.eastWall.activeSelf)
        {
            MazeCell eastCell = mazeCells[cell.r, cell.c+1];
            int eastCellPathLength = eastCell.drawRoute(materialPath,0);

            var meshRendererEast = upDot.GetComponent<MeshRenderer>();
            meshRendererEast.material = materialMarkerCorrect;
            meshRendererEast.material.color =  colourDirDot(pathLength, eastCellPathLength);
        }
        else
            Destroy(upDot.gameObject);
        if (cell.r == 0){
            if (!cell.northWall.activeSelf) {
                MazeCell northCell = mazeCells[cell.r-1, cell.c];
                int northCellPathLength = northCell.drawRoute(materialPath,0);

                var meshRendererNorth = rightDot.GetComponent<MeshRenderer>();
                meshRendererNorth.material = materialMarkerCorrect;
                meshRendererNorth.material.color =  colourDirDot(pathLength, northCellPathLength);
            }
            else
                Destroy(rightDot.gameObject);
        }
        if (cell.c == 0) {
            if (!cell.westWall.activeSelf) {
                MazeCell westCell = mazeCells[cell.r, cell.c-1];
                int westCellPathLength = westCell.drawRoute(materialPath,0);

                var meshRendererWest = downDot.GetComponent<MeshRenderer>();
                meshRendererWest.material = materialMarkerCorrect;
                meshRendererWest.material.color =  colourDirDot(pathLength, westCellPathLength);
            }
            else
                Destroy(downDot.gameObject);
        }
        if (cell.r > 0 && !mazeCells[cell.r-1,cell.c].southWall.activeSelf) {
            MazeCell northCell = mazeCells[cell.r-1, cell.c];
            int northCellPathLength = northCell.drawRoute(materialPath,0);

            var meshRendererNorth = rightDot.GetComponent<MeshRenderer>();
            meshRendererNorth.material = materialMarkerCorrect;
            meshRendererNorth.material.color =  colourDirDot(pathLength, northCellPathLength);
        }
        else
            Destroy(rightDot.gameObject);
        if (cell.c > 0 && !mazeCells[cell.r,cell.c-1].eastWall.activeSelf)
        {
                MazeCell westCell = mazeCells[cell.r, cell.c-1];
                int westCellPathLength = westCell.drawRoute(materialPath,0);

                var meshRendererWest = downDot.GetComponent<MeshRenderer>();
                meshRendererWest.material = materialMarkerCorrect;
                meshRendererWest.material.color =  colourDirDot(pathLength, westCellPathLength);
            }
            else
                Destroy(downDot.gameObject);



        Texture2D texture = new Texture2D(128, 128);
        cell.roof.GetComponent<Renderer>().material.mainTexture = texture;

    }

    public Color colourDirDot(int pathLength, int newPathLength) {
        int diff = pathLength - newPathLength;
        if (diff == 1) {
            return Color.green;
        }
        else if (diff == -1) {
            return Color.blue;
        }
        else {
            return Color.blue;
        }
    }
    public int LongestPathLength(MazeCell cell) {
        int maxPathLength = 0;
        if (cell.kids.Count > 0) {
            foreach (MazeCell kid in cell.kids) {
                int pathLength = LongestPathLength(kid);
                if (pathLength > maxPathLength)
                    maxPathLength = pathLength;
            }
            return maxPathLength + 1;
        }
        else
            return 0;
        return maxPathLength;
    }
    void ColorObject(int r, int c, int initPathLength, int cellPathLength)
    {
        // int direction = UnityEngine.Random.Range(0, 3);
        int direction = 2;
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
					MeshRenderer meshRendererCopy = copyMazeCells[r, c].floor.GetComponent<MeshRenderer>();
					meshRenderer.material = materialNormal;
					meshRendererCopy.material = materialNormal;
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
				mazeCells [r, c] .floor = Instantiate (roof, new Vector3 (r*size, -(heigth/2f), c*size), Quaternion.identity) as GameObject;
				mazeCells [r, c] .floor.name = "Floor " + r + "," + c;
				mazeCells [r, c] .floor.transform.Rotate (Vector3.right, 90f);

				mazeCells [r, c] .roof = Instantiate (roof, new Vector3 (r*size, (heigth/2f), c*size), Quaternion.identity) as GameObject;
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


