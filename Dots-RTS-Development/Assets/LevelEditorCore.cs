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
	
	private void Start() {
		//thisOneCamera = gameObject.GetComponent<Camera>();

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
#if (UNITY_EDITOR || UNITY_STANDALONE)
		if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject() && CanPlaceCells) {
			//Your code here
			Vector2 pos = thisOneCamera.ScreenToWorldPoint(Input.mousePosition);
			GameObject newCell = Instantiate(CellPrefab, pos, Quaternion.identity);
		}
#endif
#if UNITY_ANDROID
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && CanPlaceCells) {
			//Your code here
			Vector2 pos = thisOneCamera.ScreenToWorldPoint(Input.mousePosition);
			GameObject newCell = Instantiate(CellPrefab, pos, Quaternion.identity);
		}
#endif
	}
}
