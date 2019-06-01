using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using WindowsInput;
using TMPro;

public class Buttons : MainMenuUI  {
	
	public void Continue() {
		Control.Script.UnPause();
		Time.timeScale = 1;
	}

	public void OnNewWindowOpen(GameObject newWindow) {
		UI_Manager.AddWindow(newWindow);
		newWindow.SetActive(true);
	}

	public void SwitchUpgradeWindow(Transform transform) {
		GameObject atk = GameObject.Find("UPGRADE_Panel").transform.Find("ATK_Upgrades").gameObject;
		GameObject def = GameObject.Find("UPGRADE_Panel").transform.Find("DEF_Upgrades").gameObject;
		GameObject util = GameObject.Find("UPGRADE_Panel").transform.Find("UTIL_Upgrades").gameObject;
		if(transform.name == "ATK") {
			def.SetActive(false);
			atk.SetActive(true);
			util.SetActive(false);

		}
		else if (transform.name == "DEF") {
			def.SetActive(true);
			atk.SetActive(false);
			util.SetActive(false);

		}
		else if(transform.name == "UTIL") {
			atk.SetActive(false);
			def.SetActive(false);
			util.SetActive(true);
		}
		if(SceneManager.GetActiveScene().name == Scenes.GAME) {
			string typeName;
			Sprite sprite;

			switch (transform.name) {
				case "ATK": {
					typeName = "Offensive Upgrades";
					Global.spriteDictionary.TryGetValue("AttackIcon", out sprite);
					print(sprite.bounds);
					break;
				}
				case "DEF": {
					typeName = "Defensive Upgrades";
					Global.spriteDictionary.TryGetValue("DefenceIcon", out sprite);
					print(sprite.bounds);
					break;
				}
				case "UTIL": {
					typeName = "Utility based Upgrades";
					Global.spriteDictionary.TryGetValue("UtilityIcon", out sprite);
					break;
				}
				default: {
					typeName = "Unknown Upgrades";
					sprite = null;
					break;
				}
			}
			UI_ReferenceHolder.LP_TypeUpgradeText.text = typeName;
			UI_ReferenceHolder.LP_TypeUpgradeImage.sprite = sprite;
		}
	}

	public void EditMode() {
		SceneManager.LoadScene(Scenes.LEVEL_EDITOR);
	}
	public void FitCellsToggle(Toggle toggle) {
		GameObject.Find("Core").GetComponent<LevelEditorCore>().areCellsFitToScreen = toggle.isOn;
	}

	public void PauseGameorEscape() {
		InputSimulator.SimulateKeyPress(VirtualKeyCode.ESCAPE);
	}

	public void CreateNewProfile() {
		ProfileManager.Instance.ShowProfileCreation();
	}

