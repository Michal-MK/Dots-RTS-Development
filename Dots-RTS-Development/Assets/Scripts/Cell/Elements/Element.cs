using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour {

	public Cell.enmTeam team;
	public enmDebuffs debuff;
	public CellBehaviour attacker;
	public CellBehaviour target;

	private int damage = 1;

	public enum enmDebuffs {
		NONE,
		DOUBLE_DAMAGE,
		CRITICAL_CHANCE,
		SLOW_REGENERATION,
		DOT,
	}

	private void Start() {
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
			case enmDebuffs.DOUBLE_DAMAGE: {
				damage = 2;
				break;
			}
			case enmDebuffs.SLOW_REGENERATION: {
				damage = 1;
				print("Handeled by cell");
				target.DamageCell(team, damage, debuff);
				return;
			}
			case enmDebuffs.CRITICAL_CHANCE: {
				if (Random.Range(0f, 1f) > 0.5f) {
					print("CRitical!!");
					damage = 2;
				}
				else {
					damage = 1;
				}
				break;
			}
			case enmDebuffs.DOT: {
				damage = 1;
				print("Handeled by cell");
				target.DamageCell(team, damage, debuff);
				return;
			}
		}
		#endregion

		target.DamageCell(team, damage);
	}
}