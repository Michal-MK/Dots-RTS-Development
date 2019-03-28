using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
	public List<AudioSource> sources = new List<AudioSource>();
	public bool play;

	void Start() {
		play = true;
		foreach (AudioSource s in transform.GetComponentsInChildren<AudioSource>()) {
			sources.Add(s);
		}
	}
	private void OnDestroy() {
		play = false;
	}

	public void AddToSoundQueue(AudioClip newClip) {
		foreach (AudioSource s in sources) {
			if (s.clip == null) {
				s.clip = newClip;
				s.Play();
				StartCoroutine(RemoveClipAfterFinish(s));
				return;
			}
		}
	}

	public IEnumerator RemoveClipAfterFinish(AudioSource s) {
		yield return new WaitUntil(() => !s.isPlaying);
		s.clip = null;
	}
}
