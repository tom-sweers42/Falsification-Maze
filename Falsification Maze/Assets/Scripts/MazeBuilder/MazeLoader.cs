using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;
using SimpleJSON;

public class MazeLoader : MonoBehaviour {
    #region Variables
    // Maze intstantiators
    #region Maze Instantiators
	public int mazeRows, mazeColumns;
    public int randomDeletesDivider;
	public int randomAdditionsDivider;
	public MazeCell[,] mazeCells;
    public MazeCell[,] copyMazeCells;
	public float size = 2f;
    public float heigth = 2f;
    #endregion

    // Correctness of green path
    #region Green Path
    public bool correctPath;
    #endregion
    // Maze Gameobjects
    #region Maze Gameobjects
	public GameObject wall;

    public GameObject roof;
    public GameObject dotWall;
    #endregion

    // Maze Materials
    #region Maze Materials
	public Material materialFinish;
	public Material materialPath;
	public Material materialMarkerFar;
	public Material materialMarkerNear;
	public Material materialMarkerCorrect;
    public Material materialNormal;
    #endregion

    // Finishing
    #region Finishing
    private bool finished = false;
    private bool endGame = false;
    public GameObject levelComplete;
    public GameObject timeOver;
    public GameObject eos;
    #endregion


    // FM
    #region FM
    public TMP_Text fm;
    public MazeCell checkCell;
	public int initPathLength;
    #endregion

    // Data Collection
    #region Data Collectin
    public GameObject data;
    public FirstPersonAIO player;
    public int pauseCounter = 0;

    public PlayerCollider playerCollider;

    private float timeLookUp = 0f;
    #endregion

    //Timing
    #region Timing
    public GameObject timeSystem;
    public TMP_Text timeText;
    public TMP_Text levelText;
    public TMP_Text failedLevelText;
    public float start = 0;
    private float diff;
    public float maxTime = 5; //seconds
    private bool noTime = false;
    #endregion

    #endregion

    //======================================================================
    // LEVEL MANAGEMENT
    //======================================================================

