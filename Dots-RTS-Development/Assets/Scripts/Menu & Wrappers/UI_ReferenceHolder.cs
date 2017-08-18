﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class UI_ReferenceHolder : MonoBehaviour {

	#region References

	#region MainMenuRefs
	public static TextMeshProUGUI gameName;
	public static TextMeshProUGUI gameVersion;
	public static GameObject startGameButton;
	public static GameObject levelEditorButton;
	public static GameObject onlineLevelsButton;
	public static TextMeshProUGUI profileName;
	public static GameObject quotGameButton;

	#endregion

	#region EditorRefs

	#endregion

	#region LevelSelectRefs
	public static RectTransform rectCampaign;
	public static RectTransform rectCustom;
	public static GameObject centralToMainMenu;
	public static GameObject campaignButton;
	public static GameObject customButton;
	public static GameObject buyUpgradesSceneButton;
	#endregion

	#region LevelPlayerRefs
	public static GameObject menuPanel;

	#endregion

	#region LevelShareRefs

	#endregion

	#region DebugRefs
	//public static GameObject menuPanel;

	#endregion

	#region
	public static TextMeshProUGUI resultingJudgdement;
	public static TextMeshProUGUI didDominate;
	public static TextMeshProUGUI totalTimeToClear;
	public static TextMeshProUGUI totalCoinsAwarded;
	#endregion

	#region ProfileRefs
	public static GameObject PS_Canvas;
	public static GameObject PO_Canvas;
	public static TextMeshProUGUI PO_Name;
	public static TextMeshProUGUI PO_OnLevel;
	public static RawImage PO_OnLevelImage;
	public static TextMeshProUGUI PO_CurrentCoins;
	public static TextMeshProUGUI PO_GamesPlayed;
	public static Transform PO_AcquiredUpgrades;
	public static ProfileInfo PO_DeleteProfile;

	#endregion

	#region Upgrade Store
	public static Button buyButton;
	public static TextMeshProUGUI upgradeNameHolder;
	public static TextMeshProUGUI upgradeDescHolder;
	public static TextMeshProUGUI upgradeCostHolder;
	public static TextMeshProUGUI upgradesOwnedHolder;
	public static TextMeshProUGUI profileMoney;
	public static TextMeshProUGUI profileNameUpgradeStore;
	#endregion
	#endregion

	void Start() {
		SceneManager_activeSceneChanged(SceneManager.GetActiveScene(), SceneManager.GetActiveScene());
		SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
	}



	private void SceneManager_activeSceneChanged(Scene oldS, Scene newS) {

		switch (newS.name) {
			case "Main_Menu": { //Main Menu

				return;
			}
			case "Level_Editor": { // Editor

				return;
			}
			case "Level_Select": { // LevelSelect
				rectCampaign = GameObject.Find("Canvas_Campaign").GetComponent<RectTransform>();
				rectCustom = GameObject.Find("Canvas_CustomLevels").GetComponent<RectTransform>();
				centralToMainMenu = GameObject.Find("Return_To_Menu");
				campaignButton = GameObject.Find("Campaign_Button");
				customButton = GameObject.Find("Custom_Button");
				buyUpgradesSceneButton = GameObject.Find("Buy_Upgrades");
				return;
			}
			case "Level_Player": { //PlayScene
				menuPanel = GameObject.Find("Canvas").transform.Find("MenuPanel").gameObject;
				return;
			}
			case "Level_Sharing": { //Level Sharing

				return;
			}
			case "Debug": { // DebugScene
				menuPanel = GameObject.Find("Canvas").transform.Find("MenuPanel").gameObject;
				return;
			}
			case "Post_Game": { // PostGame Scene
				resultingJudgdement = GameObject.Find("Result").GetComponent<TextMeshProUGUI>();
				didDominate = GameObject.Find("Domination").GetComponent<TextMeshProUGUI>();
				totalTimeToClear = GameObject.Find("Time").GetComponent<TextMeshProUGUI>();
				totalCoinsAwarded = GameObject.Find("Total_Coins_Awarded").GetComponent<TextMeshProUGUI>();
				return;
			}
			case "Profiles": {
				PS_Canvas = GameObject.Find("PS_Canvas");
				PO_Canvas = GameObject.Find("PO_Canvas");
				PO_OnLevel = GameObject.Find("PO_OnLevel").GetComponent<TextMeshProUGUI>();
				PO_OnLevelImage = GameObject.Find("PO_OnLevelImage").GetComponent<RawImage>();
				PO_Name = GameObject.Find("PO_Name").GetComponent<TextMeshProUGUI>();
				PO_GamesPlayed = GameObject.Find("PO_GamesPlayed").GetComponent<TextMeshProUGUI>();
				PO_CurrentCoins = GameObject.Find("PO_CurrentCoins").GetComponent<TextMeshProUGUI>();
				PO_AcquiredUpgrades = GameObject.Find("PO_AcquiredUpgrades").transform;
				PO_DeleteProfile = GameObject.Find("PO_DeleteProfile").GetComponent<ProfileInfo>();
				PO_Canvas.SetActive(false);
				return;
			}
			case "Upgrade_Store": { //Upgrade Shop
				buyButton = GameObject.Find("Buy").GetComponent<Button>();
				upgradeNameHolder = GameObject.Find("Upgrade_Name").GetComponent<TextMeshProUGUI>();
				upgradeDescHolder = GameObject.Find("Upgrade_Desc").GetComponent<TextMeshProUGUI>();
				upgradeCostHolder = GameObject.Find("Upgrade_Cost_Money").GetComponent<TextMeshProUGUI>();
				upgradesOwnedHolder = GameObject.Find("Already_Owned_Count").GetComponent<TextMeshProUGUI>();
				profileNameUpgradeStore = GameObject.Find("Profile_Name").GetComponent<TextMeshProUGUI>();
				profileMoney = GameObject.Find("Coins_To_Spend").GetComponent<TextMeshProUGUI>();

				return;
			}
		}
	}
}
