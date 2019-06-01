using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Cell {

	public MonoBehaviour coroutineRunner;
	public bool isRegenerating = false;
	public bool isDecaying = false;

	public event EventHandler<int> OnElementGenerated;
	public event EventHandler<int> OnElementDecayed;


	public List<Upgrades> appliedDebuffs = new List<Upgrades>();

	public Cell(MonoBehaviour coroutineRunner) {
		this.coroutineRunner = coroutineRunner;
	}

	public IEnumerator GenerateElements() {
		isRegenerating = true;
		while (isRegenerating) {
			yield return new WaitForSeconds(RegenPeriod);
			if (ElementCount < MaxElements) {
				ElementCount++;
				OnElementGenerated?.Invoke(this, ElementCount);
			}
		}
	}

	public void Decay(float decayRate, GameCell superClass) {
		if (!isDecaying) {
			isDecaying = true;
			coroutineRunner.StartCoroutine(DecayElements(decayRate, superClass));
		}
	}

	private IEnumerator DecayElements(float decayRate, GameCell superClass) {
		float d = decayRate;
		superClass.StopCoroutine(superClass.generateCoroutine);
		while (isDecaying) {
			yield return new WaitForSeconds(d);
			ElementCount--;
			OnElementDecayed?.Invoke(this, ElementCount);
			if (MaxElements - ElementCount > MaxElements * 0.5f) {
				d *= 0.5f;
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
				OnElementDecayed?.Invoke(this, ElementCount);
			}
		}
		appliedDebuffs.Remove(Upgrades.ATK_DOT);
	}

	/// <summary>
	/// The elements that are available in selected cell.
	/// </summary>
	public int ElementCount;

	/// <summary>
	/// How fast will this cell generate new elements.
	/// </summary>
	public float RegenPeriod;

	/// <summary>
	/// The maximum amount of elements this cell can hold.
	/// </summary>
	public int MaxElements;

	/// <summary>
	/// Team this cell belongs to.
	/// </summary>
	public Team Team;

	/// <summary>
	/// The radius of the cell
	/// </summary>
	public float CellRadius;

	/// <summary>
	/// The speed of elements spawned by this cell
	/// </summary>
	[HideInInspector]
	public float ElementSpeed = 5;

	/// <summary>
	/// Cell's position in the world
	/// </summary>
	[HideInInspector]
	public Vector3 CellPosition;

	/// <summary>
	/// Clip to play when an element spawns
	/// </summary>
	[HideInInspector]
	public AudioClip ElementSpawn;

}