	public void SetSelectedUpgrade() {
		int selected = UM_Store.selectedUpgrade = (int.Parse(string.Format(gameObject.name).Remove(0, 8)));

		if(selected <= 99) {
			if (selected < Upgrade.TOTAL_OFFENSIVE_UPGRADES) {
				string[] upgradeInfo = FolderAccess.GetUpgrade((Upgrades)selected);
				if (upgradeInfo != null) {
					UI_ReferenceHolder.U_upgradeNameHolder.text = upgradeInfo[0];
					UI_ReferenceHolder.U_upgradeDescHolder.text = upgradeInfo[1];
					UI_ReferenceHolder.U_upgradeCostHolder.text = Upgrade.GetCost((Upgrades)selected).ToString() + " coins";
					UI_ReferenceHolder.U_upgradesOwnedHolder.text = ProfileManager.CurrentProfile.AcquiredUpgrades[(Upgrades)selected].ToString() + " pcs";
					GetComponent<Image>().sprite = Upgrade.UPGRADE_GRAPHICS[(Upgrades)selected];
				}
			}
			else {
				UI_ReferenceHolder.U_upgradeNameHolder.text = "Nothing Yet";
				UI_ReferenceHolder.U_upgradeDescHolder.text = "OFFENSIVE UPGRADE";
				UI_ReferenceHolder.U_upgradeCostHolder.text = "Infinite coins";
				UI_ReferenceHolder.U_upgradesOwnedHolder.text = "x pcs.";
				GetComponent<Image>().sprite = FolderAccess.GetNIYImage();
			}
		}
		else if(selected > 99 && selected <= 199) {
			if (selected < 100 + Upgrade.TOTAL_DEFENSIVE_UPGRADES) {
				string[] upgradeInfo = FolderAccess.GetUpgrade((Upgrades)selected);
				if (upgradeInfo != null) {
					UI_ReferenceHolder.U_upgradeNameHolder.text = upgradeInfo[0];
					UI_ReferenceHolder.U_upgradeDescHolder.text = upgradeInfo[1];
					UI_ReferenceHolder.U_upgradeCostHolder.text = Upgrade.GetCost((Upgrades)selected).ToString() + " coins";
					UI_ReferenceHolder.U_upgradesOwnedHolder.text = ProfileManager.CurrentProfile.AcquiredUpgrades[(Upgrades)selected].ToString() + " pcs";
					GetComponent<Image>().sprite = Upgrade.UPGRADE_GRAPHICS[(Upgrades)selected];
				}
			}
			else {
				UI_ReferenceHolder.U_upgradeNameHolder.text = "Nothing Yet";
				UI_ReferenceHolder.U_upgradeDescHolder.text = "DEFENSIVE UPGRADE";
				UI_ReferenceHolder.U_upgradeCostHolder.text = "Infinite coins";
				UI_ReferenceHolder.U_upgradesOwnedHolder.text = "x pcs.";
				GetComponent<Image>().sprite = FolderAccess.GetNIYImage();
			}
		}
		else{
			if (selected < 200 + Upgrade.TOTAL_UTILITY_UPGRADES) {
				string[] upgradeInfo = FolderAccess.GetUpgrade((Upgrades)selected);
				if (upgradeInfo != null) {
					UI_ReferenceHolder.U_upgradeNameHolder.text = upgradeInfo[0];
					UI_ReferenceHolder.U_upgradeDescHolder.text = upgradeInfo[1];
					UI_ReferenceHolder.U_upgradeCostHolder.text = Upgrade.GetCost((Upgrades)selected).ToString() + " coins";
					UI_ReferenceHolder.U_upgradesOwnedHolder.text = ProfileManager.CurrentProfile.AcquiredUpgrades[(Upgrades)selected].ToString() + " pcs";
					GetComponent<Image>().sprite = Upgrade.UPGRADE_GRAPHICS[(Upgrades)selected];
				}
			}
			else {
				UI_ReferenceHolder.U_upgradeNameHolder.text = "Nothing Yet";
				UI_ReferenceHolder.U_upgradeDescHolder.text = "UTILITY UPGRADE";
				UI_ReferenceHolder.U_upgradeCostHolder.text = "Infinite coins";
				UI_ReferenceHolder.U_upgradesOwnedHolder.text = "x pcs.";
				GetComponent<Image>().sprite = FolderAccess.GetNIYImage();
			}
		}
	}


	public void HighlightSelectedToggle() {
		Color32 defaultColor = new Color32(255, 196, 4, 255);
		//if (gameObject.GetComponent<Toggle>().isOn) {
		//	transform.Find("Text").GetComponent<TextMeshProUGUI>().color = Color.red;
		//}
		//else {
			transform.Find("Text").GetComponent<TextMeshProUGUI>().color = defaultColor;
		//}
	}

	public void ShowHidePanel(GameObject window) {
		if (window.GetComponent<RectTransform>().anchoredPosition == Vector2.zero) {
			window.GetComponent<Animator>().SetTrigger("Hide");
			UI_Manager.CloseWindow(window);
		}
		else {
			window.GetComponent<Animator>().SetTrigger("Show");
			UI_Manager.AddWindow(window);
		}
	}
}