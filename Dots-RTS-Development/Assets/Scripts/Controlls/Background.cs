using UnityEngine;
using UnityEngine.EventSystems;

public class Background : MonoBehaviour, IPointerEnterHandler {

	public static bool onReleaseClear = true;

	public void OnPointerEnter(PointerEventData eventData) {
		if (Input.GetMouseButton(0)) {
			onReleaseClear = true;
			GameCell.lastEnteredCell = null;
		}
	}

	private void Update() {
		if (onReleaseClear) {
			if (Input.GetMouseButtonUp(0)) {
				GameCell.ClearSelection();
			}
		}
	}
}
