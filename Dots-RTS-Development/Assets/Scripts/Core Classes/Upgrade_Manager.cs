using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Upgrade_Manager : MonoBehaviour {

	public static bool isUpgrading = false;

	public Upgrade.Upgrades[] upgrades = new Upgrade.Upgrades[8] {
		Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,
	};

	/// <summary>
	/// Adds upgrades from a save file or otherwise defined source
	/// </summary>
	public Upgrade.Upgrades[] PreinstallUpgrades {
		get { return upgrades; }
		set { upgrades = value;
			UpgradePreinstallSprites();
		}

	}

	protected virtual void UpgradePreinstallSprites() {
		
	}

	/// <summary>
	/// Installs upgrade to set slot
	/// </summary>
	public void InstallUpgrade(Cell cell,int slot, Upgrade.Upgrades upgrade) {
		upgrades[slot] = upgrade;
		if((int)upgrade >= 200) {
			switch (upgrade) {
				case Upgrade.Upgrades.UTIL_FASTER_ELEMENT_SPEED: {
					cell.elementSpeed += 3;
					return;
				}
				case Upgrade.Upgrades.UTIL_FASTER_REGENERATION: {
					cell.regenPeriod -= 0.4f;
					return;
				}
			}
		}
	}

	/// <summary>
	/// Uninstalls upgrade by slot
	/// </summary>
	public void UninstallUpgrade(int slot) {
		upgrades[slot] = Upgrade.Upgrades.NONE;
	}

	/// <summary>
	/// Uninstalls upgrade by type
	/// </summary>
	public void UninstallUpgrade(Upgrade.Upgrades type) {
		for (int i = 0; i < upgrades.Length; i++) {
			if (upgrades[i] == type) {
				upgrades[i] = Upgrade.Upgrades.NONE;
			}
		}
	}
	/// <summary>
	/// Does the cell have this type of upgrade already?
	/// </summary>
	public bool HasUpgade(Upgrade.Upgrades type) {
		for (int i = 0; i < upgrades.Length; i++) {
			if (upgrades[i] == type) {
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 	Does the cell have this type of upgrade already?
	/// </summary>
	/// <param name="type"></param>
	/// <returns>Amount of upgrades of type "type" in this cell.</returns>
	public int HasUpgrade(Upgrade.Upgrades type) {
		int _count = 0;
		for (int i = 0; i < upgrades.Length; i++) {
			if (upgrades[i] == type) {
				_count++;
			}
		}
		return _count;
	}

	/// <summary>
	/// Can player install additional upgrades?
	/// </summary>
	public bool HasFreeSlots() {
		for (int i = 0; i < upgrades.Length; i++) {
			if (upgrades[i] == Upgrade.Upgrades.NONE) {
				return true;
			}
		}
		return false;
	}

	public int GetFirstFreeSlot() {
		for (int i = 0; i < upgrades.Length; i++) {
			if(upgrades[i] == Upgrade.Upgrades.NONE) {
				return i;
			}
		}
		return -1;
	}
}
