using UnityEngine;

public class MainMenuUI : MonoBehaviour {

	// Turns OFF the game
	public void ExitGame() {
		if (GameEnvironment.IsEditor) {
			UnityEditor.EditorApplication.isPlaying = false;
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
		//UI_ReferenceHolder.centralToMainMenu.SetActive(false);
		//UI_ReferenceHolder.campaignButton.SetActive(false);
		//UI_ReferenceHolder.customButton.SetActive(false);
		//UI_ReferenceHolder.buyUpgradesSceneButton.SetActive(false);

	}

	public void ReturnToDefaultScreen() {
		UI_ReferenceHolder.LS_rectCampaign.anchoredPosition = new Vector3(-2048, 0);
		UI_ReferenceHolder.LS_rectCustom.anchoredPosition = new Vector3(2048, 0);
		UI_ReferenceHolder.LS_canvasBase.SetActive(true);
		//UI_ReferenceHolder.centralToMainMenu.SetActive(true);
		//UI_ReferenceHolder.campaignButton.SetActive(true);
		//UI_ReferenceHolder.customButton.SetActive(true);
		//UI_ReferenceHolder.buyUpgradesSceneButton.SetActive(true);
	}
}