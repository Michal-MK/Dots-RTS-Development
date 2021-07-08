using System;
using UnityEngine;

[Serializable]
public class SerializedVector3 {
	public SerializedVector3() { }

	public SerializedVector3(Vector3 vec) {
		x = vec.x;
		y = vec.y;
		z = vec.z;
	}

	public float x, y, z;

	public static explicit operator Vector3(SerializedVector3 v) {
		return new Vector3(v.x, v.y, v.z);
	}

	public static explicit operator Vector2(SerializedVector3 v) {
		return new Vector2(v.x, v.y);
	}
}