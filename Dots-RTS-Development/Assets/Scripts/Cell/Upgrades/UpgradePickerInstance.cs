using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradePickerInstance : MonoBehaviour,IPointerDownHandler,IPointerClickHandler {

	public static event EventHandler<OnPickerClickedEventArgs> OnPickerClicked;

	public Image upgradeImg;

	public UpgradeType UpgradeType { get; set; }
	public Upgrades upgrade { get; set; }

	public void OnPointerClick(PointerEventData eventData) {
		if (OnPickerClicked != null) {
			OnPickerClicked(this, new OnPickerClickedEventArgs(/*TODO*/null, /*TODO*/this));
			print("Invoked");
		}
	}

	public void OnPointerDown(PointerEventData eventData) {
		//Necessary for some reason
	}
}
