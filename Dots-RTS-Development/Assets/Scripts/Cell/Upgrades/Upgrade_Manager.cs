using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Upgrade_Manager : MonoBehaviour, IPointerClickHandler {

	public static bool isUpgrading = false;

	public static event Control.EnteredUpgradeMode OnUpgradeBegin;
	public static event Control.QuitUpgradeMode OnUpgradeQuit;

	public CellBehaviour cell;
	public SpriteRenderer slotRender;
	public Transform slotHolder;

	public Upgrade.Upgrades[] upgrades = new Upgrade.Upgrades[8];

	public GameObject[] slotsGO = new GameObject[8];

	[HideInInspector]
	public BoxCollider2D[] slots = new BoxCollider2D[8];

	private void Start() {
		for (int i = 0; i < slots.Length; i++) {
			slots[i] = slotHolder.GetChild(i).GetComponent<BoxCollider2D>();
		}
		//print(slots[2].gameObject.name);
	}

	/// <summary>
	/// Adds upgrades from a seav file of othetwise defined source
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

	public void OnPointerClick(PointerEventData eventData) {
		//Detects double click on cell
		if (eventData.clickCount % 2 == 0) {
			isUpgrading = true;
			OnUpgradeBegin(this);
			slotRender.color = new Color32(255, 255, 255, 30);

			/*Display menu with upgrades, zoom camer onto the cell.*/
		}
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (isUpgrading) {
				OnUpgradeQuit(this);
			}
		}
	}
}
