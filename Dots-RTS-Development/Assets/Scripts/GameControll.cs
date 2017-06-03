using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControll : MonoBehaviour {

	public static List<CellScript> cells = new List<CellScript>();

	public static void GameOver() {
		print("You Lost");
	}
}

