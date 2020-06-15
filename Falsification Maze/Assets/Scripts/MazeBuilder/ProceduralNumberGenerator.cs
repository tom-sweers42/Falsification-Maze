using UnityEngine;
using System.Collections;

// The code in this file is taken from https://github.com/lonedevdotcom/MazeGenerator at 29-03-2020
// The author is github user lonedevdotcom
// The code in this file is unchanged.
public class ProceduralNumberGenerator {
	public static int currentPosition = 0;
	public const string key = "12342412334242143223111244441212334432121223344";

	public static int GetNextNumber() {
		string currentNum = key.Substring(currentPosition++ % key.Length, 1);
		return int.Parse (currentNum);
	}
}
