using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Upgrade_Manager : MonoBehaviour, IPointerClickHandler {

	public static bool isUpgrading = false;
	private static Upgrade_Manager currentCell;
	/// <summary>
	/// Selected item in the upgrade shop.
	/// </summary>
	public static int selectedUpgrade = -1;

	public static event Control.EnteredUpgradeMode OnUpgradeBegin;
	public static event Control.QuitUpgradeMode OnUpgradeQuit;

	public CellBehaviour cell;
	public SpriteRenderer slotRender;
	public Transform slotHolder;

	public Upgrade.Upgrades[] upgrades = new Upgrade.Upgrades[8] {
		Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,
	};

	public GameObject[] slotsGO = new GameObject[8];

	[HideInInspector]
	public BoxCollider2D[] slots = new BoxCollider2D[8];

	private void Start() {
		if (SceneManager.GetActiveScene().name != Scenes.SHOP) {
			for (int i = 0; i < slots.Length; i++) {
				slots[i] = slotHolder.GetChild(i).GetComponent<BoxCollider2D>();

			}
		}
	}
	/// <summary>
	/// Buy selected upgrade from the store
	/// </summary>
	public void BuyUpgrade() {
		GameObject upgrade = GameObject.Find("Upgrade" + selectedUpgrade);
		Animator anim = GameObject.Find("Warning").GetComponent<Animator>();
		//Preform some highlights

		//Subtract total money + add the upgrade to proflie
		int cost = Upgrade.GetCost((Upgrade.Upgrades)selectedUpgrade);

		foreach (KeyValuePair<Upgrade.Upgrades, int> col in ProfileManager.getCurrentProfile.acquiredUpgrades) {
			if (col.Key == (Upgrade.Upgrades)selectedUpgrade) {
				if (cost <= ProfileManager.getCurrentProfile.ownedCoins) {
					ProfileManager.getCurrentProfile.ownedCoins -= cost;
					ProfileManager.getCurrentProfile.acquiredUpgrades[col.Key] += 1;
					ProfileManager.SerializeChanges();
					UI_ReferenceHolder.U_profileMoney.text = ProfileManager.getCurrentProfile.ownedCoins + " coins";
					return;
				}
				else {
					anim.GetComponent<TextMeshProUGUI>().text = "You are missing\n" + (cost - ProfileManager.getCurrentProfile.ownedCoins) + " coins.";
					anim.Play("Show");
				}
			}
		}

	}

	/// <summary>
	/// Adds upgrades from a save file or otherwise defined source
	/// </summary>
	public Upgrade.Upgrades[] PreinstallUpgrades {
		get { return upgrades; }
		set { upgrades = value; }

	}

	/// <summary>
	/// Installs upgrade to set slot
	/// </summary>
	public void InstallUpgrade(int slot, Upgrade.Upgrades upgrade) {
		upgrades[slot] = upgrade;
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

	public void OnPointerClick(PointerEventData eventData) {
		//print("Click " + gameObject.name);
		//Detects double click on cell
		if (eventData.clickCount == 2) {
			if (OnUpgradeBegin != null) {
				if(currentCell != null) {
					OnUpgradeQuit(currentCell);
				}
				isUpgrading = true;
				currentCell = this;
				OnUpgradeBegin(currentCell); //Sent to Camera,UpgradePanelData 
			}
		}
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != Scenes.SHOP) {
			if (isUpgrading && currentCell != null) {
				isUpgrading = false;
				OnUpgradeQuit(currentCell);
				currentCell = null;
			}
		}
	}
}
