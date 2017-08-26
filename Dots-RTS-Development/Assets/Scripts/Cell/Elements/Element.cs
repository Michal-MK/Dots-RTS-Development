using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour {

	public Cell.enmTeam team;
	public Upgrade.Upgrades[] debuffs;
	public CellBehaviour attacker;
	public CellBehaviour target;

	private int damage = 1;
	public float eSpeed = 10;
    public float RandomTimeOffset;

	private void Start() {
        RandomTimeOffset = Random.Range(0, 50);
		attacker.UpdateCellInfo();
		debuffs = attacker.uManager.upgrades;
		transform.position = ElementSpawnPoint();
	}

	private Vector3 ElementSpawnPoint() {
		float angle = Random.Range(0, 2 * Mathf.PI);
		float x = Mathf.Sin(angle);
		float y = Mathf.Cos(angle);
		return new Vector3(transform.position.x + x * attacker.cellRadius * 0.5f, transform.position.y + y * attacker.cellRadius * 0.5f);
	}
	public void ExecuteAttack() {
		Upgrade.Upgrades[] infection = new Upgrade.Upgrades[8] {
			Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE
		};

		#region Deterimne type of attack (buff)
		for (int i = 0; i < debuffs.Length; i++) {
			switch (debuffs[i]) {
				case Upgrade.Upgrades.DOUBLE_DAMAGE: {
					print("Contains Double Damage");
					damage = damage * 2;
					break;
				}
				case Upgrade.Upgrades.SLOW_REGENERATION: {
					//damage = 1;
					infection[i] = debuffs[i];
					break;
				}
				case Upgrade.Upgrades.CRITICAL_CHANCE: {
					infection[i] = debuffs[i];
					break;
				}
				case Upgrade.Upgrades.DOT: {
					print("Contains Dot");
					//damage = 1;
					infection[i] = debuffs[i];
					break;
				}
			}
		}
		#endregion
		target.DamageCell(team, damage, infection);
	}
}