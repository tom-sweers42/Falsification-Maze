using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    private GameObject curFloor;
    public MazeLoader gameManager;
    public Material materialPath;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other) {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.name.StartsWith("Floor") && curFloor != other.gameObject) {
            curFloor = other.gameObject;
            string pair = curFloor.name.Split(' ')[1];
            int r = Int32.Parse(pair.Split(',')[0]);
            int c = Int32.Parse(pair.Split(',')[1]);
            gameManager.clearPath();
            gameManager.mazeCells[r,c].drawRoute(materialPath);

        }
    }
}
