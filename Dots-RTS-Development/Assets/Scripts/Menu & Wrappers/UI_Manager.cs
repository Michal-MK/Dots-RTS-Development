using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour {

	public Text errorText;
    public GameObject[] thingsToDisable = new GameObject[2];

	private static Stack<GameObject> activeWindows = new Stack<GameObject>();

	private void Awake() {
		if (SceneManager.GetActiveScene().name == Scenes.PLAYER) {
			Upgrade_Manager.OnUpgradeBegin += Upgrade_Manager_OnUpgradeBegin;
			Upgrade_Manager.OnUpgradeQuit += Upgrade_Manager_OnUpgradeQuit;
		}
	}
	private void OnDisable() {
		Upgrade_Manager.OnUpgradeBegin -= Upgrade_Manager_OnUpgradeBegin;
		Upgrade_Manager.OnUpgradeQuit -= Upgrade_Manager_OnUpgradeQuit;
		activeWindows.Clear();
	}



	private void Upgrade_Manager_OnUpgradeBegin(Upgrade_Manager sender) {
		AddWindow(UI_ReferenceHolder.MULTI_upgradePanel.gameObject);
		UI_ReferenceHolder.MULTI_upgradePanel.anchoredPosition = Vector2.zero;
		//upgrades.GetComponent<Animator>().Play("Show");
	}

	private void Upgrade_Manager_OnUpgradeQuit(Upgrade_Manager sender) {
		UI_ReferenceHolder.MULTI_upgradePanel.anchoredPosition = new Vector2(0, -360);
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

	public static void CloseMostRecent(int count = 1) {
		if (activeWindows.Count > 0) {
			GameObject g = activeWindows.Pop();
			if (g != null) {
				g.SetActive(false);
			}
			else {
				Control.script.Pause();
			}
		}
		else {
			Control.script.Pause();
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
