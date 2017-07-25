﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSetup : MonoBehaviour {

	public GameObject teamGOPrefab;
	public GameObject xGO;
	List<TeamBox> mySpawns = new List<TeamBox>();
	public static Dictionary<int, int> clanDict = new Dictionary<int, int>();
	//LineRenderer lineRenderer;
	public RectTransform roundTable;
	public GameObject sampleLineRenderer;
	List<GameObject> myLines = new List<GameObject>();

	// Use this for initialization
	void OnEnable() {
		//lineRenderer = gameObject.GetComponent<LineRenderer>();
		for (int i = 0; i < LevelEditorCore.teamList.Count; i++) {
			GameObject newTeamBox = Instantiate(teamGOPrefab, this.transform);
			newTeamBox.GetComponent<Image>().color = ColourTheTeamButtons.GetColorBasedOnTeam((int)LevelEditorCore.teamList[i]);
			mySpawns.Add(newTeamBox.GetComponent<TeamBox>());
			mySpawns[i].team = (int)LevelEditorCore.teamList[i];
			mySpawns[i].myParrent = gameObject.GetComponent<TeamSetup>();
			mySpawns[i].panel = roundTable;
			mySpawns[i].r = roundTable.sizeDelta.x / 2;
		}
		float diffAngle = (2 * Mathf.PI) / mySpawns.Count;
		float nextAngle = 0;
		for (int i = 0; i < mySpawns.Count; i++) {
			mySpawns[i].transform.position = new Vector3(Mathf.Cos(nextAngle) * 200, Mathf.Sin(nextAngle) * 200, 0) + roundTable.position;

			nextAngle += diffAngle;
			mySpawns[i].AllThingsSet();
		}
		MakeLines();
	}
	public void OnDisable() {
		for (int i = 0; i < mySpawns.Count; i++) {
			Destroy(mySpawns[i].gameObject);
		}
		mySpawns.Clear();
		DestroyAllInList(myLines);
	}

	// Update is called once per frame
	public void TeamBoxPosChange(Vector2 pos, TeamBox it) {
		float lowestDisFound = Mathf.Infinity;
		int indexClosest = 99;
		int indexMe = it.team;
		for (int i = 0; i < mySpawns.Count; i++) {
			//Debug.Log(i);
			float dist = Vector2.Distance(mySpawns[i].gameObject.transform.position, pos);
			if (mySpawns[i] == it) {
				mySpawns[i].gameObject.transform.position = mySpawns[i].initialPos;
				continue;
			}
			if (dist < lowestDisFound) {
				lowestDisFound = dist;
				indexClosest = mySpawns[i].team;
			}
		}
		float distToX = Vector2.Distance(xGO.transform.position, pos);
		if (distToX < lowestDisFound) {
			lowestDisFound = distToX;
			indexClosest = -1;
		}
		if (Vector2.Distance(pos, it.initialPos) < lowestDisFound) {
			return;
		}

		if (indexClosest == -1) {
			RemoveFromClan(indexMe);
		}
		else {
			CreateAClan(indexMe, indexClosest);
		}

	}

	void RemoveFromClan(int firstTeam) {
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


		MakeLines();
		print("Keys ===============================");
		keys = clanDict.Keys;
		foreach (int j in keys) {
			//print(j + " Key");
			int debug;
			clanDict.TryGetValue(j, out debug);
			print(j + " has these allies: " + debug);
		}
	}

	void CreateAClan(int firstTeam, int secondTeam) {

		print("First team is " + firstTeam + " second team is " + secondTeam);

		int tempAllies = 0;

		if (clanDict.ContainsKey(firstTeam)) {

			clanDict.TryGetValue(firstTeam, out tempAllies);

			List<int> content = IntToList(tempAllies);
			if (AlreadyIsInList(content, secondTeam)) {

			}
			else {
				tempAllies = tempAllies * 10;
				tempAllies += secondTeam;

				clanDict.Remove(firstTeam);
				clanDict.Add(firstTeam, tempAllies);
			}
			tempAllies = 0;
		}
		else {
			tempAllies += secondTeam;

			clanDict.Add(firstTeam, tempAllies);
			tempAllies = 0;
		}

		if (clanDict.ContainsKey(secondTeam)) {
			clanDict.TryGetValue(secondTeam, out tempAllies);

			List<int> content = IntToList(tempAllies);
			if (AlreadyIsInList(content, firstTeam)) {

			}
			else {
				tempAllies = tempAllies * 10;
				tempAllies += firstTeam;

				clanDict.Remove(secondTeam);
				clanDict.Add(secondTeam, tempAllies);
			}
			tempAllies = 0;
		}
		else {
			tempAllies += firstTeam;
			clanDict.Add(secondTeam, tempAllies);
			tempAllies = 0;
		}

		CheckAndAddOtherTeams(firstTeam, secondTeam);


		MakeLines();
		print("Keys ===============================");
		Dictionary<int, int>.KeyCollection keys = clanDict.Keys;
		foreach (int j in keys) {
			//print(j + " Key");
			int debug;
			clanDict.TryGetValue(j, out debug);
			print(j + " has these allies: " + debug);
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

		List<int> misses2 = new List<int>();
		for (int x = 0; x < firstTeamFriendsList.Count; x++) {
			if (!secondTeamFriendsList.Contains(firstTeamFriendsList[x])) {
				misses2.Add(firstTeamFriendsList[x]);
				//print("Miss is " + firstTeamFriendsList[x]);
			}
		}
		foreach (int miss in misses2) {
			//Debug.Log("like This: " + miss + " " + secondTeam);
			if (miss != secondTeam && miss != firstTeam) {
				CreateAClan(miss, secondTeam);
			}
		}
	}
	void DestroyAllInList(List<GameObject> list) {
		for (int i = 0; i < list.Count; i++) {
			Destroy(list[i]);
		}
	}
	void MakeLines() {

		DestroyAllInList(myLines);
		List<List<int>> ActualClans = new List<List<int>>();

		Dictionary<int, int>.KeyCollection keys = clanDict.Keys;
		foreach (int j in keys) {
			//print(j + " Key");
			int value;
			clanDict.TryGetValue(j, out value);
			List<int> clanJ = IntToList(value);
			clanJ.Add(j);
			clanJ.Sort();

			for (int i = 0; i < clanJ.Count; i++) {
				print(clanJ[i]);
			}
			bool newClan = true;
			foreach (List<int> clan in ActualClans) {
				//print(" one clan with [0] = " + clan[0]);
				if (clanJ.Contains(clan[0])) {
					newClan = false;
					break;
				}

			}
			if (newClan) {
				ActualClans.Add(clanJ);
			}

		}
		Debug.Log("number of clans: " + ActualClans.Count);
		for (int c = 0; c < ActualClans.Count; c++) {

			LineRenderer r = Instantiate(sampleLineRenderer).GetComponent<LineRenderer>();
			myLines.Add(r.gameObject);
			r.startColor = ColourTheTeamButtons.GetColorBasedOnTeam(c);
			r.endColor = ColourTheTeamButtons.GetColorBasedOnTeam(c);
			r.positionCount = ActualClans[c].Count;
			for (int i = 0; i < ActualClans[c].Count; i++) {
				r.SetPosition(i, (Vector2)Camera.main.ScreenToWorldPoint(mySpawns[MySpawnsIndexFromTeam(ActualClans[c][i])].transform.position));
			}
		}


	}
	int MySpawnsIndexFromTeam(int team) {
		int output = -1;
		for (int i = 0; i <	mySpawns.Count; i++) {
			TeamBox spawn = mySpawns[i];
			if (spawn.team == team) {
				output = i;
			}
		}
		if (output == -1) {
			Debug.LogError("Nothing in mySpawns falls under team " + team);
		}
		return output;
	}
}