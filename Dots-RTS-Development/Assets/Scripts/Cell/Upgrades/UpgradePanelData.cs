using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;
public class UpgradePanelData : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

	public Upgrade.Upgrades type;
	new public string name;
	public int count;
	public Image typeImage;
	private TextMeshProUGUI desc;

	private static UM_InGame currentCell;

	public static event Control.InstallUpgradeHandler OnUpgradeInstalled;

	private static bool isSubscribed = false;
	private static bool isListeningForSlot = true;

	private void Awake() {
		if (!isSubscribed) {
			//print("Subscribed Panel");
			UM_InGame.OnUpgradeBegin += Upgrade_Manager_OnUpgradeBegin;
			isSubscribed = true;
		}
	}

	private void OnDestroy() {
		//print("Unsubbed Panel");
		UM_InGame.OnUpgradeBegin -= Upgrade_Manager_OnUpgradeBegin;
		UM_InGame.OnUpgradeQuit -= Upgrade_Manager_OnUpgradeQuit;
		isSubscribed = false;

	}

	void Start() {
		if (SceneManager.GetActiveScene().name == Scenes.PLAYER || SceneManager.GetActiveScene().name == Scenes.DEBUG) {
			if (ProfileManager.getCurrentProfile == null) {
				Control.DebugSceneIndex = SceneManager.GetActiveScene().buildIndex;
				SceneManager.LoadScene(Scenes.PROFILES);
				return;
			}
			if (type != Upgrade.Upgrades.NONE) {
				count = ProfileManager.getCurrentProfile.acquiredUpgrades[type];
				UpdateUpgradeOverview();
			}
			desc = transform.parent.parent.Find("Description").GetComponent<TextMeshProUGUI>();
		}
	}

	private void Upgrade_Manager_OnUpgradeQuit(UM_InGame sender) {
		UM_InGame.OnUpgradeQuit -= Upgrade_Manager_OnUpgradeQuit;


		print("Upgrade Quit " + sender.gameObject.name);
		sender.slotRender.color = new Color(1, 1, 1, 0);
		foreach (BoxCollider2D col in sender.slotHolder.GetComponentsInChildren<BoxCollider2D>()) {
			col.enabled = false;
		}
		Upgrade_Manager.isUpgrading = false;
		currentCell = null;
	}

	private void Upgrade_Manager_OnUpgradeBegin(UM_InGame sender) {
		UM_InGame.OnUpgradeQuit += Upgrade_Manager_OnUpgradeQuit;

		print("Upgrade Begin " + sender.gameObject.name);
		currentCell = sender;
		currentCell.slotRender.color = new Color(1, 1, 1, 0.2f);
		foreach (BoxCollider2D col in currentCell.slotHolder.GetComponentsInChildren<BoxCollider2D>()) {
			col.enabled = true;
		}
	}

	//Updates upgrade image visuals
	public void UpdateUpgradeOverview() {
		if (count == 0) {
			GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
		}
		else {
			GetComponent<Image>().color = new Color(1, 1, 1, 1);
		}
		transform.Find("UpgradeCount").gameObject.GetComponent<TextMeshProUGUI>().text = count.ToString();
		typeImage.sprite = Upgrade.UPGRADE_GRAPHICS[type];
	}

	//Upgrade Instalation Logic
	public void OnPointerClick(PointerEventData eventData) {
		if (SceneManager.GetActiveScene().name != Scenes.PROFILES) {
			if (currentCell.HasFreeSlots() && count > 0) {

				if (eventData.clickCount == 1) {
					if (isListeningForSlot) {
						UpgradeSlot.OnSlotClicked += InstallUpgradeTo;
						isListeningForSlot = false;
					}
					currentCell.slotRender.color = new Color(1, 1, 1, 0.8f);
				}
				else if (eventData.clickCount == 2) {
					UpgradeSlot.OnSlotClicked -= InstallUpgradeTo;
					int i = currentCell.GetFirstFreeSlot();
					InstallUpgradeTo(null, i);
				}
			}
		}
	}

	//Stuff to do with instalation, moving numbers around
	private void InstallUpgradeTo(UpgradeSlot sender, int slot) {
		UpgradeSlot.OnSlotClicked -= InstallUpgradeTo;
		currentCell.InstallUpgrade(currentCell.cell,slot, type);
		count--;
		UpdateUpgradeOverview();
		if (sender == null) {
			SpriteRenderer s = currentCell.transform.Find("UpgradeSlots/" + slot).GetComponent<SpriteRenderer>();
			s.sprite = Upgrade.UPGRADE_GRAPHICS[type];
			s.size = Vector2.one * 25;
		}
		else {
			sender.selfSprite.sprite = Upgrade.UPGRADE_GRAPHICS[type];
			sender.selfSprite.size = Vector2.one * 25;
		}
		currentCell.slotRender.color = new Color(1, 1, 1, 0.25f);
		isListeningForSlot = true;
	}

	public void OnPointerEnter(PointerEventData eventData) {
		if (desc != null) {
			desc.text = FolderAccess.GetUpgradeName(type);
		}
	}

	public void OnPointerExit(PointerEventData eventData) {
		if (desc != null) {
			desc.text = "";
		}
	}
}
