using UnityEngine;

public static class Extensions {

	public static T Find<T>() {
		return GameObject.Find(typeof(T).Name).GetComponent<T>();
	}

	public static T FindInactive<T>(Transform parent) {
		return parent.Find(typeof(T).Name).GetComponent<T>();
	}

}