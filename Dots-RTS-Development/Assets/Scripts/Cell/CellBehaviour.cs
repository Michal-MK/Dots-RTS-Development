using System.Collections;
using TMPro;
using UnityEngine;

public class CellBehaviour : MonoBehaviour {

	#region Prefab References
	[HideInInspector]
	public SpriteRenderer cellSprite;
	[HideInInspector]
	public CircleCollider2D col;
	[HideInInspector]
	public Rigidbody2D rg;

	[HideInInspector]
	public Upgrade_Manager uManager;

	[HideInInspector]
	public TextMeshPro elementCountDisplay;
	[HideInInspector]
	public MeshRenderer elementCountDisplayRenderer;

	[HideInInspector]
	public SpriteRenderer cellSelectedRenderer;
	#endregion

	public Cell Cell;

	//TODO Hardcoded BS Alert
	public IEnumerator ScaleCell() {
		while (true) {
			yield return new WaitForEndOfFrame();
			float mappedValue;
			if (Cell.ElementCount < 10) {
				mappedValue = 1;
			}
			else if (Cell.ElementCount >= 10 && Cell.ElementCount <= Cell.MaxElements) {
				mappedValue = Map.MapFloat(Cell.ElementCount, 10, Cell.MaxElements, 1f, 2f);
			}
			else {
				if (Cell.ElementCount < 1000) {
					mappedValue = Map.MapFloat(Cell.ElementCount, Cell.MaxElements, 999f, 2f, 4f);
				}
				else {
					mappedValue = 4;
				}
			}
			for (float f = 0; f <= 0.1f; f += 0.05f) {
				transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(mappedValue, mappedValue), f);
				yield return null;
			}
			Cell.CellRadius = col.radius * transform.localScale.x;
		}
	}

	public void UpdateVisual() {
		cellSprite.color = CellColours.GetColor(Cell.Team);
		elementCountDisplay.text = Cell.ElementCount.ToString();
	}

	public void EnableCircle(Color? color = null) {
		if (color == null) {
			color = Color.yellow;
		}
		cellSelectedRenderer.color = (Color)color;
		cellSelectedRenderer.enabled = true;
	}
}
