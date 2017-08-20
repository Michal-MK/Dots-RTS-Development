using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControler : MonoBehaviour {
	public Camera c;
	public RectTransform background;

	private Vector3 defaultPos;
	private float defaultSize;

	private float bgVertSize;
	private float bgHorSize;
	private float camHorDiff;
	private float camVertDiff;
	private float camHorDiffL;
	private float camVertDiffB;




	private void Awake() {
		Upgrade_Manager.OnUpgradeBegin += OnBeginUpgrading;
		Upgrade_Manager.OnUpgradeQuit += OnQuitUpgrading;
	}
	private void OnDisable() {
		Upgrade_Manager.OnUpgradeBegin -= OnBeginUpgrading;
		Upgrade_Manager.OnUpgradeQuit -= OnQuitUpgrading;
	}


	void Start() {
		defaultPos = transform.position;
		defaultSize = c.orthographicSize;
	}

	private void OnBeginUpgrading(Upgrade_Manager sender) {
		c = Camera.main;

		c.orthographicSize = defaultSize;

		c.orthographicSize *= 0.5f;

		Vector3 newPos = sender.transform.position + (Vector3.back * 10);

		SpriteRenderer bg = GameObject.Find("Background").GetComponent<SpriteRenderer>();

		camVertDiff = (newPos.y + c.orthographicSize) - (bg.transform.position.y + bg.size.y / 2);
		//print("Diff to TOP " + camVertDiff);

		camHorDiff = (newPos.x + c.orthographicSize * c.aspect) - (bg.transform.position.x + bg.size.x / 2);
		//print("Diff to RIGHT " + camHorDiff);

		camHorDiffL = (newPos.x - c.orthographicSize * c.aspect) - (bg.transform.position.x - bg.size.x / 2);
		//print("Diff to LEFT " + camHorDiffL);

		camVertDiffB = (newPos.y - c.orthographicSize) - (bg.transform.position.y - bg.size.y / 2);
		//print("Diff to BOTTOM " + camVertDiffB);





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

		Vector3 percentage = c.WorldToViewportPoint(newPos);
		print(percentage.y);

		if (percentage.y <= 0.1f) {
			print("A");
			Vector3 pointU = c.ViewportToWorldPoint(new Vector3(0.5f, 0.3f));
			Vector3 pointD = c.ViewportToWorldPoint(new Vector3(0.5f, 0));

			float d = Vector3.Distance(pointU, pointD);

			transform.position = new Vector3(newPos.x, newPos.y - d, newPos.z);
		}
		else if (percentage.y > 0.96f) {
			print("B");
			transform.position = new Vector3(newPos.x, newPos.y, newPos.z);

		}
		else {
			print("C");
			Vector3 pointU = c.ViewportToWorldPoint(new Vector3(0.5f, 0.65f));
			float d = Vector3.Distance(c.transform.position, pointU);
			transform.position = new Vector3(newPos.x, newPos.y - d, newPos.z);
		}
	}

	private void OnQuitUpgrading(Upgrade_Manager sender) {
		c.orthographicSize = defaultSize;
		transform.position = defaultPos;
	}
}
