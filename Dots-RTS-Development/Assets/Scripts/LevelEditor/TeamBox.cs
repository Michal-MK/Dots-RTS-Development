using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class TeamBox : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {

	public TeamSetup myParent;
	public Vector3 initialPos;

	public Team team;
	public RectTransform myRectTransform;

	private Vector2 oldMousePos = Vector2.zero;

	public float myAngle;

	public void AllThingsSet() {
		myRectTransform = gameObject.GetComponent<RectTransform>();
		//panel = transform.parent.gameObject.GetComponent<RectTransform>();
		initialPos = transform.position;
	}

	public void OnDrag(PointerEventData eventData) {

		Vector2 mousePos = eventData.position;

		Vector2 mouseDelta = (mousePos - oldMousePos);
		Vector2 mouseOffset = ((Vector2)myRectTransform.position - mousePos);

		if (Vector2.Distance(myParent.roundTable.position, mousePos + mouseOffset + mouseDelta) < (myParent.roundTableR + 2 * myRectTransform.sizeDelta.x)) {
			myRectTransform.position = mousePos + mouseOffset + mouseDelta;
		}
		oldMousePos = mousePos;

	}

	public void OnPointerDown(PointerEventData eventData) {
		oldMousePos = eventData.position;
	}



	public void OnPointerUp(PointerEventData eventData) {
		myParent.TeamBoxPosChange(transform.position, this);
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.clickCount == 2) {
			myParent.core.EnableSingleDiffInputField(this);
		}
	}
}
