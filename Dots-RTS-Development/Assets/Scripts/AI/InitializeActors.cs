using System;
using System.Collections.Generic;
using UnityEngine;

public class InitializeActors {

	public GameObject playerData;

	public List<EnemyAI> AIs = new List<EnemyAI>();

	private PlayManager playManager;

	public void StartAiInitialization(Dictionary<Team, AIHolder> clanDict, Dictionary<Team, float> difficultyDict, PlayManager instance) {
		playManager = instance;
		//Make an ai for every team contained in the clandict
		Dictionary<Team, AIHolder>.KeyCollection teams = clanDict.Keys;
		foreach (Team team in teams) {
			if ((int)team >= 2) {
				float diff;
				if (difficultyDict.TryGetValue(team, out diff) == false) {
					SetAis(team, 2);
				}
				else {
					SetAis(team, diff);
				}
			}
		}

		List<IAlly> InterfaceList = new List<IAlly>();
		foreach (EnemyAI ai in AIs) {
			InterfaceList.Add(ai);
		}
		InterfaceList.Add(playManager.Player);


		foreach (IAlly iAlly in InterfaceList) {
			AIHolder temp;

			if (clanDict.TryGetValue(iAlly.Team, out temp) == false) {
				Debug.LogError("Not all IAlly|s are in the dictionary");
			}
			List<Team> allies = temp.allies;
			List<IAlly> alliesI = new List<IAlly>();
			foreach (Team team in allies) {
				foreach (IAlly t in InterfaceList) {
					if (t.Team == team) {
						alliesI.Add(t);
					}
				}
			}
			List<Team> targets = temp.targets;
			List<IAlly> targetsI = new List<IAlly>();
			foreach (Team team in targets) {
				foreach (IAlly t in InterfaceList) {
					if (t.Team == team) {
						targetsI.Add(t);
					}
				}
			}

			foreach (IAlly ally in alliesI) {
				iAlly.AddAlly(ally);
			}
			foreach (IAlly targ in targetsI) {
				iAlly.AddTarget(targ);
			}
		}

		foreach (EnemyAI ai in AIs) {
			if (ai != null) {
				ai.FindRelationWithCells();
			}
		}
	}

	public void SetAis(Team team, float decisionSpeed) {
		GameObject aiHolder = new GameObject("AI code " + (int)team + " " + team);
		//Select AI preset according to the enemy team

		AIBehaviour ai = aiHolder.AddComponent<AIBehaviour>();
		ai.decisionSpeed = decisionSpeed;
		ai.playManager = playManager;
		ai.team = team;
		ai.isActive = true;
		AIs.Add(ai);
		ai.Player = playManager.Player;
	}


	public EnemyAI GetAI(Team team) {
		foreach (EnemyAI ai in AIs) {
			if (ai.Team == team) {
				return ai;
			}
		}
		return null; //Safe
	}
}

