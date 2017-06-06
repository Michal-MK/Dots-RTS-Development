using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelEditorCore : MonoBehaviour {

	
	public InputField teamInput;
	public InputField maxInput;
	public InputField startInput;
	public InputField regenInput;

	public Texture2D[] cursors;
	public Camera thisOneCamera;
	public GameObject CellPrefab;

	Cell clickedCell; //Current Cell I'm changing the values of.

	bool CanPlaceCells = false;
	bool DestroyCells = false;
	bool CellSelected = false;
	
	int nextTeam;
	int nextMax;
	float nextRegen;
	int nextStarting;
	


	private void Start() {
		//thisOneCamera = gameObject.GetComponent<Camera>();
		nextMax = 50;
		nextStarting = 10;
		nextTeam = 0;
		nextRegen = 2;
	}

	public void ToggleDestoyCells() {
		DestroyCells = !DestroyCells;
		CellSelected = false;
		if (DestroyCells) {
			Cursor.SetCursor(cursors[1], new Vector2(0, 0), CursorMode.Auto);
		}
		else {
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
	}
	// Use this for toggling weather to place cells or to change cells
	public void PanelChange() {
		if (CellSelected == false) {
			if (!int.TryParse(maxInput.text, out nextMax)) {
				nextMax = 50;
			}
			if (!int.TryParse(startInput.text, out nextStarting)) {
				nextStarting = 10;
			}
			if (!int.TryParse(teamInput.text, out nextTeam)) {
				nextTeam = 0;
			}
			if (!float.TryParse(regenInput.text, out nextRegen)) {
				nextRegen = 2;
			}
		}
		else {
			int tempMax;
			int tempCount;
			int tempTeam;
			float tempRegen;
			if (int.TryParse(maxInput.text,out tempMax)) {
				clickedCell.maxElements = tempMax;
			}
			if (int.TryParse(startInput.text, out tempCount)) {
				clickedCell.elementCount = tempCount;
			}
			if (int.TryParse(teamInput.text, out tempTeam)) {
				clickedCell.cellTeam = (Cell.enmTeam)tempTeam;
			}
			if (float.TryParse(regenInput.text, out tempRegen)) {
				clickedCell.regenFrequency = tempRegen;
			}
		}
	}


	public void PlaceCellValueChange() {
		CanPlaceCells = !CanPlaceCells;
		CellSelected = false;
		if (CanPlaceCells) {
			Cursor.SetCursor(cursors[0], new Vector2(0, 0), CursorMode.Auto);
		}
		else {
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
	}
	private void Update() {

#if (UNITY_EDITOR || UNITY_STANDALONE)
		if (Input.GetMouseButtonUp(0) && Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward)) {

			RaycastHit2D recentestHitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward);

			CellSelected = true;
			if (recentestHitInfo.collider.transform.parent != null) {
				// I hit an upgrade slot ==> cancelling
				return;
			}
			else {
				clickedCell = recentestHitInfo.collider.gameObject.GetComponent<Cell>();
			}
			teamInput.text = clickedCell.cellTeam.ToString();
			maxInput.text = clickedCell.maxElements.ToString();
			startInput.text = clickedCell.elementCount.ToString();
			regenInput.text = clickedCell.regenFrequency.ToString();


		}

		if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject() && CanPlaceCells) {
			Vector2 pos = thisOneCamera.ScreenToWorldPoint(Input.mousePosition);
			GameObject newCell = Instantiate(CellPrefab, pos, Quaternion.identity);
			Cell c = newCell.GetComponent<Cell>();

			c.cellTeam = (Cell.enmTeam)nextTeam;
			c.maxElements = nextMax;
			c.regenFrequency = nextRegen;;
			c.elementCount = nextStarting;

			SavingScript.AddCell(c);
		}
#endif
#if UNITY_ANDROID
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && CanPlaceCells) {
			Vector2 pos = thisOneCamera.ScreenToWorldPoint(Input.mousePosition);
			GameObject newCell = Instantiate(CellPrefab, pos, Quaternion.identity);
			Cell c = newCell.GetComponent<Cell>();

			c.cellTeam = (Cell.enmTeam)nextTeam;
			c.maxElements = nextMax;
			c.regenFrequency = nextRegen;
			c.elementCount = nextStarting;

			SavingScript.AddCell(c);
		}
#endif
	}
}
