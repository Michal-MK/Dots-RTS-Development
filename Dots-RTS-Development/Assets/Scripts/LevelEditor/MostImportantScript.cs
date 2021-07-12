using UnityEngine;

public class MostImportantScript : MonoBehaviour {
	public AudioSource source;

	private void OnEnable() {
		source.Play();
	}
}
