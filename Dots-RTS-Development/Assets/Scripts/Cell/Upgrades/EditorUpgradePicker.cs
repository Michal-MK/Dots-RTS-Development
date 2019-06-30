using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorUpgradePicker : MonoBehaviour, IPointerDownHandler, IPointerClickHandler {

	public event EventHandler<EditorUpgradePicker> OnPickerClicked;

	public Image upgradeImg;

	public UpgradeType UpgradeType { get; set; }
	public Upgrades Upgrade { get; set; }

	public void OnPointerClick(PointerEventData eventData) {
		OnPickerClicked?.Invoke(this, this);
	}

	public void OnPointerDown(PointerEventData eventData) {
		//Necessary for some reason
	}
}
