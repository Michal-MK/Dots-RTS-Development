using UnityEngine;
using UnityEngine.UI;

public class Buttons : MainMenuUI {

	public void Continue(Button sender) {
		Control.Script.PauseAttempt(sender);
	}

	public void LoadScene(string sceneName) {
		SceneLoader.Instance.Load(sceneName, null);
	}

	public void FitCellsToggle(Toggle toggle) {
		Extensions.Find<LevelEditorCore>().CellsFitToScreen = toggle.isOn;
	}

	public void PauseGameOrEscape(Button sender) {
		Control.Script.PauseAttempt(sender);
	}

	public void CreateNewProfile() {
		ProfileManager.Instance.ShowProfileCreation();
	}

	public void SetSelectedUpgrade() {
		int selected = StoreUpgradeManager.selectedUpgrade = int.Parse(gameObject.name.Remove(0, 8));

		if (selected <= 99) {
			if (selected < Upgrade.TOTAL_OFFENSIVE_UPGRADES) {
				UpgradeData upgradeInfo = FolderAccess.GetUpgrade((Upgrades)selected);
				
				if (upgradeInfo == null) return;
				UI_ReferenceHolder.U_upgradeNameHolder.text = upgradeInfo.Name;
				UI_ReferenceHolder.U_upgradeDescHolder.text = upgradeInfo.Description;
				UI_ReferenceHolder.U_upgradeCostHolder.text = Upgrade.GetCost((Upgrades)selected) + " coins";
				UI_ReferenceHolder.U_upgradesOwnedHolder.text = ProfileManager.CurrentProfile.AcquiredUpgrades[(Upgrades)selected] + " pcs";
				GetComponent<Image>().sprite = Upgrade.UpgradeGraphics[(Upgrades)selected];
			}
			else {
				UI_ReferenceHolder.U_upgradeNameHolder.text = "Nothing Yet";
				UI_ReferenceHolder.U_upgradeDescHolder.text = "OFFENSIVE UPGRADE";
				UI_ReferenceHolder.U_upgradeCostHolder.text = "Infinite coins";
				UI_ReferenceHolder.U_upgradesOwnedHolder.text = "x pcs.";
				GetComponent<Image>().sprite = FolderAccess.GetNIYImage();
			}
		}
		else if (selected <= 199) {
			if (selected < 100 + Upgrade.TOTAL_DEFENSIVE_UPGRADES) {
				UpgradeData upgradeInfo = FolderAccess.GetUpgrade((Upgrades)selected);
				if (upgradeInfo == null) return;
				
				UI_ReferenceHolder.U_upgradeNameHolder.text = upgradeInfo.Name;
				UI_ReferenceHolder.U_upgradeDescHolder.text = upgradeInfo.Description;
				UI_ReferenceHolder.U_upgradeCostHolder.text = Upgrade.GetCost((Upgrades)selected) + " coins";
				UI_ReferenceHolder.U_upgradesOwnedHolder.text = ProfileManager.CurrentProfile.AcquiredUpgrades[(Upgrades)selected] + " pcs";
				GetComponent<Image>().sprite = Upgrade.UpgradeGraphics[(Upgrades)selected];
			}
			else {
				UI_ReferenceHolder.U_upgradeNameHolder.text = "Nothing Yet";
				UI_ReferenceHolder.U_upgradeDescHolder.text = "DEFENSIVE UPGRADE";
				UI_ReferenceHolder.U_upgradeCostHolder.text = "Infinite coins";
				UI_ReferenceHolder.U_upgradesOwnedHolder.text = "x pcs.";
				GetComponent<Image>().sprite = FolderAccess.GetNIYImage();
			}
		}
		else {
			if (selected < 200 + Upgrade.TOTAL_UTILITY_UPGRADES) {
				UpgradeData upgradeInfo = FolderAccess.GetUpgrade((Upgrades)selected);
				if (upgradeInfo == null) return;
				
				UI_ReferenceHolder.U_upgradeNameHolder.text = upgradeInfo.Name;
				UI_ReferenceHolder.U_upgradeDescHolder.text = upgradeInfo.Description;
				UI_ReferenceHolder.U_upgradeCostHolder.text = Upgrade.GetCost((Upgrades)selected) + " coins";
				UI_ReferenceHolder.U_upgradesOwnedHolder.text = ProfileManager.CurrentProfile.AcquiredUpgrades[(Upgrades)selected] + " pcs";
				GetComponent<Image>().sprite = Upgrade.UpgradeGraphics[(Upgrades)selected];
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

	public void ShowHidePanel(GameObject window) {
		if (window.GetComponent<RectTransform>().anchoredPosition == Vector2.zero) {
			window.GetComponent<Animator>().SetTrigger(AnimatorStates.HIDE);
			WindowManagement.Instance.CloseWindow(window);
		}
		else {
			window.GetComponent<Animator>().SetTrigger(AnimatorStates.SHOW);
			WindowManagement.Instance.AddWindow(new Window(window, window.GetComponent<Animator>(), false, null));
		}
	}

	public void OpenAsWindow(GameObject obj) {
		WindowManagement.Instance.AddWindow(new Window(obj, null));
	}
}
