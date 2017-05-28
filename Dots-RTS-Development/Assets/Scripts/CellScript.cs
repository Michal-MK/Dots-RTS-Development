using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellScript : MonoBehaviour {

	private int count;
	private float regenSpeed;
	private int maxCount;
	private Vector2 position;

	public TextMesh text;
	public GameObject cellObj;


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



	//Keeps generateing new elements
	public IEnumerator Generate() {
		print(regenSpeed);
		while (true) {
			yield return new WaitForSecondsRealtime(regenSpeed);
			print(count);
			if (count < maxCount) {
				count++;
				text.text = count.ToString();
			}
		}
	}


	//Changes colour when howered over
	private void OnMouseOver() {
		text.color = new Color32(255, 0, 0, 255);
	}

	//Changes the colour back to original
	private void OnMouseExit() {
		text.color = new Color(1, 1, 1);
	}
}
