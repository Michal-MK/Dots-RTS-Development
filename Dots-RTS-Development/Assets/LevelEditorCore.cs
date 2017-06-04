using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelEditorCore : MonoBehaviour {
	public Toggle placecellsToggle;
	public Texture2D[] cursors;
	public Camera thisOneCamera;
	public GameObject CellPrefab;

	public bool CanPlaceCells = false;
	public bool OverUi;
	
	private void Start() {
		//thisOneCamera = gameObject.GetComponent<Camera>();

	}
	public void HoveringOverUiEnter() {
		OverUi = true;
	}
	public void HoveringOverUiExit() {
		OverUi = false;
	}
	// Use this for toggling weather to place cells or to change cells
	public void PlaceCellValueChange() {
		CanPlaceCells = !CanPlaceCells;
		if (placecellsToggle.isOn) {
			Cursor.SetCursor(cursors[0], new Vector2(0, 0), CursorMode.Auto);
		}
		else {
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
	}
	private void Update() {
		if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject() && CanPlaceCells) {
			//Your code here
			Vector2 pos = thisOneCamera.ScreenToWorldPoint(Input.mousePosition);
			GameObject newCell = Instantiate(CellPrefab, pos, Quaternion.identity);
		}
	}

	void OnMouseDown() {
		if (Input.GetMouseButtonDown(0)) {
			if (CanPlaceCells && !OverUi) {
				Vector2 pos = thisOneCamera.ScreenToWorldPoint(Input.mousePosition);
				GameObject newCell = Instantiate(CellPrefab, pos, Quaternion.identity);
			}
		}
	}
}
