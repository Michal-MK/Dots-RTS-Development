using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditCell : MonoBehaviour {

	public LevelEditorCore core;

	public Upgrade_Manager um;

	public static event GameControll.EnteredCellEditMode EditModeChanged;

	private Camera _cam;
	private Vector3 _oldCamPos;

	public bool isEditing = false;

	private void Awake() {
		_cam = Camera.main;
		core = GameObject.Find("Main Camera").GetComponent<LevelEditorCore>();
	}


	private void OnMouseOver() {
		if (Input.GetMouseButtonDown(1) && isEditing == false) {
			isEditing = true;
			EditModeChanged(this);
			_oldCamPos = _cam.transform.position;
			_cam.transform.position = gameObject.transform.position + new Vector3 (0,0,-10);
			_cam.orthographicSize = _cam.orthographicSize * 0.25f;
			Cell c = gameObject.GetComponent<Cell>();

			core.teamInput.text = c._team.ToString();
			core.startInput.text = c.elementCount.ToString();
			core.regenInput.text = c.regenPeriod.ToString();
			core.maxInput.text = c.maxElements.ToString();

			
		}
	}

	private void Update() {
		if (isEditing) {
			if (Input.GetKeyDown(KeyCode.Escape) && isEditing == true) {
				_cam.orthographicSize = _cam.orthographicSize * 4;
				_cam.transform.position = _oldCamPos;
				isEditing = false;
				EditModeChanged(this);
			}
		}
	}

}
