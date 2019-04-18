using UnityEngine;

public class ProfileManagerBehaviour : MonoBehaviour {

	public GameObject profileVisual;

	public Transform listTransform;
	private void Awake() {
		ProfileManager.Initialize(this);
	}
}
