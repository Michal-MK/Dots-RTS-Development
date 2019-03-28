using UnityEngine;

public class Upgrade_Manager : MonoBehaviour {

	public static bool isUpgrading = false;

	public Upgrades[] upgrades = new Upgrades[8] {
		Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,
	};

	/// <summary>
	/// Adds upgrades from a save file or otherwise defined source
	/// </summary>
	public void PreinstallUpgrades(Upgrades[] upgades) {
		upgrades = upgades;
		UpgradePreinstallSprites();
	}

	protected virtual void UpgradePreinstallSprites() {

	}

	/// <summary>
	/// Installs upgrade to set slot
	/// </summary>
	public void InstallUpgrade(Cell cell, int slot, Upgrades upgrade) {
		upgrades[slot] = upgrade;
		if ((int)upgrade >= 200) {
			switch (upgrade) {
				case Upgrades.UTIL_FASTER_ELEMENT_SPEED: {
					cell.ElementSpeed += 3;
					return;
				}
				case Upgrades.UTIL_FASTER_REGENERATION: {
					cell.RegenPeriod -= 0.4f;
					return;
				}
			}
		}
	}

	/// <summary>
	/// Uninstalls upgrade by slot
	/// </summary>
	public void UninstallUpgrade(int slot) {
		upgrades[slot] = Upgrades.NONE;
	}

	/// <summary>
	/// Uninstalls upgrade by type
	/// </summary>
	public void UninstallUpgrade(Upgrades type) {
		for (int i = 0; i < upgrades.Length; i++) {
			if (upgrades[i] == type) {
				upgrades[i] = Upgrades.NONE;
			}
		}
	}
	/// <summary>
	/// Does the cell have this type of upgrade already?
	/// </summary>
	public bool HasUpgade(Upgrades type) {
		for (int i = 0; i < upgrades.Length; i++) {
			if (upgrades[i] == type) {
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Does the cell have this type of upgrade already?
	/// </summary>
	/// <param name="type"></param>
	/// <returns>Amount of upgrades of type "type" in this cell.</returns>
	public int HasUpgrade(Upgrades type) {
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
			if (upgrades[i] == Upgrades.NONE) {
				return true;
			}
		}
		return false;
	}

	public int GetFirstFreeSlot() {
		for (int i = 0; i < upgrades.Length; i++) {
			if (upgrades[i] == Upgrades.NONE) {
				return i;
			}
		}
		return -1;
	}
}
