using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour {
	public List<AudioSource> sources = new List<AudioSource>();

	void Start() {
		foreach (AudioSource s in transform.GetComponentsInChildren<AudioSource>()) {
			sources.Add(s);
		}
	}

	public void PlaySound(AudioClip newClip) {
		foreach (AudioSource s in sources.Where(s => s.clip == null)) {
			s.clip = newClip;
			s.Play();
			StartCoroutine(RemoveClipAfterFinish(s));
			return;
		}
	}

	private IEnumerator RemoveClipAfterFinish(AudioSource s) {
		yield return new WaitUntil(() => !s.isPlaying);
		s.clip = null;
	}
}
