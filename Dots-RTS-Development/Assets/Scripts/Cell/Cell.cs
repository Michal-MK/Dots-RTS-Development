using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Cell {

	public MonoBehaviour coroutineRunner;
	public bool isRegenerating;
	public bool isDecaying;

	public event EventHandler<int> OnElementGenerated;
	public event EventHandler<int> OnElementDecayed;


	public List<Upgrades> appliedDebuffs = new List<Upgrades>();

	public Cell(MonoBehaviour coroutineRunner) {
		this.coroutineRunner = coroutineRunner;
	}

	public IEnumerator GenerateElements() {
		isRegenerating = true;
		while (isRegenerating) {
			yield return new WaitForSeconds(regenPeriod);
			if (elementCount < maxElements) {
				elementCount++;
				OnElementGenerated?.Invoke(this, elementCount);
			}
		}
	}

	public void Decay(float decayRate, GameCell superClass) {
		if (isDecaying) return;

		isDecaying = true;
		coroutineRunner.StartCoroutine(DecayElements(decayRate, superClass));
	}

	private IEnumerator DecayElements(float decayRate, GameCell superClass) {
		superClass.StopCoroutine(superClass.generateCoroutine);
		while (isDecaying) {
			yield return new WaitForSeconds(decayRate);
			elementCount--;
			OnElementDecayed?.Invoke(this, elementCount);
			if (maxElements - elementCount > maxElements * 0.5f) {
				decayRate *= 0.5f;
			}
			if (elementCount <= maxElements) {
				isDecaying = false;
				isRegenerating = false;
			}
		}
	}

	public IEnumerator DoT(float timeBetweenTicks, int totalDamageInflicted) {
		appliedDebuffs.Add(Upgrades.ATK_DOT);
		for (int i = 0; i < totalDamageInflicted; i++) {
			yield return new WaitForSeconds(timeBetweenTicks);
			if (elementCount >= 1) {
				elementCount--;
				OnElementDecayed?.Invoke(this, elementCount);
			}
		}
		appliedDebuffs.Remove(Upgrades.ATK_DOT);
	}

	/// <summary>
	/// The elements that are available in selected cell.
	/// </summary>
	public int elementCount;

	/// <summary>
	/// How fast will this cell generate new elements.
	/// </summary>
	public float regenPeriod;

	/// <summary>
	/// The maximum amount of elements this cell can hold.
	/// </summary>
	public int maxElements;

	/// <summary>
	/// Team this cell belongs to.
	/// </summary>
	public Team team;

	/// <summary>
	/// The radius of the cell
	/// </summary>
	public float cellRadius;

	/// <summary>
	/// The speed of elements spawned by this cell
	/// </summary>
	public float elementSpeed = 5;

	/// <summary>
	/// Cell's position in the world
	/// </summary>
	[HideInInspector]
	public Vector3 cellPosition;

	/// <summary>
	/// Clip to play when an element spawns
	/// </summary>
	public AudioClip elementSpawn;

	/// <summary>
	/// Clip to play when an de-spawns
	/// </summary>
	public AudioClip elementAttack;
}
