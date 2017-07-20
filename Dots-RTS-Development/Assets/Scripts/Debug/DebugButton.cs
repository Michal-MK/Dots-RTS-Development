using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugButton : MonoBehaviour {

	public Enemy_AI aiScript;

	public void ListMembers() {
		print("Targets-----------------------------------------");
		for (int i = 0; i < aiScript._targets.Count; i++) {
			print(aiScript._targets[i].gameObject.name);
		}
		print("AI-----------------------------------------");
		for (int i = 0; i < aiScript._aiCells.Count; i++) {
			print(aiScript._aiCells[i].gameObject.name);
		}
		print("Neutrals-----------------------------------------");
		for (int i = 0; i < aiScript._neutrals.Count; i++) {
			print(aiScript._neutrals[i].gameObject.name);
		}
	}

	public void ListCells() {
		foreach (Cell item in Control.cells) {
			print(item.gameObject.name);
		}
		print(Control.cells.Count);
	}
}
