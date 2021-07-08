using System;
using System.Collections.Generic;
using System.Linq;
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
	[HideInInspector] public float roundTableR;

	private void OnEnable() {
		print(GameObject.Find("Canvas").GetComponent<Canvas>().scaleFactor);
		roundTableR = ((roundTable.rect.width * 0.5f)) - teamGOPrefab.GetComponent<RectTransform>().sizeDelta.x;


		for (int i = 0; i < core.teamList.Count; i++) {

			GameObject newTeamBox = Instantiate(teamGOPrefab, roundTable.transform);

			//print(newTeamBox.transform.localPosition);
			newTeamBox.GetComponent<Image>().color = CellColours.GetColor(core.teamList[i]);
			teamBoxes.Add(newTeamBox.GetComponent<TeamBox>());
			teamBoxes[i].team = core.teamList[i];
			teamBoxes[i].myParent = gameObject.GetComponent<TeamSetup>();
		}

		diffAngle = (2 * Mathf.PI) / teamBoxes.Count;
		float nextAngle = 0;

		foreach (TeamBox t in teamBoxes) {
			t.transform.localPosition = BasicConversions.PolarToCartesian(nextAngle, roundTableR);

			t.myAngle = nextAngle;
			nextAngle += diffAngle;
			t.AllThingsSet();
		}
		UpdateRoundTableVisuals();

		//PASS
		foreach (KeyValuePair<Team, AIHolder> item in clanDict) {
			print(item.Key + " " + item.Value);
		}

	}

	public void OnDisable() {
		foreach (TeamBox t in teamBoxes) {
			Destroy(t.gameObject);
		}
		teamBoxes.Clear();
	}

	public void TeamBoxPosChange(Vector2 pos, TeamBox teamBox) {
		float lowestDistFound = Mathf.Infinity;
		Team indexClosest = Team.Neutral;
		teamBox.transform.position = teamBox.initialPos;

		foreach (TeamBox t in teamBoxes) {
			float dist = Vector2.Distance(t.initialPos, pos);
			if (dist < lowestDistFound) {
				lowestDistFound = dist;
				indexClosest = t.team;
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
		CreateClan(teamBox.team, indexClosest);
	}

	public void RemoveFromClan(Team firstTeam) {
		print("Removing a clan " + firstTeam);

		Dictionary<Team, AIHolder>.KeyCollection keys = clanDict.Keys;

		Dictionary<Team, AIHolder> changes = new Dictionary<Team, AIHolder>();

		foreach (Team j in keys) {
			print(j + " Key");

			clanDict.TryGetValue(j, out AIHolder alliesOfJ);
			if (alliesOfJ.allies.Contains(firstTeam)) {
				alliesOfJ.allies.Remove(firstTeam);

				changes.Add(j, alliesOfJ);
			}
		}

		Dictionary<Team, AIHolder>.KeyCollection changesKeys = changes.Keys;
		foreach (Team k in changesKeys) {
			changes.TryGetValue(k, out AIHolder value);

			clanDict.Remove(k);

			if (value.allies.Count > 0) {
				clanDict.Add(k, value);
			}
		}
		clanDict.Remove(firstTeam);
		UpdateRoundTableVisuals();

	}

	private void CreateClan(Team firstTeam, Team secondTeam) {
		print("Creating a clan " + firstTeam + " " + secondTeam);
		BindTwo(firstTeam, secondTeam);
		BindTwo(secondTeam, firstTeam);
		CheckAndAddOtherTeams(firstTeam, secondTeam);

		// MoveTeamBoxToLeftOf(mySpawns[MySpawnsIndexFromTeam(firstTeam)], mySpawns[MySpawnsIndexFromTeam(secondTeam)]);
		UpdateRoundTableVisuals();

	}

	private void BindTwo(Team first, Team second) {
		AIHolder tempAllies = new AIHolder();
		if (clanDict.ContainsKey(first)) {

			clanDict.TryGetValue(first, out tempAllies);

			if (!tempAllies.allies.Any(t => t == second)) {
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

	private void CheckAndAddOtherTeams(Team firstTeam, Team secondTeam) {
		clanDict.TryGetValue(firstTeam, out AIHolder firstTeamFriends);
		clanDict.TryGetValue(secondTeam, out AIHolder secondTeamFriends);

		List<Team> misses = new List<Team>();
		foreach (Team t in secondTeamFriends.allies) {
			if (!firstTeamFriends.allies.Contains(t)) {
				misses.Add(t);
			}
		}
		foreach (Team miss in misses) {
			if (miss != firstTeam && miss != secondTeam) {
				CreateClan(firstTeam, miss);
			}
		}

		misses.Clear();
		foreach (Team t in firstTeamFriends.allies) {
			if (!secondTeamFriends.allies.Contains(t)) {
				misses.Add(t);

				//print("Miss is " + firstTeamFriendsList[x]);
			}
		}
		foreach (Team miss in misses) {
			//Debug.Log("like This: " + miss + " " + secondTeam);
			if (miss != secondTeam && miss != firstTeam) {
				CreateClan(miss, secondTeam);
			}
		}
	}

	private static void DestroyAllInList(List<GameObject> list) {
		foreach (GameObject go in list) {
			Destroy(go);
		}
	}

	private void UpdateRoundTableVisuals() {

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
				foreach (TeamBox t in teamBoxes.Where(t => t.team == j)) {
					t.transform.localPosition = BasicConversions.PolarToCartesian(angle, roundTableR);
					t.myAngle = angle;
					t.AllThingsSet();
					img.fillAmount += 1f / teamBoxes.Count;
					lastTeamBoxAngle = Mathf.Rad2Deg * (angle + diffAngle / 2);
					newList.Remove(t);
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

	public Dictionary<Team, AIHolder> DictWithAllInfo() {
		Dictionary<Team, AIHolder> newDict = new Dictionary<Team, AIHolder>();
		Dictionary<Team, AIHolder>.KeyCollection teamKeys = clanDict.Keys;
		List<Team> noClaners = new List<Team>(core.teamList);
		foreach (Team team in teamKeys) {
			noClaners.Remove(team);
			clanDict.TryGetValue(team, out AIHolder holder);
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

[Serializable]
public class AIHolder {
	public List<Team> allies = new List<Team>();
	public List<Team> targets = new List<Team>();
}
