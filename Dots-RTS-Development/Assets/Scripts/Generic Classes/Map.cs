using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

	//Maps the value of "number" from a set range to another
	public static float MapFloat( float number, float fromRange, float toRange, float mapFrom, float mapTo) {
		if(number < fromRange || number > toRange) {
			Debug.LogError("Input number: " + number + "is not from defined range!");
		}

		if(number == fromRange) {
			return mapFrom;
		}else if (number == toRange) {
			return mapTo;
		}
		else {
			return ((number - fromRange) * (mapTo - mapFrom) / (toRange - fromRange)) + mapFrom;
		}
	}
}
