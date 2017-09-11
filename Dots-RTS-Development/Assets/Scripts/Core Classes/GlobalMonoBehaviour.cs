using UnityEngine;
using System.Collections.Generic;

public class GlobalMonoBehaviour : MonoBehaviour {

	private void Start() {
		StartCoroutine(GetSpritesFromResources("AttackIcon"));
		StartCoroutine(GetSpritesFromResources("DefenceIcon"));
		StartCoroutine(GetSpritesFromResources("UtilityIcon"));
		StartCoroutine(GetSpritesFromResources("TeamBox"));

	}

	IEnumerator<Sprite> GetSpritesFromResources(string path) {
		ResourceRequest s = Resources.LoadAsync<Sprite>(path);
		yield return (Sprite)s.asset;

		Global.spriteDictionary.Add(path, (Sprite)s.asset);
		//print("Got IT");
	}
}
