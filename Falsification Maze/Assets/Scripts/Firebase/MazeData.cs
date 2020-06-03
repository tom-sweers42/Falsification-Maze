using UnityEngine;

using System.Collections;
using System.Collections.Generic;
public class MazeData : MonoBehaviour
{
    private bool created = false;
    public string playerId;

    public List<float> timeLevels = new List<float>();
    public float timeLevel1;
    public float timeLevel2;
    public float timeLevel3;
    public float timeLevel4;

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
