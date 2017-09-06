using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSetup : MonoBehaviour {

	public LevelEditorCore core;
	public GameObject teamGOPrefab;
	public RectTransform leaveClanButtonGo;
	public RectTransform roundTable;
	public GameObject clanBG;
	public RectTransform panel;

	private List<TeamBox> teamBoxes = new List<TeamBox>();
	private List<GameObject> bgList = new List<GameObject>();
	public Dictionary<Cell.enmTeam, AllyHolder> clanDict = new Dictionary<Cell.enmTeam, AllyHolder>();

	private float diffAngle;
	[HideInInspector]
	public float roundTableR;






	void OnEnable() {
		roundTableR = (roundTable.anchorMax.x - roundTable.anchorMin.x) *( panel.anchorMax.x - panel.anchorMin.x ) * Screen.currentResolution.width * 0.5f;
		
		print(Screen.currentResolution.width);
		for (int i = 0; i < core.teamList.Count; i++) {
			GameObject newTeamBox = Instantiate(teamGOPrefab, roundTable.transform);
			newTeamBox.GetComponent<Image>().color = ColourTheTeamButtons.GetColorBasedOnTeam(core.teamList[i]);
			teamBoxes.Add(newTeamBox.GetComponent<TeamBox>());
			teamBoxes[i].team = core.teamList[i];
			teamBoxes[i].myParrent = gameObject.GetComponent<TeamSetup>();
		}

		diffAngle = (2 * Mathf.PI) / teamBoxes.Count;
		float nextAngle = 0;

		for (int i = 0; i < teamBoxes.Count; i++) {
			teamBoxes[i].transform.position = AngleToPos(nextAngle);
			teamBoxes[i].myAngle = (nextAngle);
			nextAngle += diffAngle;
			teamBoxes[i].AllThingsSet();
		}
		UpdateRoundTableVisuals();
	}
	public void OnDisable() {
		for (int i = 0; i < teamBoxes.Count; i++) {
			Destroy(teamBoxes[i].gameObject);
		}
		teamBoxes.Clear();
	}

	private void OnDestroy() {
		clanDict.Clear();
	}

	// Update is called once per frame
	public void TeamBoxPosChange(Vector2 pos, TeamBox it) {
		float lowestDisFound = Mathf.Infinity;
		Cell.enmTeam indexClosest = Cell.enmTeam.NEUTRAL;
		it.transform.position = it.initialPos;

		for (int i = 0; i < teamBoxes.Count; i++) {          //Debug.Log(i);
			float dist = Vector2.Distance(teamBoxes[i].initialPos, pos);
			if (dist < lowestDisFound) {
				lowestDisFound = dist;
				indexClosest = teamBoxes[i].team;
			}
		}
		float distToX = Vector2.Distance(leaveClanButtonGo.transform.position, pos);
		if (distToX < lowestDisFound) {
			RemoveFromClan(it.team);
			return;
		}
		if (indexClosest == it.team) {
			return;
		}
		RemoveFromClan(it.team);
		CreateAClan(it.team, indexClosest);

	}

	public void RemoveFromClan(Cell.enmTeam firstTeam) {
		Dictionary<Cell.enmTeam, AllyHolder>.KeyCollection keys = clanDict.Keys;

		Dictionary<Cell.enmTeam, AllyHolder> changes = new Dictionary<Cell.enmTeam, AllyHolder>();

		foreach (Cell.enmTeam j in keys) {
			//print(j + " Key");
			AllyHolder alliesOfJ;
			clanDict.TryGetValue(j, out alliesOfJ);
			List<Cell.enmTeam> alliesOfJList = alliesOfJ.allies;
			if (alliesOfJList.Contains(firstTeam)) {
				alliesOfJList.Remove(firstTeam);
				changes.Add(j, alliesOfJ);

			}

		}

		Dictionary<Cell.enmTeam, AllyHolder>.KeyCollection changesKeys = changes.Keys;
		foreach (Cell.enmTeam k in changesKeys) {
			AllyHolder value;
			changes.TryGetValue(k, out value);

			clanDict.Remove(k);

			if (value.allies.Count > 0) {
				clanDict.Add(k, value);
			}
		}

		clanDict.Remove(firstTeam);
		UpdateRoundTableVisuals();
	}

	void CreateAClan(Cell.enmTeam firstTeam, Cell.enmTeam secondTeam) {

		BindTwo(firstTeam, secondTeam);
		BindTwo(secondTeam, firstTeam);
		CheckAndAddOtherTeams(firstTeam, secondTeam);
		// MoveTeamBoxToLeftOf(mySpawns[MySpawnsIndexFromTeam(firstTeam)], mySpawns[MySpawnsIndexFromTeam(secondTeam)]);
		UpdateRoundTableVisuals();

	}
	void BindTwo(Cell.enmTeam first, Cell.enmTeam second) {
		AllyHolder tempAllies = new AllyHolder();
		if (clanDict.ContainsKey(first)) {

			clanDict.TryGetValue(first, out tempAllies);

			if (!AlreadyIsInAllyHolder(tempAllies, second)) {
				tempAllies.allies.Add(second);

				clanDict.Remove(first);
				clanDict.Add(first, tempAllies);
			}

		}
		else {
			tempAllies.allies.Add(second);
			clanDict.Add(first, tempAllies);
		}
	}

	//public static List<int> IntToList(int input) {
	//	List<int> returnList = new List<int>();
	//	string sInput = input.ToString();
	//	for (int i = 0; i < sInput.Length; i++) {
	//		int value = int.Parse(sInput[i].ToString());
	//		returnList.Add(value);
	//	}

	//	return returnList;
	//}
	//public static int ListToInt(List<int> list) {
	//	int output = 0;
	//	if (list.Count == 0) {
	//		return 0;
	//	}
	//	foreach (int i in list) {
	//		output = output * 10;
	//		output += i;
	//	}
	//	return output;
	//}

	bool AlreadyIsInAllyHolder(AllyHolder list, Cell.enmTeam value) {
		for (int i = 0; i < list.allies.Count; i++) {
			if (list.allies[i] == value) {
				return true;
			}
		}
		return false;
	}
	void CheckAndAddOtherTeams(Cell.enmTeam firstTeam, Cell.enmTeam secondTeam) {
		AllyHolder firstTeamFriends;
		clanDict.TryGetValue(firstTeam, out firstTeamFriends);
		List<Cell.enmTeam> firstTeamFriendsList = firstTeamFriends.allies;

		AllyHolder secondTeamFriends;
		clanDict.TryGetValue(secondTeam, out secondTeamFriends);
		List<Cell.enmTeam> secondTeamFriendsList = secondTeamFriends.allies;

		List<Cell.enmTeam> misses = new List<Cell.enmTeam>();
		for (int y = 0; y < secondTeamFriendsList.Count; y++) {
			if (!firstTeamFriendsList.Contains(secondTeamFriendsList[y])) {
				misses.Add(secondTeamFriendsList[y]);
			}
		}
		foreach (Cell.enmTeam miss in misses) {
			if (miss != firstTeam && miss != secondTeam) {
				CreateAClan(firstTeam, miss);
			}
		}

		misses.Clear();
		for (int x = 0; x < firstTeamFriendsList.Count; x++) {
			if (!secondTeamFriendsList.Contains(firstTeamFriendsList[x])) {
				misses.Add(firstTeamFriendsList[x]);
				//print("Miss is " + firstTeamFriendsList[x]);
			}
		}
		foreach (Cell.enmTeam miss in misses) {
			//Debug.Log("like This: " + miss + " " + secondTeam);
			if (miss != secondTeam && miss != firstTeam) {
				CreateAClan(miss, secondTeam);
			}
		}
	}
	static void DestroyAllInList(List<GameObject> list) {
		for (int i = 0; i < list.Count; i++) {
			Destroy(list[i]);
		}
	}
	public void UpdateRoundTableVisuals() {

		DestroyAllInList(bgList);
		List<List<Cell.enmTeam>> actualClans = new List<List<Cell.enmTeam>>();

		Dictionary<Cell.enmTeam, AllyHolder>.KeyCollection keys = clanDict.Keys;
		foreach (Cell.enmTeam j in keys) {
			//print(j + " Key");
			AllyHolder value;
			clanDict.TryGetValue(j, out value);
			List<Cell.enmTeam> clanJ = value.allies;
			clanJ.Add(j);
			clanJ.Sort();


			bool newClan = true;
			foreach (List<Cell.enmTeam> clan in actualClans) {
				//print(" one clan with [0] = " + clan[0]);
				if (clanJ.Contains(clan[0])) {
					newClan = false;
					break;
				}

			}
			if (newClan) {
				actualClans.Add(clanJ);
			}

		}
		List<TeamBox> newList = new List<TeamBox>(teamBoxes);

		float angle = 0;


		for (int i = 0; i < actualClans.Count; i++) {
			GameObject bg = Instantiate(clanBG, roundTable);
			bgList.Add(bg);
			Image img = bg.GetComponent<Image>();
			img.color = ColourTheTeamButtons.GetColorBasedOnTeam((Cell.enmTeam)(i + 2));
			RectTransform rt = bg.GetComponent<RectTransform>();
			rt.sizeDelta = roundTable.sizeDelta;
			rt.SetAsFirstSibling();
			float lastTeamBoxAngle = 0;

			foreach (Cell.enmTeam j in actualClans[i]) {


				foreach (TeamBox t in teamBoxes) {
					if (t.team == j) {
						Debug.Log(t.team);
						t.transform.position = AngleToPos(angle);
						t.myAngle = angle;
						t.AllThingsSet();
						img.fillAmount += 1f / teamBoxes.Count;
						lastTeamBoxAngle = Mathf.Rad2Deg * (angle + diffAngle / 2);
						newList.Remove(t);
					}
				}
				angle += diffAngle;
			}
			rt.rotation = Quaternion.Euler(0, 0, -lastTeamBoxAngle);

		}

		foreach (List<Cell.enmTeam> clan in actualClans) {


		}
		foreach (TeamBox t in newList) {
			t.transform.position = AngleToPos(angle);
			t.myAngle = angle;
			t.AllThingsSet();
			angle += diffAngle;
		}


	}

	Vector2 AngleToPos(float angle) {
		return new Vector2(Mathf.Sin(angle) * (roundTableR), Mathf.Cos(angle) * (roundTableR)) + (Vector2)roundTable.position;
	}


	public int MySpawnsIndexFromTeam(Cell.enmTeam team) {
		int output = -1;
		for (int i = 0; i < teamBoxes.Count; i++) {
			TeamBox spawn = teamBoxes[i];
			if (spawn.team == team) {
				output = i;
				break;
			}
		}
		if (output == -1) {
			Debug.LogError("Nothing in mySpawns falls under team " + team);
		}
		return output;
	}
	public int MySpawnsIndexFromAngle(float RawAngle) {
		float angle = RawAngle;
		if (Mathf.Round(angle * 10) / 10 >= Mathf.Round(2 * Mathf.PI * 10) / 10) {
			angle -= 2 * Mathf.PI;
		}
		int output = -1;
		for (int i = 0; i < teamBoxes.Count; i++) {
			TeamBox spawn = teamBoxes[i];
			if (Mathf.Round(spawn.myAngle * 10) / 10 == Mathf.Round(angle * 10) / 10) {
				output = i;
				break;
			}
		}
		if (output == -1) {
			Debug.LogError("Nothing in teamBoxes has angle " + angle);
		}
		return output;
	}
}
public class AllyHolder {
	public List<Cell.enmTeam> allies = new List<Cell.enmTeam>();
}