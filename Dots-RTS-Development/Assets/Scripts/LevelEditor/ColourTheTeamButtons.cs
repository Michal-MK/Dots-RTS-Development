using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColourTheTeamButtons : MonoBehaviour {

	public TextMeshProUGUI text;

	// Use this for initialization
	void Start() {
		int team = int.Parse(gameObject.name[6].ToString());
		gameObject.GetComponent<Image>().color = CellColours.GetColor((Team)team);
		text.color = CellColours.GetContrastColor((Team)team);
	}

	public static string GetDescriptionBasedOnTeam(Team team) {

		switch (team) {
			case Team.NEUTRAL: {
				return "Neutral";
			}
			case Team.ALLIED: {
				return "Ally";
			}
			default: {
				return "Enemy";
			}
		}
	}
}
