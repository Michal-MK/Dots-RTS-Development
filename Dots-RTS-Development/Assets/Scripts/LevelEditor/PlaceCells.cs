using UnityEngine;
using UnityEngine.EventSystems;

public class PlaceCells : MonoBehaviour {

	public GameObject CellPrefab;

	private void Update() {

#if (UNITY_EDITOR || UNITY_STANDALONE)
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && LevelEditorCore.editorMode == LevelEditorCore.Mode.PlaceCells) {
			Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			GameObject newCell = Instantiate(CellPrefab, pos, Quaternion.identity);
			Cell c = newCell.GetComponent<Cell>();
			
			c.WorkingAwakeHandler();
			//print(LevelEditorCore.start);
			newCell.SetActive(true);
			newCell.GetComponent<EditCell>().FastResize();

			c.cellTeam = (Cell.enmTeam)LevelEditorCore.team;
			c.maxElements = LevelEditorCore.max;
			c.regenPeriod = LevelEditorCore.regen;
			c.elementCount = LevelEditorCore.start;

			LevelEditorCore.AddCell(c);
			
		}
#endif
#if (UNITY_ANDROID || UNITY_IOS)
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && LevelEditorCore.editorMode == LevelEditorCore.Mode.PlaceCells) {
			Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			GameObject newCell = Instantiate(CellPrefab, pos, Quaternion.identity);
			Cell c = newCell.GetComponent<Cell>();

			c.cellTeam = (Cell.enmTeam)LevelEditorCore.team;
			c.maxElements = LevelEditorCore.max;
			c.regenPeriod = LevelEditorCore.regen;
			c.elementCount = LevelEditorCore.start;

			newCell.SetActive(true);

			LevelEditorCore.AddCell(c);
		}
#endif
	}
}
