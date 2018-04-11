using UnityEngine;

public class DebugButton : MonoBehaviour {

	public Enemy_AI aiScript;

	public void ListMembers() {
		//print("Targets-----------------------------------------");
		//for (int i = 0; i < aiScript._targets.Count; i++) {
		//	print(aiScript._targets[i].gameObject.name);
		//}
		//print("AI-----------------------------------------");
		//for (int i = 0; i < aiScript._aiCells.Count; i++) {
		//	print(aiScript._aiCells[i].gameObject.name);
		//}
		//print("Neutrals-----------------------------------------");
		//for (int i = 0; i < aiScript._neutrals.Count; i++) {
		//	print(aiScript._neutrals[i].gameObject.name);
		//}
	}

	public void ListUpgradesOnUI() {
		foreach (Upgrade.Upgrades u in UpgradeSlot_UI.getAssignedUpgrades) {
			print(u);
		}
	}

	public void A() {
		UpgradeSlot_UI.instances[0] = Upgrade.Upgrades.ATK_CRITICAL_CHANCE;
	}

	public void B() {
		print(GameObject.Find("Cell NEUTRAL").GetComponent<UM_Editor>().upgrades.Length);
		GameObject.Find("Cell NEUTRAL").GetComponent<UM_Editor>().upgrades[8] = Upgrade.Upgrades.ATK_DOT;
	}

	public void C() {
		print("UI");
		foreach (Upgrade.Upgrades u in UpgradeSlot_UI.instances) {
			print(u);
		}
		print("---------------------------------------------------");
		print("CELL");
		foreach (Upgrade.Upgrades u in GameObject.Find("Cell NEUTRAL").GetComponent<UM_Editor>().upgrades) {
			print(u);
		}
		print("---------------------------------------------------");
		print("CUSTOM A");
		foreach (Upgrade.Upgrades u in a) {
			print(u);
		}
		print("---------------------------------------------------");
		print("CUSTOM B");
		foreach (Upgrade.Upgrades u in b) {
			print(u);
		}
	}
	public void D() {
		a[0] = Upgrade.Upgrades.ATK_DOT;
	}

	public Upgrade.Upgrades[] b = new Upgrade.Upgrades[8] {
		Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,
	};

	private static Upgrade.Upgrades[] a = new Upgrade.Upgrades[8] {
		Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,Upgrade.Upgrades.NONE,
	};
}
