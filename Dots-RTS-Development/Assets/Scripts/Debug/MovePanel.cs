using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class MovePanel : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerClickHandler {
	public GameObject panel;
	public Animator anim;
	public RectTransform CSTransform;

	public float anchorDiffPercent;
	private bool moving = true;
	private bool isShown = true;

	private float second = 0f;

	#region Events
	void Start() {
		Control.RMBPressed += Control_RMBPressed;
	}
	void OnDestroy() {
		Control.RMBPressed -= Control_RMBPressed;
	}
	#endregion

	public void OnDrag(PointerEventData eventData) {
		float cursorPerCent = eventData.position.y / Screen.height;
		CSTransform.anchorMin = new Vector2(0, cursorPerCent - (anchorDiffPercent * 0.5f));
		CSTransform.anchorMax = new Vector2(1, cursorPerCent + (anchorDiffPercent * 0.5f));
	}

	public void OnEndDrag(PointerEventData eventData) {
		float cursorPerCent = eventData.position.y / Screen.height;

		if (cursorPerCent < 0.125f) {
			CSTransform.anchorMin = new Vector2(0, 0);
			CSTransform.anchorMax = new Vector2(1, anchorDiffPercent);
		}
		else if (cursorPerCent > 0.875f) {
			CSTransform.anchorMin = new Vector2(0, 1 - anchorDiffPercent);
			CSTransform.anchorMax = new Vector2(1, 1f);
		}

	}

	public void OnPointerClick(PointerEventData eventData) {
		//print("WHAAAT");
		if (!isShown) {
			anim.SetTrigger("Show");
			isShown = true;
		}
		else if (eventData.clickCount == 2) {
			moving = true;
			second = 1 - CSTransform.anchorMax.y;
			StartCoroutine(HideEnum(0.25f));

		}
	}
	private void Control_RMBPressed(Vector2 position) {
		//print("Pressed " + isShown);
		if (!isShown) {
			anim.SetTrigger("Show");
			isShown = true;
		}
		else {
			moving = true;
			second = 1 - CSTransform.anchorMax.y;
			StartCoroutine(HideEnum(0.25f));

		}
	}
	IEnumerator HideEnum(float TopAnchor) {
		float initialTopAnchor = 1.25f;
		//print("MOVING");
		while (moving) {
			yield return new WaitForEndOfFrame();
			CSTransform.anchorMin = new Vector2(0, Mathf.Lerp(initialTopAnchor - anchorDiffPercent, TopAnchor - anchorDiffPercent, second + anchorDiffPercent));
			CSTransform.anchorMax = new Vector2(1, Mathf.Lerp(initialTopAnchor, TopAnchor, second + anchorDiffPercent));
			if (second + anchorDiffPercent >= 1) {

				CSTransform.anchorMin = new Vector2(0, TopAnchor - anchorDiffPercent);
				CSTransform.anchorMax = new Vector2(1, TopAnchor);
				moving = false;
			}
			second += Time.deltaTime;
		}
#if !UNITY_ANDROID && !UNITY_IOS
		anim.SetTrigger("Hide");
#endif

		isShown = false;
	}
}
