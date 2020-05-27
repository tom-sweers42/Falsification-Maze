using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MazeCell {
    public int c,r;
	public bool visited = false;
    public bool discovered = false;
    public List<MazeCell> path;
    public MazeCell next;

    public List<MazeCell> kids;
	public GameObject northWall, southWall, eastWall, westWall, floor, roof;

    public MazeCell(int c, int r){
        this.c = c;
        this.r = r;
        kids = new List<MazeCell>();
    }

    public int drawRoute(Material materialPath, int counter) {
        MeshRenderer meshRenderer = this.floor.GetComponent<MeshRenderer>();
        meshRenderer.material = materialPath;
        if (next != null) {
            counter = next.drawRoute(materialPath, counter + 1);
        }

        return counter;

    }

    public bool hasMoreThanOneOpening(MazeCell[,] mazeCells ) {
        Debug.Log(r);
        Debug.Log(c);
        int counter = 0;
        if (!southWall.activeSelf)
        {
            counter++;
        }
        if (!eastWall.activeSelf)
        {
            counter++;
        }
        if (r == 0){
            if (!northWall.activeSelf) {
                counter++;
            }
        }
        if (c == 0) {
            if (!westWall.activeSelf) {
                counter++;
            }
        }
        if (r > 0 && !mazeCells[r-1,c].southWall.activeSelf) {
            counter++;
        }
        if (c > 0 && !mazeCells[r,c-1].eastWall.activeSelf)
        {
            counter++;

        }
        // Debug.Log(counter);
        Debug.Log(counter);
        return counter>2;
    }
    public void showDirection(){
        if (next != null) {
            int rDir = next.r - r;
            int cDir = next.c - c;

            if (rDir == 0 && cDir == -1) {
                //north
                Debug.Log("NORTH");
            }
            if (rDir == 0 && cDir == 1) {
                //south
                Debug.Log("SOUTH");
            }
            if (rDir == -1 && cDir == 0) {
                //west
                Debug.Log("WEST");
            }
            if (rDir == 1 && cDir == 0) {
                //east
                Debug.Log("EAST");
            }
        }
    }
}
