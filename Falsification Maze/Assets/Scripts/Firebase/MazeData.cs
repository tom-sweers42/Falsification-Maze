using UnityEngine;

using System.Collections;
using System.Collections.Generic;
public class MazeData : MonoBehaviour
{
    private bool created = false;
    public string playerId;

    public List<float> timeLevels = new List<float>();
    public List<float> timeLookUpLevels = new List<float>();

    public List<int> tileCounterLevels = new List<int>();
    public List<int> tileCorrectCounterLevels = new List<int>();
    public List<int> tileWrongCounterLevels = new List<int>();

    public List<int> intialShortestPathLengthLevels = new List<int>();
    public List<int> fmCounterLevels = new List<int>();
    public List<List<float>> fmTimeStampsLevels = new List<List<float>>();

    public List<int> pauseCounterLevels = new List<int>();


    void Start() {

        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }

        else
        {
            Destroy(this.gameObject);
        }

    }

}
