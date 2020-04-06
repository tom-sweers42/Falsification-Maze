﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class MazeAlgorithm {
	protected MazeCell[,] mazeCells;
	protected int mazeRows, mazeColumns;

	public MazeCell finishCell;
	protected MazeAlgorithm(MazeCell[,] mazeCells) : base() {
		this.mazeCells = mazeCells;
		mazeRows = mazeCells.GetLength(0);
		mazeColumns = mazeCells.GetLength(1);
	}

	public abstract void CreateMaze ();
    public void addShortestPaths(Material materialPath) {
		finishCell = mazeCells[mazeRows-1, mazeColumns-1];
        finishCell.discovered = true;
        List<MazeCell> finishPath = new List<MazeCell>();
        Queue<List<MazeCell>> currentPaths = new Queue<List<MazeCell>>();
        currentPaths.Enqueue(finishPath);
        Queue<MazeCell> mazeQueue = new Queue<MazeCell>();
        this.shortestPath(currentPaths, mazeQueue, finishCell);

        // foreach (var cell in mazeCells[0,0].path) {
        //     MeshRenderer meshRenderer = cell.floor.GetComponent<MeshRenderer>();
        //     meshRenderer.material = materialPath;
        // }
        MazeCell next = mazeCells[0,0];
        next.drawRoute(materialPath);
    }

    public void shortestPath(Queue<List<MazeCell>> currentPaths, Queue<MazeCell> mazeQueue, MazeCell cell) {
        // List<MazeCell> currentPath = new List<MazeCell>(currentPaths.Dequeue());
        // currentPath.Add(cell);
        // cell.path = currentPath;
        if (cell.r>0 && !mazeCells[cell.r-1, cell.c].southWall.activeSelf && !mazeCells[cell.r-1, cell.c].discovered) {
            // Debug.Log("Path North");
            // currentPaths.Enqueue(cell.path);
            mazeQueue.Enqueue(mazeCells[cell.r-1, cell.c]);
            mazeCells[cell.r-1, cell.c].discovered = true;
            mazeCells[cell.r-1, cell.c].next = cell;
        }

        if (!cell.eastWall.activeSelf && cell.c<mazeColumns && !mazeCells[cell.r, cell.c+1].discovered) {
            // Debug.Log("Path East");
            // currentPaths.Enqueue(cell.path);
            mazeQueue.Enqueue(mazeCells[cell.r, cell.c+1]);
            mazeCells[cell.r, cell.c+1].discovered = true;
            mazeCells[cell.r, cell.c+1].next = cell;
        }

        if (!cell.southWall.activeSelf && cell.r<mazeRows && !mazeCells[cell.r+1, cell.c].discovered) {
            // Debug.Log("Path South");
            // currentPaths.Enqueue(cell.path);
            mazeQueue.Enqueue(mazeCells[cell.r+1, cell.c]);
            mazeCells[cell.r+1, cell.c].discovered = true;
            mazeCells[cell.r+1, cell.c].next = cell;
        }

        if (cell.c>0 && !mazeCells[cell.r, cell.c-1].eastWall.activeSelf && !mazeCells[cell.r, cell.c-1].discovered) {
            // Debug.Log("Path West");
            // currentPaths.Enqueue(cell.path);
            mazeQueue.Enqueue(mazeCells[cell.r, cell.c-1]);
            mazeCells[cell.r, cell.c-1].discovered = true;
            mazeCells[cell.r, cell.c-1].next = cell;
        }
        if (mazeQueue.Count > 0) {
            MazeCell next = mazeQueue.Dequeue();
            this.shortestPath(currentPaths, mazeQueue, next);
        }


    }
}
