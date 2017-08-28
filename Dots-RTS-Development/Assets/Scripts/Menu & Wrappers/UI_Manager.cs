using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour {

	public Text errorText;
	Vector3 hiddenUpgradesPos;
    public GameObject[] thingsToDisable = new GameObject[2];


	private static Stack<GameObject> activeWindows = new Stack<GameObject>();


    private void Awake() {
		Upgrade_Manager.OnUpgradeBegin += Upgrade_Manager_OnUpgradeBegin;
		Upgrade_Manager.OnUpgradeQuit += Upgrade_Manager_OnUpgradeQuit;
	}
	private void OnDisable() {
		Upgrade_Manager.OnUpgradeBegin -= Upgrade_Manager_OnUpgradeBegin;
		Upgrade_Manager.OnUpgradeQuit -= Upgrade_Manager_OnUpgradeQuit;
	}



	private void Upgrade_Manager_OnUpgradeBegin(Upgrade_Manager sender) {
		AddWindow(UI_ReferenceHolder.upgradePanel.gameObject);
		UI_ReferenceHolder.upgradePanel.anchoredPosition = Vector2.zero;
		//upgrades.GetComponent<Animator>().Play("Show");
	}

	private void Upgrade_Manager_OnUpgradeQuit(Upgrade_Manager sender) {
		UI_ReferenceHolder.upgradePanel.anchoredPosition = new Vector2(0, -360);
		//upgrades.GetComponent<Animator>().Play("Hide");
	}


	// Save Error checking
	public void NoFileFound() {
		StartCoroutine(ReturnToLevelSelectIn(5));
		errorText.gameObject.SetActive(true);
		errorText.text = "No file found, returing to level select";
	}
	IEnumerator ReturnToLevelSelectIn(float seconds) {
		yield return new WaitForSeconds(seconds);
		SceneManager.LoadScene(Scenes.SELECT);
	}

    public void ChangeLayoutToPreview() {
		print("Loool what a meme");
        for (int i = 0; i < thingsToDisable.Length; i++) {
            thingsToDisable[i].SetActive(false);
        }
    }


	public static void AddWindow(GameObject window) {
		activeWindows.Push(window);
		if (!window.activeInHierarchy) {
			window.SetActive(true);
		}
		Control.isPaused = false;
		Time.timeScale = 1;
	}

	public static void CloseMostRecent() {
		if (activeWindows.Count > 0) {
			GameObject g = activeWindows.Pop();
			g.SetActive(false);
		}
		else {
			Control.Pause();
		}
	}

	public static void CloseAllActive() {
		foreach (GameObject g in activeWindows) {
			g.SetActive(false);
		}
		activeWindows = new Stack<GameObject>();
	}
	public static int getWindowCount {
		get {
			return activeWindows.Count;
		}
	}
}
