using UnityEngine;

public class Element : MonoBehaviour {

	public Team team;
	public Upgrades[] debuffs;
	public GameCell attacker;
	public GameCell target;
	public float speed = 10;

	protected bool reflected;
	protected float randomTimeOffset;
	
	private int damage = 1;

	private void Start() {
		randomTimeOffset = Random.Range(0, 50);
		attacker.UpdateCellInfo();
		debuffs = attacker.uManager.InstalledUpgrades;
		transform.position = ElementSpawnPoint();
	}

	private Vector3 ElementSpawnPoint() {
		float angle = Random.Range(0, 2 * Mathf.PI);
		float x = Mathf.Sin(angle);
		float y = Mathf.Cos(angle);
		Vector3 position = transform.position;
		return new Vector3(position.x + x * attacker.Cell.cellRadius * 0.5f, position.y + y * attacker.Cell.cellRadius * 0.5f);
	}

	protected void ExecuteAttack() {
		Upgrades[] infection = {
			Upgrades.None,Upgrades.None,Upgrades.None,Upgrades.None,Upgrades.None,Upgrades.None,Upgrades.None,Upgrades.None
		};

		for (int i = 0; i < debuffs.Length; i++) {
			switch (debuffs[i]) {
				case Upgrades.AtkDoubleDamage: {
					damage *= 2;
					break;
				}
				case Upgrades.AtkSlowRegeneration: {
					infection[i] = debuffs[i];
					break;
				}
				case Upgrades.AtkCriticalChance: {
					infection[i] = debuffs[i];
					break;
				}
				case Upgrades.AtkDot: {
					infection[i] = debuffs[i];
					break;
				}
			}
		}

		target.DamageCell(this, damage, infection);
	}

	public void Reflected() {
		if (reflected) return;
		GameCell temp = attacker;
		attacker = target;
		target = temp;
		team = attacker.Cell.team;
		reflected = true;
	}
}
