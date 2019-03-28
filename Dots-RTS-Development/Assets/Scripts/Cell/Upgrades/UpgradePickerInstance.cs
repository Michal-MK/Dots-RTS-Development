using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradePickerInstance : MonoBehaviour,IPointerDownHandler,IPointerClickHandler {

	public delegate void ClickedOnPicker(UpgradeSlot selected, UpgradePickerInstance sender);
	public static event ClickedOnPicker OnPickerClicked;

	public Image upgradeImg;

	private Upgrades _upgrade;
	private Upgrade.UpgradeType _upgradeType;

	public Upgrade.UpgradeType upgradeType {
		get { return _upgradeType; }
		set { _upgradeType = value; }
	}

	public Upgrades upgrade {
		get { return _upgrade; }
		set { _upgrade = value; }
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (OnPickerClicked != null) {
			OnPickerClicked(UpgradeSlot.HighlightedSlot, this);
			print("Invoked");
		}
	}

	public void OnPointerDown(PointerEventData eventData) {
		//Necessary for some reason
	}
}
