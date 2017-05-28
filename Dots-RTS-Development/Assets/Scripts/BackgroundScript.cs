using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScript : MonoBehaviour {
	public GameObject cellObj;


	// Use this for initialization
	void Start () {
		
	}

	private void OnMouseDown() {
		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		GameObject cell = Instantiate(cellObj,gameObject.transform);
		cell.GetComponent<CellScript>().SetCellData(mousePos);
	}

	// Update is called once per frame
	void Update () {
		
	}
}