using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControll : MonoBehaviour {

	public delegate void TeamChangeEventHandler(CellScript sender, CellScript.enmTeam previous, CellScript.enmTeam current);

	public bool isSinglePlayer = true;

	public static List<CellScript> cells = new List<CellScript>();

	public static void GameOver() {
		print("You Lost");
	}

	public static void YouWon() {
		print("You Won");
	}
}

