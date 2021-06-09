using UnityEngine;

public static class Extensions {
	public static T Find<T>() {
		return GameObject.Find(typeof(T).Name).GetComponent<T>();
	}
}