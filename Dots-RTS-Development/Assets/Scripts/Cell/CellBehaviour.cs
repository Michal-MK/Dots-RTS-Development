using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CellBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	#region Prefab References

	[HideInInspector] public SpriteRenderer cellSprite;
	[HideInInspector] public CircleCollider2D col;
	[HideInInspector] public Rigidbody2D rg;

	[HideInInspector] public UpgradeManager uManager;

	[HideInInspector] public TextMeshPro elementCountDisplay;
	[HideInInspector] public MeshRenderer elementCountDisplayRenderer;

	[HideInInspector] public SpriteRenderer cellSelectedRenderer;

	#endregion

	public Cell Cell;

	//TODO Hardcoded BS Alert
	protected IEnumerator ScaleCell() {
		while (true) {
			yield return new WaitForEndOfFrame();
			float mappedValue;
			if (Cell.elementCount < 10) {
				mappedValue = 1;
			}
			else if (Cell.elementCount >= 10 && Cell.elementCount <= Cell.maxElements) {
				mappedValue = Map.MapFloat(Cell.elementCount, 10, Cell.maxElements, 1f, 2f);
			}
			else {
				if (Cell.elementCount < 1000) {
					mappedValue = Map.MapFloat(Cell.elementCount, Cell.maxElements, 999f, 2f, 4f);
				}
				else {
					mappedValue = 4;
				}
			}
			for (float f = 0; f <= 0.1f; f += 0.05f) {
				transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(mappedValue, mappedValue), f);
				yield return null;
			}
			Cell.cellRadius = col.radius * transform.localScale.x;
		}
	}

	public void UpdateVisual() {
		cellSprite.color = CellColours.GetColor(Cell.team);
		elementCountDisplay.text = Cell.elementCount.ToString();
	}

	protected void EnableCircle(Color? color = null) {
		color ??= Color.yellow;
		cellSelectedRenderer.color = (Color)color;
		cellSelectedRenderer.enabled = true;
	}

	#region MouseUp over self

	private bool mouseOver;

	private void Update() {
		if (Input.GetMouseButtonUp(0) && mouseOver) {
			OnMouseButtonUp();
		}
	}

	public virtual void OnPointerEnter(PointerEventData eventData) {
		mouseOver = true;
	}

	public virtual void OnPointerExit(PointerEventData eventData) {
		mouseOver = false;
	}

	protected virtual void OnMouseButtonUp() { }

	#endregion
}
