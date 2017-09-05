using UnityEngine;

[RequireComponent(typeof(Animation))]
public class LocalAnimation : MonoBehaviour {
	Vector3 localPos;
	bool wasPlaying;
	new Animation animation;

	void Awake() {
		localPos = transform.position;
		wasPlaying = false;
		animation = GetComponent<Animation>();
	}

	void LateUpdate() {
		if (!animation.isPlaying && !wasPlaying) {
			return;
		}

		transform.localPosition += localPos;

		wasPlaying = animation.isPlaying;
	}
}