using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Conversions;

public class TeamSetup : MonoBehaviour {

	public LevelEditorCore core;
	public GameObject teamGOPrefab;
	public RectTransform leaveClanButtonGo;
	public RectTransform roundTable;
	public GameObject clanBG;
	public RectTransform panel;

	private List<TeamBox> teamBoxes = new List<TeamBox>();
	private List<GameObject> bgList = new List<GameObject>();
	public Dictionary<Team, AIHolder> clanDict = new Dictionary<Team, AIHolder>();

	private float diffAngle;
	[HideInInspector]
	public float roundTableR;

	#region Dictionary debug tool
	//Dictionary<Cell.enmTeam, AIHolder>.KeyCollection keys = newDict.Keys;
	//	foreach (Cell.enmTeam team in keys) {
	//		string s = "";
	//AIHolder hold = new AIHolder();
	//newDict.TryGetValue(team, out hold);
	//		foreach(Cell.enmTeam t in hold.targets) {
	//			s += (" " + t + " ");

	//		}
	//		print(team + " has targets:" + s );
			
	//	}
	#endregion

	void OnEnable() {
		print(GameObject.Find("Canvas").GetComponent<Canvas>().scaleFactor); 
		roundTableR = ((roundTable.rect.width * 0.5f)) - teamGOPrefab.GetComponent<RectTransform>().sizeDelta.x;


		for (int i = 0; i < core.teamList.Count; i++) {

			GameObject newTeamBox = Instantiate(teamGOPrefab, roundTable.transform);
			//print(newTeamBox.transform.localPosition);
			newTeamBox.GetComponent<Image>().color = CellColours.GetColor(core.teamList[i]);
			teamBoxes.Add(newTeamBox.GetComponent<TeamBox>());
			teamBoxes[i].team = core.teamList[i];
			teamBoxes[i].myParrent = gameObject.GetComponent<TeamSetup>();
		}

		diffAngle = (2 * Mathf.PI) / teamBoxes.Count;
		float nextAngle = 0;

		for (int i = 0; i < teamBoxes.Count; i++) {
			teamBoxes[i].transform.localPosition = BasicConversions.PolarToCartesian(nextAngle, roundTableR);

			teamBoxes[i].myAngle = (nextAngle);
			nextAngle += diffAngle;
			teamBoxes[i].AllThingsSet();
		}
		UpdateRoundTableVisuals();

		//PASS
		foreach (KeyValuePair<Team,AIHolder> item in clanDict) {
			print(item.Key + " " + item.Value);
		}

	}
	public void OnDisable() {
		for (int i = 0; i < teamBoxes.Count; i++) {
			Destroy(teamBoxes[i].gameObject);
		}
		teamBoxes.Clear();
	}

	public void TeamBoxPosChange(Vector2 pos, TeamBox teamBox) {
		float lowestDistFound = Mathf.Infinity;
		Team indexClosest = Team.NEUTRAL;
		teamBox.transform.position = teamBox.initialPos;

		for (int i = 0; i < teamBoxes.Count; i++) {          //Debug.Log(i);
			float dist = Vector2.Distance(teamBoxes[i].initialPos, pos);
			if (dist < lowestDistFound) {
				lowestDistFound = dist;
				indexClosest = teamBoxes[i].team;
			}
		}
		float distToX = Vector2.Distance(leaveClanButtonGo.transform.position, pos);
		if (distToX < lowestDistFound) {
			RemoveFromClan(teamBox.team);
			return;
		}
		if (indexClosest == teamBox.team) {
			return;
		}
		RemoveFromClan(teamBox.team);
		CreateAClan(teamBox.team, indexClosest);
	}

	public void RemoveFromClan(Team firstTeam) {
		print("Removing a clan " + firstTeam);

		Dictionary<Team, AIHolder>.KeyCollection keys = clanDict.Keys;

		Dictionary<Team, AIHolder> changes = new Dictionary<Team, AIHolder>();

		foreach (Team j in keys) {
			print(j + " Key");

			AIHolder alliesOfJ;
			clanDict.TryGetValue(j, out alliesOfJ);
			if (alliesOfJ.allies.Contains(firstTeam)) {
				alliesOfJ.allies.Remove(firstTeam);

				changes.Add(j, alliesOfJ);
			}

		}

		Dictionary<Team, AIHolder>.KeyCollection changesKeys = changes.Keys;
		foreach (Team k in changesKeys) {
			AIHolder value;
			changes.TryGetValue(k, out value);

			clanDict.Remove(k);

			if (value.allies.Count > 0) {
				clanDict.Add(k, value);
			}
		}
		clanDict.Remove(firstTeam);
		UpdateRoundTableVisuals();

	}

