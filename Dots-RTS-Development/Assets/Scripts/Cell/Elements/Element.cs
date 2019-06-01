using UnityEngine;

public class Element : MonoBehaviour {

	public Team Team;
	public Upgrades[] Debuffs;
	public GameCell Attacker;
	public GameCell Target;
	public float Speed = 10;

	protected bool reflected = false;
	protected int damage = 1;
    protected float RandomTimeOffset;

	private void Start() {
        RandomTimeOffset = Random.Range(0, 50);
		Attacker.UpdateCellInfo();
		Debuffs = Attacker.uManager.upgrades;
		transform.position = ElementSpawnPoint();
	}

	private Vector3 ElementSpawnPoint() {
		float angle = Random.Range(0, 2 * Mathf.PI);
		float x = Mathf.Sin(angle);
		float y = Mathf.Cos(angle);
		return new Vector3(transform.position.x + x * Attacker.Cell.CellRadius * 0.5f, transform.position.y + y * Attacker.Cell.CellRadius * 0.5f);
	}

	public void ExecuteAttack() {
		Upgrades[] infection = new Upgrades[8] {
			Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE
		};

		#region Deterimne type of attack (buff)
		for (int i = 0; i < Debuffs.Length; i++) {
			switch (Debuffs[i]) {
				case Upgrades.ATK_DOUBLE_DAMAGE: {
					damage = damage * 2;
					break;
				}
				case Upgrades.ATK_SLOW_REGENERATION: {
					//damage = 1;
					infection[i] = Debuffs[i];
					break;
				}
				case Upgrades.ATK_CRITICAL_CHANCE: {
					infection[i] = Debuffs[i];
					break;
				}
				case Upgrades.ATK_DOT: {
					//damage = 1;
					infection[i] = Debuffs[i];
					break;
				}
			}
		}
		#endregion
		Target.DamageCell(this, damage, infection);
	}

	public void Refelcted() {
		if (!reflected) {
			GameCell temp = Attacker;
			Attacker = Target;
			Target = temp;
			Team = Attacker.Cell.Team;
			reflected = true;
		}
	}
}