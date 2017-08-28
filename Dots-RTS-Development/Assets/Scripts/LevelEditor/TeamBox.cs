using UnityEngine;
using UnityEngine.EventSystems;

public class TeamBox : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {

	// Use this for initialization
	public TeamSetup myParrent;
	public Vector3 initialPos;

	public int team;
	RectTransform rect;
	public RectTransform panel;

	Vector2 oldMousePos = Vector2.zero;

	public float r;
	public static GameObject dragged;

	public float myAngle;

	public void AllThingsSet() {
		//myParrent = transform.parent.GetComponent<TeamSetup>();
		rect = gameObject.GetComponent<RectTransform>();
		//panel = transform.parent.gameObject.GetComponent<RectTransform>();
		initialPos = transform.position;
	}

	public void OnDrag(PointerEventData eventData) {

		Vector2 mousePos = eventData.position;

		Vector2 mouseDelta = (mousePos - oldMousePos);
		Vector2 mouseOffset = ((Vector2)rect.position - mousePos);

		if (Vector2.Distance(panel.position, mousePos + mouseOffset + mouseDelta) < (r + rect.sizeDelta.x)) {
			rect.position = mousePos + mouseOffset + mouseDelta;
		}
		oldMousePos = mousePos;

	}

	public void OnPointerDown(PointerEventData eventData) {
		oldMousePos = eventData.position;
	}
	void PrepareDiffInputBox() {

		LevelEditorCore.aiDifficultySingleInput.gameObject.SetActive(true);
		LevelEditorCore.aiDifficultySingleInput.transform.position = (Vector2)transform.position + new Vector2(0, rect.sizeDelta.y / 2);
		float value;
		if (LevelEditorCore.aiDifficultyDict.TryGetValue(team, out value)) {
			LevelEditorCore.aiDifficultySingleInput.text = value.ToString();
		}
		else {
			LevelEditorCore.aiDifficultySingleInput.text = "";
		}
		SingleDiffIF singleDiffIf = LevelEditorCore.aiDifficultySingleInput.gameObject.GetComponent<SingleDiffIF>();
		singleDiffIf.OnMove(team);
		singleDiffIf.core = Camera.main.GetComponent<LevelEditorCore>();

	}


	public void OnPointerUp(PointerEventData eventData) {
		myParrent.TeamBoxPosChange(eventData.pointerPress.transform.position, eventData.pointerPress.GetComponent<TeamBox>());
	}

	public void OnPointerClick(PointerEventData eventData) {
		// print(eventData.clickCount);
		if (eventData.clickCount == 2) {
			PrepareDiffInputBox();
		}
	}
}
