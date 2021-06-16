using UnityEngine;

public class CameraControler : MonoBehaviour {
	public RectTransform background;

	private Vector3 defaultPos;
	private float defaultSize = 0;

	private float camHorDiff;
	private float camVertDiff;
	private float camHorDiffL;
	private float camVertDiffB;


	private static bool isSubscribed = false;

	private void Awake() {
		if (!isSubscribed) {
			//print("Subscribed Camera");
			//UM_InGame.OnUpgradeBegin += OnBeginUpgrading; //TODO
			//UM_InGame.OnUpgradeQuit += OnQuitUpgrading;
			isSubscribed = true;
		}
	}
	private void OnDestroy() {
		//print("Unsubbed Cam");
		//UM_InGame.OnUpgradeBegin -= OnBeginUpgrading;
		//UM_InGame.OnUpgradeQuit -= OnQuitUpgrading;
		isSubscribed = false;
	}

	private void Start() {
		defaultPos = transform.position;
		defaultSize = Camera.main.orthographicSize;
	}

	private void OnBeginUpgrading(UpgradeManager sender) {
		Camera cam = Camera.main;
		if (defaultSize == 0) {
			cam.orthographicSize = defaultSize;
			print(defaultSize);
		}

		cam.orthographicSize *= 0.5f;

		Vector3 newPos = sender.transform.position + (Vector3.back * 10);

		SpriteRenderer bg = GameObject.Find("Background").GetComponent<SpriteRenderer>();

		camVertDiff = (newPos.y + cam.orthographicSize) - (bg.transform.position.y + bg.size.y / 2);
		//print("Diff to TOP " + camVertDiff);

		camHorDiff = (newPos.x + cam.orthographicSize * cam.aspect) - (bg.transform.position.x + bg.size.x / 2);
		//print("Diff to RIGHT " + camHorDiff);

		camHorDiffL = (newPos.x - cam.orthographicSize * cam.aspect) - (bg.transform.position.x - bg.size.x / 2);
		//print("Diff to LEFT " + camHorDiffL);

		camVertDiffB = (newPos.y - cam.orthographicSize) - (bg.transform.position.y - bg.size.y / 2);
		//print("Diff to BOTTOM " + camVertDiffB);


		Vector3 percentage = cam.WorldToViewportPoint(newPos);


		//RightSide
		if (camHorDiff > 0) {
			newPos = new Vector3(newPos.x - camHorDiff, newPos.y, newPos.z);
		}
		//Top
		if (camVertDiff > 0) {
			newPos = new Vector3(newPos.x, newPos.y - camVertDiff, newPos.z);
		}
		//Left
		if (camHorDiffL < 0) {
			newPos = new Vector3(newPos.x - camHorDiffL, newPos.y, newPos.z);
		}
		//Bottom
		if (camVertDiffB < 0) {
			newPos = new Vector3(newPos.x, newPos.y - camVertDiffB, newPos.z);
		}

		//Vector3 percentage = c.WorldToViewportPoint(newPos);
		print(percentage);

		if (percentage.y <= 0.1f) {
			//print("A");
			Vector3 pointU = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.3f));
			Vector3 pointD = cam.ViewportToWorldPoint(new Vector3(0.5f, 0));

			float d = Vector3.Distance(pointU, pointD);

			transform.position = new Vector3(newPos.x, newPos.y - d, newPos.z);
		}
		else if (percentage.y > 0.96f) { //Hello?
			//print("B");
			transform.position = new Vector3(newPos.x, newPos.y, newPos.z);

		}
		else {
			//print("C");
			Vector3 pointU = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.65f));
			float d = Vector3.Distance(cam.transform.position, pointU);
			transform.position = new Vector3(newPos.x, newPos.y - d, newPos.z);
		}
	}

	private void OnQuitUpgrading(UpgradeManager sender) {
		//print("Cam Def Size " + defaultSize);
		Camera.main.orthographicSize = defaultSize;
		transform.position = defaultPos;
	}
}
