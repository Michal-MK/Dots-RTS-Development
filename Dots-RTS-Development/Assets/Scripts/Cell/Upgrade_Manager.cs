using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade_Manager : MonoBehaviour {

	public GameObject[] slots = new GameObject[8];

	public enum Slot {
		First,
		Second,
		Third,
		Fourth,
		Fifht,
		Sixth,
		Seventh,
		Eigth,
	}

	public enum Upgrade {
		NoUpgrade,
		ElementMovementSpeed,
		RegenerationSpeed,
		MaxCapacity,
		DoubleDmgChance,
	}

	//Installs Upgrade onto specifiad slot
	public void InstallUpgrade(Upgrade type, Slot number) {
		slots[(int)number].GetComponent<UpgradeSlotState>().current = type;
	}

	//Removes Upgrade from a specified slot
	public void UninstallUpgrade(Slot number) {
		slots[(int)number].GetComponent<UpgradeSlotState>().current = 0;
	}
}
