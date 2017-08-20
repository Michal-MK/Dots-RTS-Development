using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WindowsInput;

public class Buttons : MainMenuUI  {
	
	public void Continue() {
		Control.UnPause();
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
			UI_ReferenceHolder.upgradeNameHolder.text = "Nothing Yet";
			UI_ReferenceHolder.upgradeDescHolder.text = "Some desc here. lel";
			UI_ReferenceHolder.upgradeCostHolder.text = "Infinite coins";
		}
		else {
			string[] upgradeInfo = FolderAccess.GetUpgrade((Upgrade.Upgrades)selected);
			if (upgradeInfo != null) {
				UI_ReferenceHolder.upgradeNameHolder.text = upgradeInfo[0];
				UI_ReferenceHolder.upgradeDescHolder.text = upgradeInfo[1];
				UI_ReferenceHolder.upgradeCostHolder.text = Upgrade.GetCost((Upgrade.Upgrades)selected).ToString() + " coins";
			}
		}
	}
}