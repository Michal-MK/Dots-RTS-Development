using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System;
using TMPro;

public class MovePanel : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler,IPointerClickHandler {

	public GameObject panel;
	public RectTransform CSTransform;
	public TextMeshProUGUI ShowPanelHint;
	public TextMeshProUGUI HidePanelHint;

	public float anchorDiffPercent;
	private bool isShown = true;

	#region Events
	void Start() {
		Control.RMBPressed += Control_RMBPressed;
	}
	void OnDestroy() {
		Control.RMBPressed -= Control_RMBPressed;
	}
	#endregion

	public void OnBeginDrag(PointerEventData eventData) {
		ShowPanelHint.enabled = false;
	}

	public void OnDrag(PointerEventData eventData) {
		float cursorPerCent = eventData.position.y / Screen.height;
		CSTransform.anchorMin = new Vector2(0, cursorPerCent - (anchorDiffPercent * 0.5f));
		CSTransform.anchorMax = new Vector2(1, cursorPerCent + (anchorDiffPercent * 0.5f));
	}

	public void OnEndDrag(PointerEventData eventData) {
		float cursorPerCent = eventData.position.y / Screen.height;
		HidePanelHint.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 10);
		if (cursorPerCent < 0.125f) {
			CSTransform.anchorMin = new Vector2(0, 0);
			CSTransform.anchorMax = new Vector2(1, anchorDiffPercent);
			

		}
		else if (cursorPerCent > 0.875f) {
			CSTransform.anchorMin = new Vector2(0, 1 - anchorDiffPercent);
			CSTransform.anchorMax = new Vector2(1, 1f);
			HidePanelHint.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200);
		}

	}

	public void OnPointerClick(PointerEventData eventData) {
		if (!isShown) {
			StartCoroutine(MoveToAnchor(anchorDiffPercent));
		}
		else if (eventData.clickCount == 2) {
			StartCoroutine(MoveToAnchor(0));

		}
	}
	private void Control_RMBPressed(Vector2 position) {
		//print("Pressed " + isShown);
		if (!isShown) {
			StartCoroutine(MoveToAnchor(anchorDiffPercent));
		}
		else {
			StartCoroutine(MoveToAnchor(0));

		}
	}
	IEnumerator MoveToAnchor(float topAnchor) {
		float initialTopAnchor = CSTransform.anchorMax.y;

		print("MOVING");
		 
		for (float time = 0; time < 1f; time += Time.fixedDeltaTime) {
			
			CSTransform.anchorMin = new Vector2(0, Mathf.SmoothStep(initialTopAnchor - anchorDiffPercent, topAnchor - anchorDiffPercent, time));
			CSTransform.anchorMax = new Vector2(1, Mathf.SmoothStep(initialTopAnchor, topAnchor, time));
			yield return null;
		}

#if !UNITY_ANDROID && !UNITY_IOS
		
#endif
		
		if (topAnchor == anchorDiffPercent) {
			isShown = true;
			HidePanelHint.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 10);
			HidePanelHint.enabled = true;
			ShowPanelHint.enabled = false;
		}
		else {
			isShown = false;
			ShowPanelHint.enabled = true;
			HidePanelHint.enabled = false;
		}
	}


}
