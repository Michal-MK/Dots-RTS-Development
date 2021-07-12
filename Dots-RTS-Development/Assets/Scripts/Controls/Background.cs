using UnityEngine;
using UnityEngine.EventSystems;

public class Background : MonoBehaviour, IPointerEnterHandler {
	public void OnPointerEnter(PointerEventData eventData) {
		if (Input.GetMouseButton(0)) return;
		if (GameObject.Find(nameof(Selection)) != null) {
			GameObject.Find(nameof(Selection)).GetComponent<Selection>().Clear();
		}
	}
}
