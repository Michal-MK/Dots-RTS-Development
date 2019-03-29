using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UM_InGame : Upgrade_Manager, IPointerClickHandler {

	private static UM_InGame currentCell;

	public event EventHandler<UM_InGame> OnUpgradeBegin;
	public event EventHandler<UM_InGame> OnUpgradeQuit;

	public GameCell cell;
	public SpriteRenderer slotRender;
	public Transform slotHolder;

	public GameObject[] slotsGO = new GameObject[8];

	[HideInInspector]
	public BoxCollider2D[] slots = new BoxCollider2D[8];

	private void Start() {
		for (int i = 0; i < slots.Length; i++) {
			slots[i] = slotHolder.GetChild(i).GetComponent<BoxCollider2D>();

		}
	}

	protected override void UpgradePreinstallSprites() {
		for (int i = 0; i < slotsGO.Length; i++) {
			if (upgrades[i] != Upgrades.NONE) {
				slotsGO[i].GetComponent<SpriteRenderer>().sprite = Upgrade.UPGRADE_GRAPHICS[upgrades[i]];
				slotsGO[i].GetComponent<SpriteRenderer>().size = Vector2.one * 25f;
			}
		}
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.clickCount == 2) {
			if (OnUpgradeBegin != null) {
				if (currentCell != null) {
					OnUpgradeQuit(this, currentCell);
				}
				isUpgrading = true;
				currentCell = this;
				OnUpgradeBegin(this, currentCell); //Sent to Camera,UpgradePanelData 
			}
		}
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (isUpgrading && currentCell != null) {
				isUpgrading = false;
				OnUpgradeQuit(this, currentCell);
				currentCell = null;
			}
		}
	}
}
