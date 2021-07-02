using UnityEngine;

public class Element : MonoBehaviour {

	public Team team;
	public Upgrades[] debuffs;
	public GameCell attacker;
	public GameCell target;
	public float speed = 10;

	protected bool reflected;
	protected int damage = 1;
	protected float randomTimeOffset;

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
			Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE
		};

		for (int i = 0; i < debuffs.Length; i++) {
			switch (debuffs[i]) {
				case Upgrades.ATK_DOUBLE_DAMAGE: {
					damage *= 2;
					break;
				}
				case Upgrades.ATK_SLOW_REGENERATION: {
					infection[i] = debuffs[i];
					break;
				}
				case Upgrades.ATK_CRITICAL_CHANCE: {
					infection[i] = debuffs[i];
					break;
				}
				case Upgrades.ATK_DOT: {
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
