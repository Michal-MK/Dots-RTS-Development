using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSetup : MonoBehaviour {
	public GameObject teamGOPrefab;
	List<GameObject> mySpawns = new List<GameObject>();
	// Use this for initialization
	void OnEnable () {
		for (int i = 0; i < LevelEditorCore.teamList.Count; i++) {
			GameObject newTeamBox = Instantiate(teamGOPrefab, this.transform.parent);
			newTeamBox.GetComponent<Image>().color = ColourTheTeamButtons.GetColorBasedOnTeam((int)LevelEditorCore.teamList[i]);
			mySpawns.Add(newTeamBox);
			
		}
		float diffAngle = (2 * Mathf.PI) / mySpawns.Count;
		float nextAngle = 0;
		for (int i = 0; i < mySpawns.Count; i++) {
			mySpawns[i].transform.position = new Vector3(Mathf.Cos(nextAngle) * 200, Mathf.Sin(nextAngle) * 200, -3) + transform.position ;

			nextAngle += diffAngle;
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
		int index = 99;
		for (int i = 0; i < mySpawns.Count; i++) {
			float dist = Vector2.Distance(mySpawns[i].transform.position, pos);
			if (dist == 0) {
				continue;
			}
			if (dist < lowestDisFound){
				lowestDisFound = dist;
				index = i;
			}
		}
		mySpawns[index].transform.position = pos;
		
	}
}
