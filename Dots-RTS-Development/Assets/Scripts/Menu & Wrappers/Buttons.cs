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
		SceneManager.LoadScene(1);
	}

	public void PauseGameorEscape() {
		InputSimulator.SimulateKeyPress(VirtualKeyCode.ESCAPE);
	}

	public void CreateNewProfile() {
		Control.pM.ShowProfileCreation();
	}

	public void SetSelectedUpgrade() {
		int selected = Upgrade_Manager.selectedUpgrade = int.Parse(string.Format(gameObject.name).Remove(0, 8));
		string[] upgradeInfo = FolderAccess.GetUpgradeDesc(selected);

		UI_ReferenceHolder.upgradeNameHolder.text = upgradeInfo[0];
		UI_ReferenceHolder.upgradeDescHolder.text = upgradeInfo[1];
		UI_ReferenceHolder.upgradeCostHolder.text = upgradeInfo[2] + " coins";

	}
}