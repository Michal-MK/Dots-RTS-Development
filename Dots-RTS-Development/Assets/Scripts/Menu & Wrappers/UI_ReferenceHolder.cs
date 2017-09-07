using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class UI_ReferenceHolder : MonoBehaviour {

	#region References

	#region MainMenuRefs
	public static TextMeshProUGUI MM_gameName;
	public static TextMeshProUGUI MM_gameVersion;
	public static GameObject MM_startGameButton;
	public static GameObject MM_levelEditorButton;
	public static GameObject MM_onlineLevelsButton;
	public static TextMeshProUGUI MM_profileName;
	public static GameObject MM_quitGameButton;

	#endregion

	#region EditorRefs
	public static GameObject LE_saveLoadTryLevel;
	public static GameObject LE_saveInfoPanel;
	public static GameObject LE_gameSettingsPanel;
	public static GameObject LE_teamPickerPanel;
	public static GameObject LE_loadForEditPanel;
	public static GameObject LE_editorSettingsPanel;
	//public static GameObject menuPanel;

	#endregion

	#region LevelSelectRefs
	public static RectTransform LS_rectCampaign;
	public static RectTransform LS_rectCustom;
	public static GameObject LS_canvasBase;
	#endregion

	#region LevelPlayerRefs
	public static GameObject MULTI_menuPanel;
	public static RectTransform MULTI_upgradePanel;
	public static TextMeshProUGUI LP_TypeUpgradeText;
	public static Image LP_TypeUpgradeImage;
	#endregion

	#region LevelShareRefs

	#endregion

	#region DebugRefs
	//public static GameObject menuPanel;
	//public static RectTransform upgradePanel;

	#endregion

	#region PostGame
	public static TextMeshProUGUI PG_resultingJudgdement;
	public static TextMeshProUGUI PG_didDominate;
	public static TextMeshProUGUI PG_totalTimeToClear;
	public static TextMeshProUGUI PG_totalCoinsAwarded;
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
	public static Button U_buyButton;
	public static TextMeshProUGUI U_upgradeNameHolder;
	public static TextMeshProUGUI U_upgradeDescHolder;
	public static TextMeshProUGUI U_upgradeCostHolder;
	public static TextMeshProUGUI U_upgradesOwnedHolder;
	public static TextMeshProUGUI U_profileMoney;
	public static TextMeshProUGUI U_profileNameUpgradeStore;
	#endregion
	#endregion

	void Start() {
		SceneManager_activeSceneChanged(SceneManager.GetActiveScene(), SceneManager.GetActiveScene());
		SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
	}



	private void SceneManager_activeSceneChanged(Scene oldS, Scene newS) {

		switch (newS.name) {
			case Scenes.MENU: { //Main Menu

				return;
			}
			case Scenes.EDITOR: { // Editor
				Transform c = GameObject.Find("Canvas").transform;
				LE_editorSettingsPanel = c.Find("ViewMenuPanel").gameObject;
				LE_gameSettingsPanel = c.Find("GameSettingsPanel").gameObject;
				LE_loadForEditPanel = c.Find("LoadPanel").gameObject;
				LE_saveInfoPanel = c.Find("SavePanel").gameObject;
				LE_saveLoadTryLevel = c.Find("SaveOrLoad").gameObject;
				LE_teamPickerPanel = c.Find("TeamSelectPanel").gameObject;
				MULTI_menuPanel = c.Find("MenuPanel").gameObject;
				return;
			}
			case Scenes.SELECT: { // LevelSelect
				LS_rectCampaign = GameObject.Find("Canvas_Campaign").GetComponent<RectTransform>();
				LS_rectCustom = GameObject.Find("Canvas_CustomLevels").GetComponent<RectTransform>();
				LS_canvasBase = GameObject.Find("Canvas_Base");
				return;
			}
			case Scenes.PLAYER: { //PlayScene
				MULTI_menuPanel = GameObject.Find("Canvas").transform.Find("MenuPanel").gameObject;
				MULTI_upgradePanel = GameObject.Find("CanvasCamera").transform.Find("UPGRADE_Panel").GetComponent<RectTransform>();
				LP_TypeUpgradeImage = MULTI_upgradePanel.transform.Find("UPGRADE_Types/TYPE_Holder/TYPE_Image").GetComponent<Image>();
				LP_TypeUpgradeText = MULTI_upgradePanel.transform.Find("UPGRADE_Types/TYPE_Holder/TYPE_Name").GetComponent<TextMeshProUGUI>();				
				return;
			}
			case Scenes.SHARING: { //Level Sharing

				return;
			}
			case Scenes.DEBUG: { // DebugScene
				MULTI_menuPanel = GameObject.Find("Canvas").transform.Find("MenuPanel").gameObject;
				MULTI_upgradePanel = GameObject.Find("CanvasCamera").transform.Find("Upgrade_Panel").GetComponent<RectTransform>();
				return;
			}
			case Scenes.POSTG: { // PostGame Scene
				PG_resultingJudgdement = GameObject.Find("Result").GetComponent<TextMeshProUGUI>();
				PG_didDominate = GameObject.Find("Domination").GetComponent<TextMeshProUGUI>();
				PG_totalTimeToClear = GameObject.Find("Time").GetComponent<TextMeshProUGUI>();
				PG_totalCoinsAwarded = GameObject.Find("Total_Coins_Awarded").GetComponent<TextMeshProUGUI>();
				return;
			}
			case Scenes.PROFILES: { //Profiles
				PS_Canvas = GameObject.Find("Canvases").transform.Find("PS_Canvas").gameObject;
				PO_Canvas = GameObject.Find("Canvases").transform.Find("PO_Canvas").gameObject;

				PO_Name = PO_Canvas.transform.Find("PO_Name").GetComponent<TextMeshProUGUI>();

				PO_OnLevel = PO_Canvas.transform.Find("TopLeft_Panel_CampaignLevel/PO_OnLevel").GetComponent<TextMeshProUGUI>();					/*GameObject.Find("PO_OnLevel").GetComponent<TextMeshProUGUI>();*/
				PO_OnLevelImage = PO_Canvas.transform.Find("TopLeft_Panel_CampaignLevel/PO_OnLevelImage").GetComponent<RawImage>();					/*GameObject.Find("PO_OnLevelImage").GetComponent<RawImage>();*/

				PO_CurrentCoins = PO_Canvas.transform.Find("TopRight_Panel_GeneralInfo/PO_CurrentCoins").GetComponent<TextMeshProUGUI>();			/*GameObject.Find("PO_CurrentCoins").GetComponent<TextMeshProUGUI>();*/
				PO_GamesPlayed = PO_Canvas.transform.Find("TopRight_Panel_GeneralInfo/PO_GamesPlayed").GetComponent<TextMeshProUGUI>();				/*GameObject.Find("PO_GamesPlayed").GetComponent<TextMeshProUGUI>();*/

				PO_AcquiredUpgrades = PO_Canvas.transform.Find("BottomLeft_Panel_Uprades/PO_AcquiredUpgrades").transform;                           /*GameObject.Find("PO_AcquiredUpgrades").transform;*/

				PO_DeleteProfile = PO_Canvas.transform.Find("BottomRight_Panel_Buttons/PO_DeleteProfile").GetComponent<ProfileInfo>();				/*GameObject.Find("PO_DeleteProfile").GetComponent<ProfileInfo>();*/
				return;
			}
			case Scenes.SHOP: { //Upgrade Shop
				U_buyButton = GameObject.Find("Buy").GetComponent<Button>();
				U_upgradeNameHolder = GameObject.Find("Upgrade_Name").GetComponent<TextMeshProUGUI>();
				U_upgradeDescHolder = GameObject.Find("Upgrade_Desc").GetComponent<TextMeshProUGUI>();
				U_upgradeCostHolder = GameObject.Find("Upgrade_Cost_Money").GetComponent<TextMeshProUGUI>();
				U_upgradesOwnedHolder = GameObject.Find("Already_Owned_Count").GetComponent<TextMeshProUGUI>();
				U_profileNameUpgradeStore = GameObject.Find("Profile_Name").GetComponent<TextMeshProUGUI>();
				U_profileMoney = GameObject.Find("Coins_To_Spend").GetComponent<TextMeshProUGUI>();

				return;
			}
		}
	}
}
