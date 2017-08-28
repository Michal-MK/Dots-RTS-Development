using UnityEngine;

public class CellBehaviourEditor : Cell {
	public bool isBeingEdited;
	public bool showMaxSize;

	public SpriteRenderer maxSize;
	public SpriteRenderer organele;


/*
	private IEnumerator Start() {
		yield return new WaitForEndOfFrame();
		print(UI_ReferenceHolder.maxSizeSlider.gameObject.name);
		UI_ReferenceHolder.maxSizeSlider.onValueChanged.AddListener(delegate { SliderValueChange(); });

		if (showMaxSize) {
			Color c = new Color(1, 1, 1, 0.5f);
			maxSize.color = c;
			organele.color = c;
		}
	}

	private void SliderValueChange() {
		float f = Map.MapFloat(UI_ReferenceHolder.maxSizeSlider.value, 0, 1, 1, 4);
		if (showMaxSize) {
			maxSize.transform.localScale = new Vector3(f, f);
		}
	}
	*/
}