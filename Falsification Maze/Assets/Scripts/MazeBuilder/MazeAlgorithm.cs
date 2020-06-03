using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class MazeAlgorithm {
    // Maze
	protected MazeCell[,] mazeCells;
	protected int mazeRows, mazeColumns;

    // Finish
	public MazeCell finishCell;

    // Constructor
	protected MazeAlgorithm(MazeCell[,] mazeCells) : base() {
		this.mazeCells = mazeCells;
		mazeRows = mazeCells.GetLength(0);
		mazeColumns = mazeCells.GetLength(1);
	}

	public abstract void CreateMaze ();

    public (MazeCell, MazeCell) findWrongFinish(int pathLength){
        MazeCell next = mazeCells[0,0];
        MazeCell greenPathFinishCell = null;

        MazeCell prev = null;
        int halfPath = pathLength / 4;
        for (int i = 0; i <= halfPath-1; i++) {
            prev = next;
            next = next.next;
        }
        int l = 0;
        while (l <= 10) {
            prev = next;
            next = next.next;
            next.kids.Remove(prev);
            (int j, MazeCell path) = longestPath(next);
            l = j;
            greenPathFinishCell = path;
        }
        return (greenPathFinishCell, next);
    }
    public int addShortestPaths(Material materialPath, int r, int c, int fr, int fc) {

        foreach (MazeCell cell in mazeCells) {
            cell.discovered = false;
            cell.kids = new List<MazeCell>();
            cell.next = null;
        }

		finishCell = mazeCells[fr, fc];
        finishCell.discovered = true;
        List<MazeCell> finishPath = new List<MazeCell>();
        Queue<List<MazeCell>> currentPaths = new Queue<List<MazeCell>>();
        currentPaths.Enqueue(finishPath);
        Queue<MazeCell> mazeQueue = new Queue<MazeCell>();
        this.shortestPath(currentPaths, mazeQueue, finishCell);

        MazeCell next = mazeCells[r,c];
        int pathLength = next.drawRoute(materialPath, 0);

        return pathLength; // return value for wall coloring
    }

    public (int, MazeCell) longestPath(MazeCell cell) {
        int maxPathLength = 0;
        MazeCell maxDeepCell = cell;
        if (cell.kids.Count > 0) {
            foreach (MazeCell kid in cell.kids) {
                (int pathLength, MazeCell deepCell) = longestPath(kid);
                if (pathLength >= maxPathLength) {
                    maxPathLength = pathLength;
                    maxDeepCell = deepCell;
                }
            }
            return (maxPathLength + 1, maxDeepCell );
        }
        return (0, cell);
    }
    public void shortestPath(Queue<List<MazeCell>> currentPaths, Queue<MazeCell> mazeQueue, MazeCell cell) {
        if (cell.r>0 && !mazeCells[cell.r-1, cell.c].southWall.activeSelf && !mazeCells[cell.r-1, cell.c].discovered) {
            mazeQueue.Enqueue(mazeCells[cell.r-1, cell.c]);
            mazeCells[cell.r-1, cell.c].discovered = true;
            mazeCells[cell.r-1, cell.c].next = cell;
            cell.kids.Add(mazeCells[cell.r-1, cell.c]);
        }

        if (!cell.eastWall.activeSelf && cell.c<mazeColumns && !mazeCells[cell.r, cell.c+1].discovered) {
            mazeQueue.Enqueue(mazeCells[cell.r, cell.c+1]);
            mazeCells[cell.r, cell.c+1].discovered = true;
            mazeCells[cell.r, cell.c+1].next = cell;
            cell.kids.Add(mazeCells[cell.r, cell.c+1]);
        }

        if (!cell.southWall.activeSelf && cell.r<mazeRows && !mazeCells[cell.r+1, cell.c].discovered) {
            mazeQueue.Enqueue(mazeCells[cell.r+1, cell.c]);
            mazeCells[cell.r+1, cell.c].discovered = true;
            mazeCells[cell.r+1, cell.c].next = cell;
            cell.kids.Add(mazeCells[cell.r+1, cell.c]);
        }

        if (cell.c>0 && !mazeCells[cell.r, cell.c-1].eastWall.activeSelf && !mazeCells[cell.r, cell.c-1].discovered) {
            mazeQueue.Enqueue(mazeCells[cell.r, cell.c-1]);
            mazeCells[cell.r, cell.c-1].discovered = true;
            mazeCells[cell.r, cell.c-1].next = cell;
            cell.kids.Add(mazeCells[cell.r, cell.c-1]);
        }

        if (mazeQueue.Count > 0) {
            MazeCell next = mazeQueue.Dequeue();
            this.shortestPath(currentPaths, mazeQueue, next);
        }
    }

}
