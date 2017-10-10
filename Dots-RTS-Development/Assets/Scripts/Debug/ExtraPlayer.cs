using UnityEngine;
using System.Collections;
using System;

public class ExtraPlayer : MonoBehaviour, IAlly {

	public bool AMIANALLY;
	public Player MATE;
	private void Start() {
		if (this.IsAllyOf(MATE)) {
			print(this.name + " Is an Ally of " + MATE.name);
		}
	}

	public bool IsAllyOF(IAlly other) {
		return AMIANALLY;
	}


	public void PRintCrap() {
		//print("Stuff yaay");
	}
}
