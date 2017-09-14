using UnityEngine;
using UnityEngine.EventSystems;

public class TeamBox : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {


	public TeamSetup myParrent;
	public Vector3 initialPos;

	public Cell.enmTeam team;
	public RectTransform myRectTransform;

	Vector2 oldMousePos = Vector2.zero;

	public float myAngle;

	public void AllThingsSet() {
		//myParrent = transform.parent.GetComponent<TeamSetup>();
		myRectTransform = gameObject.GetComponent<RectTransform>();
		//panel = transform.parent.gameObject.GetComponent<RectTransform>();
		initialPos = transform.position;
	}

	public void OnDrag(PointerEventData eventData) {

		Vector2 mousePos = eventData.position;

		Vector2 mouseDelta = (mousePos - oldMousePos);
		Vector2 mouseOffset = ((Vector2)myRectTransform.position - mousePos);

		if (Vector2.Distance(myParrent.roundTable.position, mousePos + mouseOffset + mouseDelta) < (myParrent.roundTableR + 2 * myRectTransform.sizeDelta.x)) {
			myRectTransform.position = mousePos + mouseOffset + mouseDelta;
		}
		oldMousePos = mousePos;

	}

	public void OnPointerDown(PointerEventData eventData) {
		oldMousePos = eventData.position;
	}



	public void OnPointerUp(PointerEventData eventData) {
		myParrent.TeamBoxPosChange(transform.position, this);
	}

	public void OnPointerClick(PointerEventData eventData) {
		// print(eventData.clickCount);
		if (eventData.clickCount == 2) {
			myParrent.core.EnableSingleDiffInputField(this);
		}
	}
}
