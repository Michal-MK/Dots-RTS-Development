using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {

	private int count;
	private float regenSpeed = 1;
	private int maxCount;
	Coroutine generate;
	public TextMesh text;
	Cell newCell;

	public Cell(int count, int maxCount, float regenSpeed) {
		this.count = count;
		this.maxCount = maxCount;
		this.regenSpeed = regenSpeed;
	}


	private void Start() {
		count = UnityEngine.Random.Range(0, 10);
		text.text = count.ToString();
		maxCount = 10;
		generate = StartCoroutine(Generate());
	}

	public Cell createNewCell(int count, int max, float regenSpeed) {
		return new Cell(count, maxCount, regenSpeed);
	}


	public IEnumerator Generate() {
		while (true) {
			yield return new WaitForSeconds(regenSpeed);
			if (count < maxCount) {
				count++;
				text.text = count.ToString();
			}
		}
	}
}
