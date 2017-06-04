using System.Collections;
using System;
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

	int nextTeam;
	int nextMax;
	int nextRegen;
	int nextStarting;
	public InputField teamInput;
	public InputField maxInput;
	public InputField startInput;
	public InputField regenInput;


	private void Start() {
		//thisOneCamera = gameObject.GetComponent<Camera>();

	}
	// Use this for toggling weather to place cells or to change cells
	public void PanelChange() {
		if (!int.TryParse(maxInput.text, out nextMax)) {
			nextMax = 50;
		}
		if (!int.TryParse(regenInput.text, out nextRegen)) {
			nextRegen = 2;
		}
		if (!int.TryParse(startInput.text, out nextStarting)) {
			nextStarting = 10;
		}
		if (!int.TryParse(teamInput.text, out nextTeam)) {
			nextTeam = 0;
		}
	}

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
			Vector2 pos = thisOneCamera.ScreenToWorldPoint(Input.mousePosition);
			GameObject newCell = Instantiate(CellPrefab, pos, Quaternion.identity);
			CellScript newcellscript = newCell.GetComponent<CellScript>();

			newcellscript.AlterTeam((CellScript.enmTeam)nextTeam);
			newcellscript.AlterCellMax(nextMax);
			newcellscript.AlterRegen(nextRegen);
			newcellscript.AlterStartingElementCount(nextStarting);
		}
#endif
#if UNITY_ANDROID
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && CanPlaceCells) {
			Vector2 pos = thisOneCamera.ScreenToWorldPoint(Input.mousePosition);
			GameObject newCell = Instantiate(CellPrefab, pos, Quaternion.identity);
			CellScript newcellscript = newCell.GetComponent<CellScript>();

			newcellscript.AlterTeam((CellScript.enmTeam)nextTeam);
			newcellscript.AlterCellMax(nextMax);
			newcellscript.AlterRegen(nextRegen);
			newcellscript.AlterStartingElementCount(nextStarting);
		}
#endif
	}
}
