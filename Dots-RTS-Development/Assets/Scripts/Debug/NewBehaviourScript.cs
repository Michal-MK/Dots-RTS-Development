using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		print(GetComponent<RectTransform>().anchoredPosition);
		GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		print(GetComponent<RectTransform>().anchoredPosition);
	}
}
