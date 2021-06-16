using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColourTheTeamButtons : MonoBehaviour {

	public TextMeshProUGUI text;

	// Use this for initialization
	private void Start() {
		int team = int.Parse(gameObject.name[6].ToString());
		gameObject.GetComponent<Image>().color = CellColours.GetColor((Team)team);
		text.color = CellColours.GetContrastColor((Team)team);
	}

	public static string GetDescriptionBasedOnTeam(Team team) {
		return team switch {
			Team.NEUTRAL => "Neutral",
			Team.ALLIED  => "Ally",
			_            => "Enemy"
		};
	}
}
