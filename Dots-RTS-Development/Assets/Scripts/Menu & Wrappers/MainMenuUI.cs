using UnityEngine;

public class MainMenuUI : MonoBehaviour {

	// Turns OFF the game
	public void ExitGame() {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
	}

	public void DisplaySelection(bool isCampaign) {
		if (isCampaign) {
			//isDisplayingCampaign = true;
			UI_ReferenceHolder.rectCustom.anchoredPosition = new Vector3(2048, 0);
			UI_ReferenceHolder.rectCampaign.anchoredPosition = new Vector3(0, 0);

		}
		else {
			//isDisplayingCampaign = false;
			UI_ReferenceHolder.rectCampaign.anchoredPosition = new Vector3(-2048, 0);
			UI_ReferenceHolder.rectCustom.anchoredPosition = new Vector3(0, 0);
		}
		UI_ReferenceHolder.canvasBase.SetActive(false);
		//UI_ReferenceHolder.centralToMainMenu.SetActive(false);
		//UI_ReferenceHolder.campaignButton.SetActive(false);
		//UI_ReferenceHolder.customButton.SetActive(false);
		//UI_ReferenceHolder.buyUpgradesSceneButton.SetActive(false);

	}

	public void ReturnToDefaultScreen() {
		UI_ReferenceHolder.rectCampaign.anchoredPosition = new Vector3(-2048, 0);
		UI_ReferenceHolder.rectCustom.anchoredPosition = new Vector3(2048, 0);
		UI_ReferenceHolder.canvasBase.SetActive(true);
		//UI_ReferenceHolder.centralToMainMenu.SetActive(true);
		//UI_ReferenceHolder.campaignButton.SetActive(true);
		//UI_ReferenceHolder.customButton.SetActive(true);
		//UI_ReferenceHolder.buyUpgradesSceneButton.SetActive(true);
	}
}