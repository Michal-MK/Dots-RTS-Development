using UnityEngine;
using System.Collections.Generic;
using System;

public class Player : MonoBehaviour,IAlly {
	//Cell.enmTeam playerTeam;
	//List<Enemy_AI> alliesOfPlayer;
	//List<Enemy_AI> targetsOfPlayer;

	public ExtraPlayer pl1;
	public ExtraPlayer pl2;
	public ExtraPlayer pl3;
	public Player pl4;

	public List<IAlly> listOfAlly = new List<IAlly>();
	public List<CellBehaviour> playerCells = new List<CellBehaviour>();

	//private void Start() {
	//	foreach (CellBehaviour cell in PlayManager.cells) {
	//		if (cell.cellTeam == Cell.enmTeam.ALLIED) {
	//			playerCells.Add(cell);
	//		}
	//	}
	//}
	private void Start() {
		listOfAlly.Add(pl1);
		//listOfAlly.Add(pl2);
		//listOfAlly.Add(pl3);
		//listOfAlly.Add(pl4);
	}


	public bool IsAllyOF(IAlly other) {
		//ExtraPlayer playah = (ExtraPlayer)other;
		//playah.PRintCrap();
		foreach (IAlly ally in listOfAlly) {

			if (ally == other) {
				try {
					print(((Enemy_AI)other).name);
				}
				catch(Exception e) {
					Debug.Log(e);
				}
				print(((ExtraPlayer)other).name);

				print("Found a match" + ally + " " + other);
				return true;
			}
			else {
				print("NOG");
			}

		}
		return false;
	}

	public void PRintCrap() {
		//print("Stuff yaay");
	}
}