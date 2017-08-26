using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

	public void EditMode() {
		Control.cells.Clear();
		SceneManager.LoadScene("Level_Editor");
	}

	public void PauseGameorEscape() {
		InputSimulator.SimulateKeyPress(VirtualKeyCode.ESCAPE);
	}

	public void CreateNewProfile() {
		Control.pM.ShowProfileCreation();
	}

	public void SetSelectedUpgrade() {
		int selected = Upgrade_Manager.selectedUpgrade = (int.Parse(string.Format(gameObject.name).Remove(0, 8)) -1);
		if (selected >= Upgrade.TOTAL_UPGRADES) {
			UI_ReferenceHolder.U_upgradeNameHolder.text = "Nothing Yet";
			UI_ReferenceHolder.U_upgradeDescHolder.text = "Some desc here. lel";
			UI_ReferenceHolder.U_upgradeCostHolder.text = "Infinite coins";
			UI_ReferenceHolder.U_upgradesOwnedHolder.text = "x pcs.";
		}
		else {
			string[] upgradeInfo = FolderAccess.GetUpgrade((Upgrade.Upgrades)selected);
			if (upgradeInfo != null) {
				UI_ReferenceHolder.U_upgradeNameHolder.text = upgradeInfo[0];
				UI_ReferenceHolder.U_upgradeDescHolder.text = upgradeInfo[1];
				UI_ReferenceHolder.U_upgradeCostHolder.text = Upgrade.GetCost((Upgrade.Upgrades)selected).ToString() + " coins";
				UI_ReferenceHolder.U_upgradesOwnedHolder.text = ProfileManager.getCurrentProfile.acquiredUpgrades[(Upgrade.Upgrades)selected].ToString() + " pcs";
			}
		}
	}
}