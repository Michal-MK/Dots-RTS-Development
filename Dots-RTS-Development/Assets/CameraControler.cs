using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour {
	public Camera c;
	public RectTransform background;

	private Vector3 defaultPos;
	private float defaultSize;
	private float camVertSize;
	private float camHorSize;
	private float bgVertSize;
	private float bgHorSize;
	private float camHorDiff;
	private float camVertDiff;
	private float camHorDiffL;
	private float camVertDiffB;




	void Awake() {
		Upgrade_Manager.OnUpgradeBegin += OnBeginUpgrading;
		Upgrade_Manager.OnUpgradeQuit += OnQuitUpgrading;
	}



	void Start() {
		defaultPos = transform.position;
		defaultSize = c.orthographicSize;

		camVertSize = c.orthographicSize;
		camHorSize = c.orthographicSize * c.aspect;
		bgVertSize = background.sizeDelta.y / 2;
		bgHorSize = background.sizeDelta.x / 2;
	}

	private void OnBeginUpgrading(Upgrade_Manager sender) {
		c.orthographicSize *= 0.5f;
		Vector3 newPos = sender.transform.position + (Vector3.back * 10);

		camVertDiff = (newPos.y + c.orthographicSize) - (background.position.y + bgVertSize);
		camHorDiff = (newPos.x + c.orthographicSize * c.aspect) - (background.position.x + bgHorSize);
		camHorDiffL = (newPos.x - c.orthographicSize * c.aspect) - (background.position.x - bgHorSize);
		camVertDiffB = (newPos.y - c.orthographicSize) - (background.position.y - bgVertSize);


		//RightSide
		if (camHorDiff > 0) {
			newPos = new Vector3(newPos.x - camHorDiff, newPos.y, newPos.z);
		}
		//Top
		if (camVertDiff > 0) {
			newPos = new Vector3(newPos.x, newPos.y - camVertDiff, newPos.z);
		}
		//Left
		if (camHorDiffL < 0) {
			newPos = new Vector3(newPos.x - camHorDiffL, newPos.y, newPos.z);
		}
		//Bottom
		if (camVertDiffB < 0) {
			newPos = new Vector3(newPos.x, newPos.y - camVertDiffB, newPos.z);
		}

		transform.position = newPos;
	}

	private void OnQuitUpgrading(Upgrade_Manager sender) {
		c.orthographicSize = defaultSize;
		transform.position = defaultPos;
	}
}
