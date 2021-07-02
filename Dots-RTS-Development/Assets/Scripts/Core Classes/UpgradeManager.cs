using System.Linq;
using UnityEngine;

public class UpgradeManager : MonoBehaviour {

	public static bool isUpgrading = false;

	[SerializeField] private Upgrades[] upgrades = {
		Upgrades.NONE, Upgrades.NONE, Upgrades.NONE, Upgrades.NONE, Upgrades.NONE, Upgrades.NONE, Upgrades.NONE, Upgrades.NONE,
	};

	public Upgrades[] InstalledUpgrades => upgrades;

	/// <summary>
	/// Adds upgrades from a save file or otherwise defined source
	/// </summary>
	public void PreinstallUpgrades(Upgrades[] preinstallUpgrades) {
		upgrades = preinstallUpgrades;
		UpgradePreinstallSprites();
	}

	protected virtual void UpgradePreinstallSprites() { }

	/// <summary>
	/// Installs upgrade to set slot
	/// </summary>
	public void InstallUpgrade(Cell cell, int slot, Upgrades upgrade) {
		upgrades[slot] = upgrade;
		if ((int)upgrade >= 200) {
			switch (upgrade) {
				case Upgrades.UTIL_FASTER_ELEMENT_SPEED: {
					cell.elementSpeed += 3;
					return;
				}
				case Upgrades.UTIL_FASTER_REGENERATION: {
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
		upgrades[slot] = Upgrades.NONE;

		//TODO revert modifications
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
	public bool HasUpgrade(Upgrades type) {
		return upgrades.Any(t => t == type);
	}

	/// <summary>
	/// Does the cell have this type of upgrade already?
	/// </summary>
	/// <param name="type"></param>
	/// <returns>Amount of upgrades of type "type" in this cell.</returns>
	public int GetUpgradeCount(Upgrades type) {
		return upgrades.Count(t => t == type);
	}

	/// <summary>
	/// Can player install additional upgrades?
	/// </summary>
	public bool HasFreeSlots() {
		return upgrades.Any(t => t == Upgrades.NONE);
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
