using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SoundManager : MonoBehaviour {
	public List<AudioSource> sources = new List<AudioSource>();
	public bool play;


	// Use this for initialization
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
		//print("attempt");
		bool added = false;
		foreach (AudioSource s in sources) {
			if(s.clip == null) {
				//print("Added");
				added = true;
				s.clip = newClip;
				s.Play();
				StartCoroutine(RemoveClipAfterFinish(s));
				return;
			}
		}
		if (!added) {
			print("Not Enough Sound players");
		}

	}

	public IEnumerator RemoveClipAfterFinish(AudioSource s) {
		yield return new WaitUntil(() => !s.isPlaying);
		s.clip = null;
		//print("Removed");
	}
}
