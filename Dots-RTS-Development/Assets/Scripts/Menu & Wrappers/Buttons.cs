using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using WindowsInput;

public class Buttons : MainMenuUI  {
	
	public void Continue() {
		Control.UnPause();
		Time.timeScale = 1;
	}

	public void OnNewWindowOpen(GameObject newWindow) {
		UI_Manager.AddWindow(newWindow);
		newWindow.SetActive(true);
	}

	public void SwitchUpgradeWindow(string buttonName) {
		GameObject atk = GameObject.Find("Canvas").transform.Find("ATK_Upgrades").gameObject;
		GameObject def = GameObject.Find("Canvas").transform.Find("DEF_Upgrades").gameObject;
		if(buttonName == "ATK") {
			def.SetActive(false);
			atk.SetActive(true);
		}
		else if (buttonName == "DEF") {
			def.SetActive(true);
			atk.SetActive(false);
		}
	}

	public void EditMode() {
		SceneManager.LoadScene(Scenes.EDITOR);
	}
	public void FitCellsToggle(Toggle toggle) {
		GameObject.Find("Core").GetComponent<LevelEditorCore>().areCellsFitToScreen = toggle.isOn;
	}

	public void PauseGameorEscape() {
		InputSimulator.SimulateKeyPress(VirtualKeyCode.ESCAPE);
	}

	public void CreateNewProfile() {
		Control.pM.ShowProfileCreation();
	}

	public void SetSelectedUpgrade() {
		int selected = Upgrade_Manager.selectedUpgrade = (int.Parse(string.Format(gameObject.name).Remove(0, 8)));
		//print(selected);

		if(selected < 99) {
			if (selected < Upgrade.TOTAL_OFFENSIVE_UPGRADES) {
				string[] upgradeInfo = FolderAccess.GetUpgrade((Upgrade.Upgrades)selected);
				if (upgradeInfo != null) {
					UI_ReferenceHolder.U_upgradeNameHolder.text = upgradeInfo[0];
					UI_ReferenceHolder.U_upgradeDescHolder.text = upgradeInfo[1];
					UI_ReferenceHolder.U_upgradeCostHolder.text = Upgrade.GetCost((Upgrade.Upgrades)selected).ToString() + " coins";
					UI_ReferenceHolder.U_upgradesOwnedHolder.text = ProfileManager.getCurrentProfile.acquiredUpgrades[(Upgrade.Upgrades)selected].ToString() + " pcs";
					GetComponent<Image>().sprite = Upgrade.UPGRADE_GRAPHICS[(Upgrade.Upgrades)selected];
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
		else if(selected > 99) {
			if (selected < 100 + Upgrade.TOTAL_DEFENSIVE_UPGRADES) {
				string[] upgradeInfo = FolderAccess.GetUpgrade((Upgrade.Upgrades)selected);
				if (upgradeInfo != null) {
					UI_ReferenceHolder.U_upgradeNameHolder.text = upgradeInfo[0];
					UI_ReferenceHolder.U_upgradeDescHolder.text = upgradeInfo[1];
					UI_ReferenceHolder.U_upgradeCostHolder.text = Upgrade.GetCost((Upgrade.Upgrades)selected).ToString() + " coins";
					UI_ReferenceHolder.U_upgradesOwnedHolder.text = ProfileManager.getCurrentProfile.acquiredUpgrades[(Upgrade.Upgrades)selected].ToString() + " pcs";
					GetComponent<Image>().sprite = Upgrade.UPGRADE_GRAPHICS[(Upgrade.Upgrades)selected];
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
	}
}