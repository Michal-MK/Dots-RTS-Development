using UnityEditor;
using UnityEngine;

public class MainMenuUI : MonoBehaviour {

	// Turns OFF the game
	public void ExitGame() {
		if (GameEnvironment.IsEditor) {
#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
#endif
		}
		Application.Quit();
	}

	public void DisplaySelection(bool isCampaign) {
		if (isCampaign) {
			UI_ReferenceHolder.LS_rectCustom.anchoredPosition = new Vector3(2048, 0);
			UI_ReferenceHolder.LS_rectCampaign.anchoredPosition = new Vector3(0, 0);
		}
		else {
			UI_ReferenceHolder.LS_rectCampaign.anchoredPosition = new Vector3(-2048, 0);
			UI_ReferenceHolder.LS_rectCustom.anchoredPosition = new Vector3(0, 0);
		}

		UI_ReferenceHolder.LS_canvasBase.SetActive(false);
	}

	public void ReturnToDefaultScreen() {
		UI_ReferenceHolder.LS_rectCampaign.anchoredPosition = new Vector3(-2048, 0);
		UI_ReferenceHolder.LS_rectCustom.anchoredPosition = new Vector3(2048, 0);
		UI_ReferenceHolder.LS_canvasBase.SetActive(true);
	}
}