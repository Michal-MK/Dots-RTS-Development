using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelectScript : MonoBehaviour {

	public int team;


	public GameObject placeCellPanel;
	public GameObject teamButtonsPanel;
	public Text teamSBText;
	public Image teamSB_IMG;
	// This means the player wants to choose a new team
	public void TeamSBPress () {
		//I want to hide the panel, turn on team buttons
		placeCellPanel.SetActive(false);
		teamButtonsPanel.SetActive(true);
	}
	
	// This mean he is done selecting
	public void TeamSelectedButton (int correspondingTeam = 0) {
		//I want to tell the LECore that the team value has changed, turn off the buttons and turn on the panel
		team = correspondingTeam;
		gameObject.SendMessage("GetPlaceCellPanelValues");
		placeCellPanel.SetActive(true);
		teamButtonsPanel.SetActive(false);

	}


	public void UpdateButtonVisual() {
		if (LevelEditorCore.team == 0) {
			teamSBText.text = "Neutral";
			teamSBText.color = Color.black;
			teamSB_IMG.color = Cell.neutral;
		}
		else if (LevelEditorCore.team == 1) {
			teamSBText.text = "Ally";
			teamSBText.color = Color.black;
			teamSB_IMG.color = Cell.ally;
		}
		else if (LevelEditorCore.team == 2) {
			teamSBText.text = "Enemy";
			teamSBText.color = Color.black;
			teamSB_IMG.color = Cell.enemy1;
		}
		else {
			//might eventually implement allies
			teamSBText.text = "Enemy";
			if (LevelEditorCore.team == 3) {
				teamSBText.color = Color.white;
				teamSB_IMG.color = Cell.enemy2;
			}
			else if (LevelEditorCore.team == 4) {
				teamSBText.color = Color.black;
				teamSB_IMG.color = Cell.enemy3;
			}
			else if (LevelEditorCore.team == 5) {
				teamSBText.color = Color.white;
				teamSB_IMG.color = Cell.enemy4;
			}
			else if (LevelEditorCore.team == 6) {
				teamSBText.color = Color.white;
				teamSB_IMG.color = Cell.enemy5;
			}
			else if (LevelEditorCore.team == 7) {
				teamSBText.color = Color.black;
				teamSB_IMG.color = Cell.enemy6;
			}
			else if (LevelEditorCore.team == 8) {
				teamSBText.color = Color.white;
				teamSB_IMG.color = Cell.enemy7;
			}
			else if (LevelEditorCore.team == 9) {
				teamSBText.color = Color.black;
				teamSB_IMG.color = Cell.enemy8;
			}
		}
	}
}
