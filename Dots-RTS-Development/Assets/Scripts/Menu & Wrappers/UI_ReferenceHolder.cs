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

	#endregion

	#region LevelPlayerRefs

	#endregion

	#region LevelShareRefs

	#endregion

	#region DebugRefs
	public static Slider maxSizeSlider;
	#endregion

	#region ProfileRefs

	#endregion

	#region Upgrade Store
	public static Button buyButton;
	public static TextMeshProUGUI upgradeNameHolder;
	public static TextMeshProUGUI upgradeDescHolder;
	public static TextMeshProUGUI upgradeCostHolder;
	public static TextMeshProUGUI upgradesOwnedHolder;
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

				return;
			}
			case 8: {
				buyButton = GameObject.Find("Buy").GetComponent<Button>();
				upgradeNameHolder = GameObject.Find("Upgrade_Name").GetComponent<TextMeshProUGUI>();
				upgradeDescHolder = GameObject.Find("Upgrade_Desc").GetComponent<TextMeshProUGUI>();
				upgradeCostHolder = GameObject.Find("Upgrade_Cost_Money").GetComponent<TextMeshProUGUI>();
				upgradesOwnedHolder = GameObject.Find("Already_Owned_Count").GetComponent<TextMeshProUGUI>();
				return;
			}
		}
	}
}
