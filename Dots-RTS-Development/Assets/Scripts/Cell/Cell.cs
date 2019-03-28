using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell {
	public MonoBehaviour coroutineRunner;
	public bool isRegenerating = false;
	public bool isDecaying = false;

	public event EventHandler<int> OnElementGenerated;
	public event EventHandler<int> OnElementDecayed;
	public event EventHandler<Team> OnTeamChnged;


	public List<Upgrades> appliedDebuffs = new List<Upgrades>();

	public AudioClip elementSpawn;

	public Cell(MonoBehaviour coroutineRunner) {
		this.coroutineRunner = coroutineRunner;
	}

	//Keeps generateing new elements for the cell
	public IEnumerator GenerateElements() {
		isRegenerating = true;
		while (isRegenerating) {
			yield return new WaitForSeconds(RegenPeriod);
			if (ElementCount < MaxElements) {
				ElementCount++;
				coroutineRunner.SendMessage("UpdateCellInfo", false, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	//Wrapper for Decaying
	public void Decay(float decayRate, GameCell superClass) {
		if (!isDecaying) {
			isDecaying = true;
			coroutineRunner.StartCoroutine(DecayElements(decayRate, superClass));
		}
	}

	//Elements start decaying when they go over the cap(max Elements).
	private IEnumerator DecayElements(float decayRate, GameCell superClass) {
		float d = decayRate;
		superClass.StopCoroutine(superClass.generateCoroutine);
		//print("Stopped");
		while (isDecaying) {
			yield return new WaitForSeconds(d);
			ElementCount--;
			if (MaxElements - ElementCount > MaxElements * 0.5f) {
				d = d * 0.5f;
			}
			if (ElementCount <= MaxElements) {
				isDecaying = false;
				isRegenerating = false;
			}
		}
	}

	public IEnumerator DoT(float timeBetweenTicks, int totalDamageInflicted) {
		appliedDebuffs.Add(Upgrades.ATK_DOT);
		for (int i = 0; i < totalDamageInflicted; i++) {
			yield return new WaitForSeconds(timeBetweenTicks);
			if (ElementCount >= 1) {
				ElementCount--;
			}
		}
		appliedDebuffs.Remove(Upgrades.ATK_DOT);
	}

	/// <summary>
	/// The elements that are available in selected cell.
	/// </summary>
	public int ElementCount { get; set; }

	/// <summary>
	/// How fast will this cell generate new elements.
	/// </summary>
	public float RegenPeriod { get; set; }

	/// <summary>
	/// The maximum amount of elements this cell can hold.
	/// </summary>
	public int MaxElements { get; set; }

	/// <summary>
	/// Team this cell belongs to.
	/// </summary>
	public Team CellTeam { get; set; }

	/// <summary>
	/// The radius of the cell
	/// </summary>
	public float CellRadius { get; set; }

	/// <summary>
	/// The speed of elements spawned by this cell
	/// </summary>
	public float ElementSpeed { get; set; } = 5;

	/// <summary>
	/// Cell's position in the world
	/// </summary>
	public Vector3 CellPosition { get; set; }
}
