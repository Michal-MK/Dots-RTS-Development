using UnityEngine;

public class Element : MonoBehaviour {

	public Team team;
	public Upgrades[] debuffs;
	public GameCell attacker;
	public GameCell target;

	protected bool reflected = false;
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
		return new Vector3(transform.position.x + x * attacker.Cell.CellRadius * 0.5f, transform.position.y + y * attacker.Cell.CellRadius * 0.5f);
	}
	public void ExecuteAttack() {
		Upgrades[] infection = new Upgrades[8] {
			Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE
		};

		#region Deterimne type of attack (buff)
		for (int i = 0; i < debuffs.Length; i++) {
			switch (debuffs[i]) {
				case Upgrades.ATK_DOUBLE_DAMAGE: {
					damage = damage * 2;
					break;
				}
				case Upgrades.ATK_SLOW_REGENERATION: {
					//damage = 1;
					infection[i] = debuffs[i];
					break;
				}
				case Upgrades.ATK_CRITICAL_CHANCE: {
					infection[i] = debuffs[i];
					break;
				}
				case Upgrades.ATK_DOT: {
					//damage = 1;
					infection[i] = debuffs[i];
					break;
				}
			}
		}
		#endregion
		target.DamageCell(this, damage, infection);
	}

	public void Refelcted() {
		print("Reflected");
		if (!reflected) {
			GameCell temp = attacker;
			attacker = target;
			target = temp;
			team = attacker.Cell.CellTeam;
			//StartCoroutine(FlagAsReflectedOnLeave);
			reflected = true;
		}
	}
	//private System.Collections.IEnumerator FlagAsReflectedOnLeave() {

	//}

}