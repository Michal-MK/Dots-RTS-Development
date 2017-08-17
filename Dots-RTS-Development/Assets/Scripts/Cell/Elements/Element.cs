using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour {

	public Cell.enmTeam team;
	public Upgrade.Upgrades debuff;
	public CellBehaviour attacker;
	public CellBehaviour target;

	private int damage = 1;
	public float eSpeed = 10;
    public float RandomTimeOffset;

	private void Start() {
        RandomTimeOffset = Random.Range(0, 50);
		attacker.UpdateCellInfo();
		debuff = attacker.providedDebuff;
		transform.position = ElementSpawnPoint();
	}

	private Vector3 ElementSpawnPoint() {
		float angle = Random.Range(0, 2 * Mathf.PI);
		float x = Mathf.Sin(angle);
		float y = Mathf.Cos(angle);
		return new Vector3(transform.position.x + x * attacker.cellRadius * 0.5f, transform.position.y + y * attacker.cellRadius * 0.5f);
	}
	public void ExecuteAttack() {

		#region Deterimne type of attack (buff)
		switch (debuff) {
			case Upgrade.Upgrades.DOUBLE_DAMAGE: {
				damage = 2;
				break;
			}
			case Upgrade.Upgrades.SLOW_REGENERATION: {
				damage = 1;
				target.DamageCell(team, damage, debuff);
				return;
			}
			case Upgrade.Upgrades.CRITICAL_CHANCE: {
				if (Random.Range(0f, 1f) > 0.5f) {
					print("CRitical!!");
					damage = 2;
				}
				else {
					damage = 1;
				}
				break;
			}
			case Upgrade.Upgrades.DOT: {
				damage = 1;
				target.DamageCell(team, damage, debuff);
				return;
			}
		}
		#endregion

		target.DamageCell(team, damage);
	}
}