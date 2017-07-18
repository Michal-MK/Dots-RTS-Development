using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WholeScreenPanelManager : MonoBehaviour {

	public enum panelsId { NoPanel, GSPanel, IOPanel };
	public static Image gSPanelChekmark;
	public static Image iOPanelChekmark;
	public static GameObject gSPanel;
	public static GameObject iOPanel;
	static panelsId _panelOn = panelsId.NoPanel;
	static bool alreadyIs;


	private void Start() {
		

		gSPanel = GameObject.Find("GameSettingsPanel");
		iOPanel = GameObject.Find("IOHugePanel");

		gSPanelChekmark = GameObject.Find("OpenGameSettingsPanel").transform.GetChild(1).GetComponent<Image>();
		iOPanelChekmark = GameObject.Find("OpenIOPanel").transform.GetChild(1).GetComponent<Image>();
	}
	
	public void ButtonInteract (int id) {
		if (_panelOn == (panelsId)id) {
			alreadyIs = true;
		}
		panelOn = (panelsId)id;
	} 

	static void Refresh() {
		if (alreadyIs) {
			_panelOn = panelsId.NoPanel;
			alreadyIs = false;
		}
		if (_panelOn == panelsId.NoPanel) {
			gSPanel.SetActive(false);
			iOPanel.SetActive(false);

			gSPanelChekmark.enabled = false;
			iOPanelChekmark.enabled = false;
		}
		else if (_panelOn == panelsId.IOPanel) {
			gSPanel.SetActive(false);
			iOPanel.SetActive(true);

			gSPanelChekmark.enabled = false;
			iOPanelChekmark.enabled = true;
		}
		else if (_panelOn == panelsId.GSPanel) {
			gSPanel.SetActive(true);
			iOPanel.SetActive(false);

			gSPanelChekmark.enabled = true;
			iOPanelChekmark.enabled = false;
		}
	}

	public static panelsId panelOn {
		get { return _panelOn; }
		set { _panelOn = value; Refresh(); }
	}

}
