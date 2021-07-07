using UnityEngine;
using UnityEngine.EventSystems;

public class Background : MonoBehaviour, IPointerEnterHandler {

	public static bool onReleaseClear = false;

	public void OnPointerEnter(PointerEventData eventData) {
		if (!Input.GetMouseButton(0)) return;
		onReleaseClear = true;
		GameCell.lastEnteredCell = null;
	}

	private void Update() {
		bool lmbState = !Input.GetMouseButtonUp(0);
		if (!onReleaseClear && lmbState) return;
		//GameCell.ClearSelection(); TODO
	}
}
