using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour {

	public Text errorText;
	public RectTransform upgradePanel;
	Vector3 hiddenUpgradesPos;

	private void Awake() {
		Upgrade_Manager.OnUpgradeBegin += Upgrade_Manager_OnUpgradeBegin;
		Upgrade_Manager.OnUpgradeQuit += Upgrade_Manager_OnUpgradeQuit;
	}
	private void OnDisable() {
		Upgrade_Manager.OnUpgradeBegin -= Upgrade_Manager_OnUpgradeBegin;
		Upgrade_Manager.OnUpgradeQuit -= Upgrade_Manager_OnUpgradeQuit;
	}



	private void Upgrade_Manager_OnUpgradeBegin(Upgrade_Manager sender) {
		upgradePanel.anchoredPosition = Vector2.zero;
		//upgrades.GetComponent<Animator>().Play("Show");
	}

	private void Upgrade_Manager_OnUpgradeQuit(Upgrade_Manager sender) {
		upgradePanel.anchoredPosition = new Vector2(0, -360);
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
		SceneManager.LoadScene(2);
	}



}
