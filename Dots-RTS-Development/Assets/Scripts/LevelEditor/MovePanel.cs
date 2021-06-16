using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System;
using TMPro;

public class MovePanel : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerClickHandler {

	public GameObject panel;
	public RectTransform CSTransform;
	public TextMeshProUGUI ShowPanelHint;
	public TextMeshProUGUI HidePanelHint;

	public float anchorDiffPercent;
	private bool isShown = true;
	private bool isMoving = false;

	#region Events
	void Start() {
		Control.RMBPressed += Control_RMBPressed;
		Control.OnPauseStateChanged += ToggleControlsPanel;
	}
	void OnDestroy() {
		Control.RMBPressed -= Control_RMBPressed;
		Control.OnPauseStateChanged -= ToggleControlsPanel;
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
		float cursorPercent = eventData.position.y / Screen.height;
		HidePanelHint.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 10);
		if (cursorPercent < 0.125f) {
			CSTransform.anchorMin = new Vector2(0, 0);
			CSTransform.anchorMax = new Vector2(1, anchorDiffPercent);


		}
		else if (cursorPercent > 0.875f) {
			CSTransform.anchorMin = new Vector2(0, 1 - anchorDiffPercent);
			CSTransform.anchorMax = new Vector2(1, 1f);
			HidePanelHint.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200);
		}
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (!isShown) {
			StartCoroutine(MoveToAnchor(anchorDiffPercent, 2));
		}
		else if (eventData.clickCount == 2) {
			StartCoroutine(MoveToAnchor(0, 2));
		}
	}

	private void ToggleControlsPanel(object sender, bool state) {
		if (state) {
			StartCoroutine(MoveToAnchor(0, 2));
		}
		else {
			StartCoroutine(MoveToAnchor(anchorDiffPercent, 2));
		}
	}

	private void Control_RMBPressed(object _, EventArgs __) {
		if (isMoving) return;
		if (!isShown) {
			StartCoroutine(MoveToAnchor(anchorDiffPercent, 2));
		}
		else {
			StartCoroutine(MoveToAnchor(0, 2));
		}
	}

	private IEnumerator MoveToAnchor(float topAnchor, float speedMultiplyer) {
		float initialTopAnchor = CSTransform.anchorMax.y;

		isMoving = true;
		for (float time = 0; time < 1f; time += Time.unscaledDeltaTime * speedMultiplyer) {
			CSTransform.anchorMin = new Vector2(0, Mathf.SmoothStep(initialTopAnchor - anchorDiffPercent, topAnchor - anchorDiffPercent, time));
			CSTransform.anchorMax = new Vector2(1, Mathf.SmoothStep(initialTopAnchor, topAnchor, time));
			yield return null;
		}
		isMoving = false;

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
