using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSetup : MonoBehaviour {

	public GameObject teamGOPrefab;
	public GameObject xGO;
	static List<TeamBox> mySpawns = new List<TeamBox>();
	public static Dictionary<int, int> clanDict = new Dictionary<int, int>();
	//LineRenderer lineRenderer;
	public RectTransform roundTable;
	static RectTransform stRoundTable;

	public GameObject clanBG;
	List<GameObject> bgList = new List<GameObject>();

	static float diffAngle;

	// Use this for initialization
	void OnEnable() {
		stRoundTable = roundTable;
		//lineRenderer = gameObject.GetComponent<LineRenderer>();
		AllAiDifficultyWriter.RedoText();
		//for (int i = 0; i < LevelEditorCore.teamList.Count; i++) {
		//	GameObject newTeamBox = Instantiate(teamGOPrefab, roundTable.transform);
		//	newTeamBox.GetComponent<Image>().color = ColourTheTeamButtons.GetColorBasedOnTeam((int)LevelEditorCore.teamList[i]);
		//	mySpawns.Add(newTeamBox.GetComponent<TeamBox>());
		//	mySpawns[i].team = (int)LevelEditorCore.teamList[i];
		//	mySpawns[i].myParrent = gameObject.GetComponent<TeamSetup>();
		//	mySpawns[i].panel = roundTable;
		//	mySpawns[i].r = (roundTable.sizeDelta.x / 2.2f) * (Screen.width / 1280f);
		//}
		diffAngle = (2 * Mathf.PI) / mySpawns.Count;
		float nextAngle = 0;
		//  print((roundTable.sizeDelta.x / 2) - 50);
		//*(Screen.width / 1280)

		for (int i = 0; i < mySpawns.Count; i++) {
			mySpawns[i].transform.position = AngleToPos(nextAngle);
			// mySpawns[i].transform.position = mySpawns[i].transform.position * (Screen.width / 1280f);
			mySpawns[i].myAngle = (nextAngle);
			nextAngle += diffAngle;
			mySpawns[i].AllThingsSet();
		}
		UpdateRoundTableVisuals();
	}
	public void OnDisable() {
		for (int i = 0; i < mySpawns.Count; i++) {
			Destroy(mySpawns[i].gameObject);
		}
		mySpawns.Clear();
	}

	private void OnDestroy() {
		clanDict.Clear();
	}

	// Update is called once per frame
	public void TeamBoxPosChange(Vector2 pos, TeamBox it) {
		float lowestDisFound = Mathf.Infinity;
		int indexClosest = 99;
		it.transform.position = it.initialPos;

		for (int i = 0; i < mySpawns.Count; i++) {          //Debug.Log(i);
			float dist = Vector2.Distance(mySpawns[i].initialPos, pos);
			if (dist < lowestDisFound) {
				lowestDisFound = dist;
				indexClosest = mySpawns[i].team;
			}
		}
		float distToX = Vector2.Distance(xGO.transform.position, pos);
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

	public void RemoveFromClan(int firstTeam) {
		Dictionary<int, int>.KeyCollection keys = clanDict.Keys;

		Dictionary<int, int> changes = new Dictionary<int, int>();

		foreach (int j in keys) {
			//print(j + " Key");
			int alliesOfJ;
			clanDict.TryGetValue(j, out alliesOfJ);
			List<int> alliesOfJList = IntToList(alliesOfJ);

			if (alliesOfJList.Contains(firstTeam)) {
				alliesOfJList.Remove(firstTeam);
				changes.Add(j, ListToInt(alliesOfJList));

			}

		}

		Dictionary<int, int>.KeyCollection changesKeys = changes.Keys;
		foreach (int k in changesKeys) {
			int value = 0;
			changes.TryGetValue(k, out value);

			clanDict.Remove(k);

			if (value != 0) {
				clanDict.Add(k, value);
			}
		}

		clanDict.Remove(firstTeam);
		UpdateRoundTableVisuals();
	}

	void CreateAClan(int firstTeam, int secondTeam) {

		BindTwo(firstTeam, secondTeam);
		BindTwo(secondTeam, firstTeam);
		CheckAndAddOtherTeams(firstTeam, secondTeam);
		// MoveTeamBoxToLeftOf(mySpawns[MySpawnsIndexFromTeam(firstTeam)], mySpawns[MySpawnsIndexFromTeam(secondTeam)]);
		UpdateRoundTableVisuals();

	}
	void BindTwo(int first, int second) {
		int tempAllies = 0;
		if (clanDict.ContainsKey(first)) {

			clanDict.TryGetValue(first, out tempAllies);

			List<int> content = IntToList(tempAllies);
			if (!AlreadyIsInList(content, second)) {
				tempAllies = tempAllies * 10;
				tempAllies += second;

				clanDict.Remove(first);
				clanDict.Add(first, tempAllies);
			}

		}
		else {
			tempAllies += second;
			clanDict.Add(first, tempAllies);
		}
	}

	public static List<int> IntToList(int input) {
		List<int> returnList = new List<int>();
		string sInput = input.ToString();
		for (int i = 0; i < sInput.Length; i++) {
			int value = int.Parse(sInput[i].ToString());
			returnList.Add(value);
		}

		return returnList;
	}
	public static int ListToInt(List<int> list) {
		int output = 0;
		if (list.Count == 0) {
			return 0;
		}
		foreach (int i in list) {
			output = output * 10;
			output += i;
		}
		return output;
	}

	bool AlreadyIsInList(List<int> list, int value) {
		for (int i = 0; i < list.Count; i++) {
			if (list[i] == value) {
				return true;
			}
		}
		return false;
	}
	void CheckAndAddOtherTeams(int firstTeam, int secondTeam) {
		int firstTeamFriends;
		clanDict.TryGetValue(firstTeam, out firstTeamFriends);
		List<int> firstTeamFriendsList = IntToList(firstTeamFriends);

		int secondTeamFriends;
		clanDict.TryGetValue(secondTeam, out secondTeamFriends);
		List<int> secondTeamFriendsList = IntToList(secondTeamFriends);

		List<int> misses = new List<int>();
		for (int y = 0; y < secondTeamFriendsList.Count; y++) {
			if (!firstTeamFriendsList.Contains(secondTeamFriendsList[y])) {
				misses.Add(secondTeamFriendsList[y]);
			}
		}
		foreach (int miss in misses) {
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
		foreach (int miss in misses) {
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
		List<List<int>> actualClans = new List<List<int>>();

		Dictionary<int, int>.KeyCollection keys = clanDict.Keys;
		foreach (int j in keys) {
			//print(j + " Key");
			int value;
			clanDict.TryGetValue(j, out value);
			List<int> clanJ = IntToList(value);
			clanJ.Add(j);
			clanJ.Sort();


			bool newClan = true;
			foreach (List<int> clan in actualClans) {
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
		List<TeamBox> newList = new List<TeamBox>(mySpawns);

		float angle = 0;


		for (int i = 0; i < actualClans.Count; i++) {
			GameObject bg = Instantiate(clanBG, stRoundTable);
			bgList.Add(bg);
			Image img = bg.GetComponent<Image>();
			img.color = ColourTheTeamButtons.GetColorBasedOnTeam((Cell.enmTeam)(i + 2));
			RectTransform rt = bg.GetComponent<RectTransform>();
			rt.sizeDelta = stRoundTable.sizeDelta;
			rt.SetAsFirstSibling();
			float lastTeamBoxAngle = 0;

			foreach (int j in actualClans[i]) {


				foreach (TeamBox t in mySpawns) {
					if (t.team == j) {
						Debug.Log(t.team);
						t.transform.position = AngleToPos(angle);
						t.myAngle = angle;
						t.AllThingsSet();
						img.fillAmount += 1f / mySpawns.Count;
						lastTeamBoxAngle = Mathf.Rad2Deg * (angle + diffAngle / 2);
						newList.Remove(t);
					}
				}
				angle += diffAngle;
			}
			rt.rotation = Quaternion.Euler(0, 0, -lastTeamBoxAngle);

		}

		foreach (List<int> clan in actualClans) {


		}
		foreach (TeamBox t in newList) {
			t.transform.position = AngleToPos(angle);
			t.myAngle = angle;
			t.AllThingsSet();
			angle += diffAngle;
		}









		//Debug.Log("number of clans: " + ActualClans.Count);
		//      for (int c = 0; c < actualClans.Count; c++) {

		//	LineRenderer r = Instantiate(sampleLineRenderer).GetComponent<LineRenderer>();
		//	myLines.Add(r.gameObject);
		//          Color32 colour = ColourTheTeamButtons.GetColorBasedOnTeam(c);
		//          r.startColor = colour;
		//          r.endColor = colour;
		//          r.positionCount = actualClans[c].Count;
		//	for (int i = 0; i < actualClans[c].Count; i++) {
		//		r.SetPosition(i, (Vector2)Camera.main.ScreenToWorldPoint(mySpawns[MySpawnsIndexFromTeam(actualClans[c][i])].transform.position));
		//	}
		//}


	}

	static Vector2 AngleToPos(float angle) {
		return new Vector2(Mathf.Sin(angle) * (stRoundTable.sizeDelta.x / 2.6f), Mathf.Cos(angle) * (stRoundTable.sizeDelta.x / 2.6f)) * (Screen.width / 1280f) + (Vector2)stRoundTable.position;
	}

	public static int MySpawnsIndexFromTeam(int team) {
		int output = -1;
		for (int i = 0; i < mySpawns.Count; i++) {
			TeamBox spawn = mySpawns[i];
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
	public static int MySpawnsIndexFromAngle(float RawAngle) {
		float angle = RawAngle;
		if (Mathf.Round(angle * 10) / 10 >= Mathf.Round(2 * Mathf.PI * 10) / 10) {
			angle -= 2 * Mathf.PI;
		}
		int output = -1;
		for (int i = 0; i < mySpawns.Count; i++) {
			TeamBox spawn = mySpawns[i];
			if (Mathf.Round(spawn.myAngle * 10) / 10 == Mathf.Round(angle * 10) / 10) {
				output = i;
				break;
			}
		}
		if (output == -1) {
			Debug.LogError("Nothing in mySpawns has angle " + angle);
		}
		return output;
	}
}
