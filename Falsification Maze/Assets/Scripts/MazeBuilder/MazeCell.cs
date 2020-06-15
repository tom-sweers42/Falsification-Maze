
// The code in this file is taken from https://github.com/lonedevdotcom/MazeGenerator at 29-03-2020
// The author is github user lonedevdotcom
// The code in this file is changed signifcantly. only the class definition and the wall and floor attributes are not made by us
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


// The class definition is not from us.
public class MazeCell {

    //Gameobjects: The first 5 were made by the original authors. The roof was added by us.
	public GameObject northWall, southWall, eastWall, westWall, floor, roof;

    // Mazeinfo
    public int c,r;

    // Path
	public bool visited = false;
    public bool discovered = false;
    public List<MazeCell> path;
    public MazeCell next;
    public List<MazeCell> kids;

    public MazeCell(int c, int r){
        this.c = c;
        this.r = r;
        kids = new List<MazeCell>();
    }
    public MazeCell DeepCopy()
    {
        MazeCell other = (MazeCell) this.MemberwiseClone();
        other.kids = new List<MazeCell>();
        other.next = null;
        return other;
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
        return counter>2;
    }
    public bool hasMoreThanOneOpeningNullVersion(MazeCell[,] mazeCells ) {
        int counter = 0;
        if (southWall == null)
        {
            counter++;
        }
        if (eastWall == null)
        {
            counter++;
        }
        if (r == 0){
            if (northWall == null) {
                counter++;
            }
        }
        if (c == 0) {
            if (westWall == null) {
                counter++;
            }
        }
        if (r > 0 && mazeCells[r-1,c].southWall == null) {
            counter++;
        }
        if (c > 0 && mazeCells[r,c-1].eastWall == null)
        {
            counter++;

        }
        return counter>2;
    }
}
