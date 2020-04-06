using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MazeCell {
    public int c,r;
	public bool visited = false;
    public bool discovered = false;
    public List<MazeCell> path;
    public MazeCell next;
	public GameObject northWall, southWall, eastWall, westWall, floor, roof;

    public MazeCell(int c, int r){
        this.c = c;
        this.r = r;
    }

    public void drawRoute(Material materialPath) {
        MeshRenderer meshRenderer = this.floor.GetComponent<MeshRenderer>();
        meshRenderer.material = materialPath;
        if (next != null) {
            next.drawRoute(materialPath);
        }

    }
}
