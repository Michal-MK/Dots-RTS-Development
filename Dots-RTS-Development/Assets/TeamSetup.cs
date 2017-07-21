using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSetup : MonoBehaviour {

	public GameObject teamGOPrefab;
	List<TeamBox> mySpawns = new List<TeamBox>();

	LineRenderer lineRenderer;
	// Use this for initialization
	void OnEnable () {
		lineRenderer = gameObject.GetComponent<LineRenderer>();
		for (int i = 0; i < LevelEditorCore.teamList.Count; i++) {
			GameObject newTeamBox = Instantiate(teamGOPrefab, this.transform);
			newTeamBox.GetComponent<Image>().color = ColourTheTeamButtons.GetColorBasedOnTeam((int)LevelEditorCore.teamList[i]);
			mySpawns.Add(newTeamBox.GetComponent<TeamBox>());
			mySpawns[i].myParrent = gameObject.GetComponent<TeamSetup>();
		}
		float diffAngle = (2 * Mathf.PI) / mySpawns.Count;
		float nextAngle = 0;
		for (int i = 0; i < mySpawns.Count; i++) {
			mySpawns[i].transform.position = new Vector3(Mathf.Cos(nextAngle) * 200, Mathf.Sin(nextAngle) * 200, 0) + transform.position ;

			nextAngle += diffAngle;
			mySpawns[i].AllThingsSet();
		}
	}
	public void OnDisable() {
		for (int i = 0; i < mySpawns.Count; i++) {
			Destroy(mySpawns[i]);
		}
		mySpawns.Clear();
		
	}

	// Update is called once per frame
	public void TeamBoxPosChange (Vector2 pos) {
		float lowestDisFound = Mathf.Infinity;
		int indexClosest = 99;
		int indexMe = 99;
		for (int i = 0; i < mySpawns.Count; i++) {
			//Debug.Log(i);
			float dist = Vector2.Distance(mySpawns[i].gameObject.transform.position, pos);
			if (dist == 0) {
				indexMe = i;
				continue;
			}
			if (dist < lowestDisFound){
				lowestDisFound = dist;
				indexClosest = i;
			}
		}
		lineRenderer.SetPosition(0, (Vector2)Camera.main.ScreenToWorldPoint(transform.position));
		lineRenderer.SetPosition(1, (Vector2)Camera.main.ScreenToWorldPoint(mySpawns[indexMe].initialPos));
		lineRenderer.SetPosition(2, (Vector2)Camera.main.ScreenToWorldPoint(mySpawns[indexClosest].initialPos));
		mySpawns[indexMe].gameObject.transform.position = mySpawns[indexMe].initialPos;
	}
}
