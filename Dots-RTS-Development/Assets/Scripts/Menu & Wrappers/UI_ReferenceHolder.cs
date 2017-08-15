using UnityEngine;
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

	#endregion

	#region LevelShareRefs

	#endregion

	#region DebugRefs
	public static Slider maxSizeSlider;
	#endregion

	#region
	public static TextMeshProUGUI resultingJudgdement;
	public static TextMeshProUGUI didDominate;
	public static TextMeshProUGUI totalTimeToClear;
	public static TextMeshProUGUI totalCoinsAwarded;
	#endregion

	#region ProfileRefs

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
		SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
	}



	private void SceneManager_activeSceneChanged(Scene oldS, Scene newS) {

		switch (newS.buildIndex) {
			case 0: {

				return;
			}
			case 1: {

				return;
			}
			case 2: {
				rectCampaign = GameObject.Find("Canvas_Campaign").GetComponent<RectTransform>();
				rectCustom = GameObject.Find("Canvas_CustomLevels").GetComponent<RectTransform>();
				centralToMainMenu = GameObject.Find("Return_To_Menu");
				campaignButton = GameObject.Find("Campaign_Button");
				customButton = GameObject.Find("Custom_Button");
				buyUpgradesSceneButton = GameObject.Find("Buy_Upgrades");
				return;
			}
			case 3: {

				return;
			}
			case 4: {

				return;
			}
			case 5: {
				maxSizeSlider = GameObject.Find("Slider").GetComponent<Slider>();
				return;
			}
			case 6: {
				resultingJudgdement = GameObject.Find("Result").GetComponent<TextMeshProUGUI>();
				didDominate = GameObject.Find("Domination").GetComponent<TextMeshProUGUI>();
				totalTimeToClear = GameObject.Find("Time").GetComponent<TextMeshProUGUI>();
				totalCoinsAwarded = GameObject.Find("Total_Coins_Awarded").GetComponent<TextMeshProUGUI>();
				return;
			}
			case 8: {
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
