using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InitializeActors {
	
	private const int DEFAULT_DECISION_SPEED = 2;

	public List<EnemyAI> AIs { get; } = new List<EnemyAI>();

	private PlayManagerBehaviour playManager;

	public void StartAiInitialization(Dictionary<Team, AIHolder> clanDict, Dictionary<Team, float> difficultyDict, PlayManagerBehaviour instance) {
		playManager = instance;

		// Make an ai for every team contained in the clan dictionary
		foreach (Team team in clanDict.Keys.Where(team => (int)team >= 2)) {
			if (difficultyDict.TryGetValue(team, out float diff) == false) {
				SetAis(team, DEFAULT_DECISION_SPEED);
			}
			else {
				SetAis(team, diff);
			}
		}

		List<IAlly> interfaceList = AIs.Cast<IAlly>().ToList();
		interfaceList.Add(playManager.Player);

		foreach (IAlly iAlly in interfaceList) {

			if (clanDict.TryGetValue(iAlly.Team, out AIHolder temp) == false) {
				Debug.LogError("Not all IAlly's are in the dictionary!");
			}

			List<Team> allies = temp.allies;
			List<IAlly> alliesI = new List<IAlly>();
			foreach (Team team in allies) {
				alliesI.AddRange(interfaceList.Where(t => t.Team == team));
			}

			List<Team> targets = temp.targets;
			List<IAlly> targetsI = new List<IAlly>();
			foreach (Team team in targets) {
				targetsI.AddRange(interfaceList.Where(t => t.Team == team));
			}

			foreach (IAlly ally in alliesI) {
				iAlly.AddAlly(ally);
			}
			foreach (IAlly target in targetsI) {
				iAlly.AddTarget(target);
			}
		}

		foreach (EnemyAI ai in AIs) {
			ai.FindRelationWithCells();
		}
	}

	private void SetAis(Team team, float decisionSpeed) {
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
		return AIs.FirstOrDefault(ai => ai.Team == team);
	}
}
