using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;
public class UpgradePanelData : MonoBehaviour, IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler {

	public Upgrade.Upgrades type;
	new public string name;
	public int count;
	public Image typeImage;
	private TextMeshProUGUI desc;

	private Upgrade_Manager currentCell;

	private static bool isSubscribed = false;

	private void Awake() {
		if (!isSubscribed) {
			//print("Subscribed Panel");
			Upgrade_Manager.OnUpgradeBegin += Upgrade_Manager_OnUpgradeBegin;
			isSubscribed = true;
		}
	}

	private void OnDestroy() {
		//print("Unsubbed Panel");
		Upgrade_Manager.OnUpgradeBegin -= Upgrade_Manager_OnUpgradeBegin;
		Upgrade_Manager.OnUpgradeQuit -= Upgrade_Manager_OnUpgradeQuit;
		isSubscribed = false;

	}

	private void Upgrade_Manager_OnUpgradeQuit(Upgrade_Manager sender) {
		Upgrade_Manager.OnUpgradeQuit -= Upgrade_Manager_OnUpgradeQuit;


		//print("Upgrade Quit " + sender.gameObject.name);
		sender.slotRender.color = new Color(1, 1, 1, 0);
		foreach (BoxCollider2D col in sender.slotHolder.GetComponentsInChildren<BoxCollider2D>()) {
			col.enabled = false;
		}
		Upgrade_Manager.isUpgrading = false;
		currentCell = null;
	}

	private void Upgrade_Manager_OnUpgradeBegin(Upgrade_Manager sender) {
		Upgrade_Manager.OnUpgradeQuit += Upgrade_Manager_OnUpgradeQuit;

		//print("Upgrade Begin " + sender.gameObject.name);
		currentCell = sender;
		currentCell.slotRender.color = new Color(1, 1, 1, 0.2f);
		foreach (BoxCollider2D col in currentCell.slotHolder.GetComponentsInChildren<BoxCollider2D>()) {
			col.enabled = true;
		}
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
			if (eventData.clickCount == 1) {
				UpgradeSlot.OnSlotClicked += InstallUpgradeTo;
				currentCell.slotRender.color = new Color(1, 1, 1, 0.8f);
			}
			else if (eventData.clickCount == 2) {

				if (currentCell.HasFreeSlots()) {
					int i = currentCell.GetFirstFreeSlot();
					if (i != -1) {
						if (count > 0) {
							InstallUpgradeTo(null, i);
						}
					}
					else {
						Debug.Log("No Free Slots Exist.");
					}
				}
			}
		}
	}

	//Stuff to do with instalation, moving numbers around
	private void InstallUpgradeTo(UpgradeSlot sender, int slot) {
		print("Triggered");
		currentCell.upgrades[slot] = type;
		count--;
		UpdateUpgradeOverview();
		UpgradeSlot.OnSlotClicked -= InstallUpgradeTo;
		if (sender == null) {
			SpriteRenderer s = currentCell.transform.Find("UpgradeSlots/" + slot).GetComponent<SpriteRenderer>();
			s.sprite = Upgrade.UPGRADE_GRAPHICS[type];
			s.size = Vector2.one * 25;
		}
		else {
			sender.selfSprite.sprite = Upgrade.UPGRADE_GRAPHICS[type];
			sender.selfSprite.size = Vector2.one * 25;
		}
		currentCell.slotRender.color = new Color(1, 1, 1, 0);
		foreach (BoxCollider2D col in currentCell.slotHolder.GetComponentsInChildren<BoxCollider2D>()) {
			col.enabled = false;
		}
	}

	public void OnPointerEnter(PointerEventData eventData) {
		desc.text = FolderAccess.GetUpgradeName(type);
	}

	public void OnPointerExit(PointerEventData eventData) {
		desc.text = "";
	}
}