	void CreateAClan(Team firstTeam, Team secondTeam) {
		print("Creating a clan " + firstTeam + " " + secondTeam);
		BindTwo(firstTeam, secondTeam);
		BindTwo(secondTeam, firstTeam);
		CheckAndAddOtherTeams(firstTeam, secondTeam);
		// MoveTeamBoxToLeftOf(mySpawns[MySpawnsIndexFromTeam(firstTeam)], mySpawns[MySpawnsIndexFromTeam(secondTeam)]);
		UpdateRoundTableVisuals();

	}
	void BindTwo(Team first, Team second) {
		AIHolder tempAllies = new AIHolder();
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

	

	void CheckAndAddOtherTeams(Team firstTeam, Team secondTeam) {
		AIHolder firstTeamFriends;
		clanDict.TryGetValue(firstTeam, out firstTeamFriends);

		AIHolder secondTeamFriends;
		clanDict.TryGetValue(secondTeam, out secondTeamFriends);

		List<Team> misses = new List<Team>();
		for (int y = 0; y < secondTeamFriends.allies.Count; y++) {
			if (!firstTeamFriends.allies.Contains(secondTeamFriends.allies[y])) {
				misses.Add(secondTeamFriends.allies[y]);
			}
		}
		foreach (Team miss in misses) {
			if (miss != firstTeam && miss != secondTeam) {
				CreateAClan(firstTeam, miss);
			}
		}

		misses.Clear();
		for (int x = 0; x < firstTeamFriends.allies.Count; x++) {
			if (!secondTeamFriends.allies.Contains(firstTeamFriends.allies[x])) {
				misses.Add(firstTeamFriends.allies[x]);
				//print("Miss is " + firstTeamFriendsList[x]);
			}
		}
		foreach (Team miss in misses) {
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
		
		List<TeamBox> newList = new List<TeamBox>(teamBoxes);

		float angle = 0;

		
		List<List<Team>> actualClans = BasicConversions.CDToActualClans(clanDict);
		for (int i = 0; i < actualClans.Count; i++) {
			GameObject bg = Instantiate(clanBG, roundTable);
			bgList.Add(bg);
			Image img = bg.GetComponent<Image>();
			img.color = CellColours.GetColor((Team)(i + 2));
			RectTransform rt = bg.GetComponent<RectTransform>();
			rt.anchorMax = Vector2.one;
			rt.anchorMin = Vector2.zero;
			rt.localPosition = Vector3.zero;
			rt.SetAsFirstSibling();
			float lastTeamBoxAngle = 0;

			foreach (Team j in actualClans[i]) {


				foreach (TeamBox t in teamBoxes) {
					if (t.team == j) {
						//Debug.Log(t.team);
						t.transform.localPosition = BasicConversions.PolarToCartesian(angle, roundTableR);
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
		foreach (TeamBox t in newList) {
			t.transform.localPosition = BasicConversions.PolarToCartesian(angle, roundTableR);
			t.myAngle = angle;
			t.AllThingsSet();
			angle += diffAngle;
		}


	}


	public static bool AlreadyIsInAllyHolder(AIHolder list, Team value) {
		for (int i = 0; i < list.allies.Count; i++) {
			if (list.allies[i] == value) {
				return true;
			}
		}
		return false;
	}



	public int MySpawnsIndexFromTeam(Team team) {
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

	public Dictionary<Team,AIHolder> DictWithAllInfo() {
		Dictionary<Team, AIHolder> newDict = new Dictionary<Team, AIHolder>();
		Dictionary<Team, AIHolder>.KeyCollection TeamKeys = clanDict.Keys;
		List<Team> noClaners = new List<Team>(core.teamList);
		foreach (Team team in TeamKeys) {
			noClaners.Remove(team);
			AIHolder holder = new AIHolder();
			if(clanDict.TryGetValue(team, out holder) == false) {
				Debug.LogError("This can't happen");
				clanDict.Add(team, holder);
			}
			holder.targets = new List<Team>(core.teamList);
			//print(" targets start " + holder.targets.Count);
			//print(" allies " + holder.allies.Count);
			holder.targets.Remove(team);
			foreach (Team ally in holder.allies) {
				holder.targets.Remove(ally);
				//print("this runs this time");
			}
			//print("targets ends at " + holder.targets.Count);
			if (holder.targets.Count + holder.allies.Count != core.teamList.Count - 1) {
				Debug.LogError("Targets&Allies don't add up to AllTeams");
			}

			newDict.Add(team, holder);
			
		}
		foreach (Team team in noClaners) {
			AIHolder newAiHolder = new AIHolder();
			newAiHolder.targets = new List<Team>(core.teamList);
			newAiHolder.targets.Remove(team);
			newDict.Add(team, newAiHolder);
		}
		
		
		return newDict;

		
	} 
}

[System.Serializable]
public class AIHolder {
	public List<Team> allies = new List<Team>();
	public List<Team> targets = new List<Team>();
}