using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellScript : MonoBehaviour {

	private int count;
	private float regenSpeed;
	private int maxCount;
	private Vector2 position;

	public bool isSelected = false;

	public TextMesh text;
	public GameObject cellObj;

	//Call to set cell attributes
	public void SetCellData(Vector2 position, int startingCount = 0, int maximum = 100, float regenerationRate = 2f) {
		gameObject.transform.position = position;
		count = startingCount;
		maxCount = maximum;
		regenSpeed = regenerationRate;
		StartCoroutine(Generate());
	}

	private void Start() {
		text.text = count.ToString();
		text.gameObject.GetComponent<MeshRenderer>().sortingOrder = 2;
	}

	//Call to alter cell regeneration rate
	public void AlterRegen(float newRegenSpeed) {
		if (regenSpeed == newRegenSpeed) {
			return;
		}
		else {
			StopCoroutine(Generate());
			regenSpeed = newRegenSpeed;
			StartCoroutine(Generate());
		}
	}
	//Call to alter cell size
	public void AlterCellMax(int newMaximum) {

	}

	//Keeps generateing new elements for the cell
	public IEnumerator Generate() {
		while (true) {
			yield return new WaitForSecondsRealtime(regenSpeed);
			if (count < maxCount) {
				count++;
				text.text = count.ToString();
			}
		}
	}

	public void SetSelected() {
		if (isSelected) {
			isSelected = false;
		}
		else {
			isSelected = true;
			text.color = new Color32(255, 0, 0, 255);
		}
	}

	//Changes colour when howered over
	private void OnMouseOver() {
		if (!isSelected) {
			text.color = new Color32(255, 0, 0, 255);
		}
		else {
			text.color = new Color32(0, 255, 255, 255);
		}
		#region Cell Debug - Change regen speed and max size by hovering over the cell and pressing "8" to increase max count or "6" to increase regenSpeed, opposite buttons decrease the values
		//Adjust max cell size
		if (Input.GetKeyDown(KeyCode.Keypad8)) {
			if (maxCount < 100) {
				maxCount++;
				print(maxCount);
			}
		}
		if (Input.GetKeyDown(KeyCode.Keypad2)) {
			if (maxCount >= 2) {
				maxCount--;
				print(maxCount);
			}
		}

		//Adjust cell regeneration rate
		if (Input.GetKeyDown(KeyCode.Keypad6)) {
			if (regenSpeed < 10) {
				regenSpeed += 0.1f;
				print(regenSpeed);
			}
		}
		if (Input.GetKeyDown(KeyCode.Keypad4)) {
			if (regenSpeed > 0.5f) {
				regenSpeed -= 0.1f;
				print(regenSpeed);
			}
		}


		#endregion
	}



	//Changes the colour back to original
	private void OnMouseExit() {
		if (!isSelected) {
			text.color = new Color32(255, 255, 255, 255);
		}
		else {
			text.color = new Color32(255, 0, 0, 255);
		}
	}

	private void OnMouseDown() {
		CellSelection.ModifySelection(this);
	}


}