    void Start () {



        data = GameObject.Find("Data");
		InitializeMaze ();
		MazeAlgorithm ma = new HuntAndKillMazeAlgorithm (mazeCells);
		ma.CreateMaze ();
		// DeleteRandomWalls();
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
            //Debug.Log(newFinish.r + ", " + newFinish.c);
            ma.addShortestPaths(materialPath,0,0, newFinish.r, newFinish.c);
            checkCell = mazeCells[checkCell.r, checkCell.c];
            for (int i = 0; i <= 5; i++) {
                checkCell = checkCell.next;
            }
        }
	}

	// Update is called once per frame
	void Update () {
        if (fm != null) {
            fm.gameObject.transform.position = new Vector3 (Screen.width * 0.5f, Screen.height * 0.2f,0);
        }
        // Debug.Log(Camera.main.transform.rotation.x);
        float delta = Time.deltaTime;
        if (!finished && !noTime){
            if (player.playerCamera.transform.localRotation[0] < -0.37) {
                timeLookUp += delta;
                //Debug.Log(timeLookUp);
            }
            start += delta;
            diff = maxTime - start;
            string minutes = Mathf.Floor(diff / 60).ToString("00");
            string seconds = (diff%60).ToString("00");
            timeText.text = "TIME " + minutes + ":" + seconds;
            if (diff <= 0 && !noTime)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
                Time.timeScale = 0f;
                timeOver.SetActive(true);
                timeText.text = "";
                failedLevelText.text = "LEVEL " + CrossSceneInformationClass.level;
                gameLost();
                noTime = true;
            }
        }
        if (noTime & (Input.GetKeyDown(KeyCode.Space)))
        {
            noTime = false;
            CrossSceneInformationClass.level += 1;
            //Debug.Log("CrossSceneInformationClass: " + CrossSceneInformationClass.level);
            if (CrossSceneInformationClass.level < 5)
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(CrossSceneInformationClass.level);
            }
            else
            {
                eos.SetActive(true);
                endGame = true;
                //Application.Quit();
            }
        }
        if (finished & (Input.GetKeyDown(KeyCode.Space)))
        {
            CrossSceneInformationClass.level += 1;
            //Debug.Log("CrossSceneInformationClass: " + CrossSceneInformationClass.level);
            if (CrossSceneInformationClass.level < 5)
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(CrossSceneInformationClass.level);
            }
            else
            {
                eos.SetActive(true);
                endGame = true;
                //Application.Quit();
            }
        }
        if (endGame & (Input.GetKeyDown(KeyCode.M)))
        {
            SceneManager.LoadScene("Menu");
        }
    }

    //======================================================================
    // MAZE GENERATION
    //======================================================================

    private void InitializeMaze()
    {

        mazeCells = new MazeCell[mazeRows, mazeColumns];
        for (int r = 0; r < mazeRows; r++)
        {
            for (int c = 0; c < mazeColumns; c++)
            {
                mazeCells[r, c] = new MazeCell(c, r);
                // For now, use the same wall object for the floor!
                mazeCells[r, c].floor = Instantiate(roof, new Vector3(r * size, -(heigth / 2f), c * size), Quaternion.identity) as GameObject;
                mazeCells[r, c].floor.transform.localScale = new Vector3(size, size, 0.6f);
                mazeCells[r, c].floor.name = "Floor " + r + "," + c;
                mazeCells[r, c].floor.transform.Rotate(Vector3.right, 90f);

                mazeCells[r, c].roof = Instantiate(roof, new Vector3(r * size, (heigth / 2f), c * size), Quaternion.identity) as GameObject;
                mazeCells[r, c].roof.transform.localScale = new Vector3(size, size, 0.6f);
                mazeCells[r, c].roof.name = "Roof " + r + "," + c;
                mazeCells[r, c].roof.transform.Rotate(Vector3.right, 90f);


                if (c == 0)
                {
                    mazeCells[r, c].westWall = Instantiate(wall, new Vector3(r * size, 0, (c * size) - (size / 2f)), Quaternion.identity) as GameObject;
                    mazeCells[r, c].westWall.transform.localScale = new Vector3(size, heigth, 0.6f);
                    mazeCells[r, c].westWall.name = "West Wall " + r + "," + c;
                }

                mazeCells[r, c].eastWall = Instantiate(wall, new Vector3(r * size, 0, (c * size) + (size / 2f)), Quaternion.identity) as GameObject;
                mazeCells[r, c].eastWall.transform.localScale = new Vector3(size, heigth, 0.6f);
                mazeCells[r, c].eastWall.name = "East Wall " + r + "," + c;

                if (r == 0)
                {
                    mazeCells[r, c].northWall = Instantiate(wall, new Vector3((r * size) - (size / 2f), 0, c * size), Quaternion.identity) as GameObject;
                    mazeCells[r, c].northWall.transform.localScale = new Vector3(size, heigth, 0.6f);
                    mazeCells[r, c].northWall.name = "North Wall " + r + "," + c;
                    mazeCells[r, c].northWall.transform.Rotate(Vector3.up * 90f);

                }

                mazeCells[r, c].southWall = Instantiate(wall, new Vector3((r * size) + (size / 2f), 0, c * size), Quaternion.identity) as GameObject;
                mazeCells[r, c].southWall.transform.localScale = new Vector3(size, heigth, 0.6f);
                mazeCells[r, c].southWall.name = "South Wall " + r + "," + c;
                mazeCells[r, c].southWall.transform.Rotate(Vector3.up * 90f);

                if (c == mazeColumns - 1 && r == mazeRows - 1)
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

    public void DeleteRandomWalls()
    {
        int randomDeletes = Convert.ToInt32((mazeColumns * mazeRows) / randomDeletesDivider);
        for (int i = 0; i < randomDeletes; i++)
        {
            int r = UnityEngine.Random.Range(0, mazeRows - 1);
            int c = UnityEngine.Random.Range(0, mazeColumns - 1);
            int direction = UnityEngine.Random.Range(0, 2);

            if (direction == 0)
            {
                if (!DestroyWallIfItExists(mazeCells[r, c].southWall))
                {
                    i--;
                }
            }
            else
            {
                if (!DestroyWallIfItExists(mazeCells[r, c].eastWall))
                {
                    i--;
                }
            }
        }
    }

    private bool DestroyWallIfItExists(GameObject wall)
    {
        if (wall != null)
        {
            GameObject.Destroy(wall);
            wall.SetActive(false);
            return true;
        }
        return false;
    }

    public int LongestPathLength(MazeCell cell)
    {
        int maxPathLength = 0;
        if (cell.kids.Count > 0)
        {
            foreach (MazeCell kid in cell.kids)
            {
                int pathLength = LongestPathLength(kid);
                if (pathLength > maxPathLength)
                    maxPathLength = pathLength;
            }
            return maxPathLength + 1;
        }
        else
            return 0;
    }

    public void ClearPath()
    {
        for (int r = 0; r < mazeRows; r++)
        {
            for (int c = 0; c < mazeColumns; c++)
            {
                if (c != mazeColumns - 1 || r != mazeRows - 1)
                {
                    MeshRenderer meshRenderer = mazeCells[r, c].floor.GetComponent<MeshRenderer>();
                    MeshRenderer meshRendererCopy = copyMazeCells[r, c].floor.GetComponent<MeshRenderer>();
                    meshRenderer.material = materialNormal;
                    meshRendererCopy.material = materialNormal;
                }
            }
        }
    }

    //======================================================================
    // ADD COLOR CUES
    //======================================================================

    void AddPathBasedColor()
    {

		for (int r = 0; r < mazeRows; r++) {
			for (int c = 0; c < mazeColumns; c++) {
                MazeCell cell = mazeCells[r,c];
                if (cell.hasMoreThanOneOpening(mazeCells)) {
                    ColorCode(cell);
                }
            }
        }
    }

    public Color colourDirDot(int pathLength, int newPathLength)
    {
        int diff = pathLength - newPathLength;
        if (diff == 1)
        {
            return Color.green;
        }
        else if (diff == -1)
        {
            return Color.blue;
        }
        else
        {
            return Color.blue;
        }
    }

    void ColorCode(MazeCell cell) {
        int maxLength = LongestPathLength(mazeCells[mazeRows-1, mazeColumns-1]);

        int pathLength = cell.drawRoute(materialPath, 0);
        float divisor = (float) pathLength/maxLength;
        var curDotWall = GameObject.Instantiate(dotWall, cell.roof.transform.position, cell.roof.transform.rotation);
        curDotWall.transform.localScale = new Vector3(size,size,0.6f);
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

    //======================================================================
    // GAME MANAGEMENT
    //======================================================================

    public void gameWon()
    {
        MazeData mazeData = data.GetComponent<MazeData>();
        mazeData.timeLevels.Add(start);
        levelComplete.SetActive(true);
        timeText.text = "";
        levelText.text = "LEVEL " + CrossSceneInformationClass.level;
        Time.timeScale = 0f;
        finished = true;
        gameOver();

    }

    public void gameLost()
    {
        MazeData mazeData = data.GetComponent<MazeData>();
        mazeData.timeLevels.Add(-1f);
        gameOver();
    }

    public void gameOver(){
        Debug.Log("Level: " + CrossSceneInformationClass.level + "Game Over...");
        // update Maze Data
        MazeData mazeData = data.GetComponent<MazeData>();
        mazeData.timeLookUpLevels.Add(timeLookUp);
        mazeData.tileCounterLevels.Add(playerCollider.tilesCounter);
        mazeData.tileCorrectCounterLevels.Add(playerCollider.totalCorrectCounter);
        mazeData.tileWrongCounterLevels.Add(playerCollider.totalWrongCounter);
        mazeData.fmCounterLevels.Add(playerCollider.fmCounter);
        mazeData.fmTimeStampsLevels.Add(playerCollider.fmTimeStamps);
        mazeData.pauseCounterLevels.Add(pauseCounter);
        mazeData.intialShortestPathLengthLevels.Add(initPathLength);
        mazeData.timeStampLevels.Add(GetTimestamp(DateTime.Now));

        // send maze data to firebase
        Debug.Log("Does it reach this!");
        StartCoroutine(SendData(mazeData, CrossSceneInformationClass.level));
    }
    public static String GetTimestamp(DateTime value)
    {
        return value.ToString("dd-MM-yyyy -- HH:mm:ss");
    }
    //======================================================================
    // DATA MANAGEMENT
    //======================================================================

    // initialize default URIs and API Key
    private readonly string tokenApi = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=AIzaSyC21UEISTGjswnV665STXXQ4ksiIXnkyZw";
    private readonly string firebaseApi = "https://falsificationmaze.firebaseio.com/users/";
    private string apiKey = "";

    IEnumerator SendData(MazeData mazeData, int level)
    {
        // get API Auth Key
        using (UnityWebRequest apiKeyRequest = UnityWebRequest.Post(tokenApi, new WWWForm()))
        {
            // send request
            Debug.Log(mazeData.playerId);
            Debug.Log(level);
            Debug.Log("Does it get here then?");
            Debug.Log(CrossSceneInformationClass.level);
            Debug.Log(apiKeyRequest);
            yield return apiKeyRequest.SendWebRequest();
            Debug.Log(apiKeyRequest.responseCode);
            // check for errors
            if (apiKeyRequest.isNetworkError || apiKeyRequest.isHttpError)
            {
                Debug.Log("dfsdf");
                Debug.LogError(apiKeyRequest.error);
            }
            else
            {
                // parse API response
                JSONNode apiKeyResponse = JSON.Parse(apiKeyRequest.downloadHandler.text);

                // fetch API Auth Key
                apiKey = apiKeyResponse["idToken"];

                Debug.Log("API Key: " + apiKey);
            }

        }
        Debug.Log("So does it get here?");
        // define JSON body for HTTP REST PUT request
        string jsonBody = JsonUtility.ToJson(mazeData);

        Debug.Log(jsonBody);

        // define firebaseUri
        // (using base Uri + new userId json filename + auth key)
        string firebasePutUri = firebaseApi + mazeData.playerId + "/level" + level.ToString() + ".json?auth=" + apiKey;

        Debug.Log(firebasePutUri);

        // send user data
        using (UnityWebRequest firebaseRequest = UnityWebRequest.Put(firebasePutUri, jsonBody))
        {
            // send request
            yield return firebaseRequest.SendWebRequest();

            // check for errors
            if (firebaseRequest.isNetworkError || firebaseRequest.isHttpError)
            {
                Debug.LogError(firebaseRequest.error);
            }
            else
            {
                Debug.Log("Data Sent!");
            }
        }
    }

}


