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

	public void B() {
		print(GameObject.Find("Cell NEUTRAL").GetComponent<UM_Editor>().upgrades.Length);
		GameObject.Find("Cell NEUTRAL").GetComponent<UM_Editor>().upgrades[8] = Upgrades.ATK_DOT;
	}

	public void C() {
		print("UI");
		//foreach (Upgrades u in UpgradeSlot_UI.UpgradeInstances) {
		//	print(u);
		//}
		print("---------------------------------------------------");
		print("CELL");
		foreach (Upgrades u in GameObject.Find("Cell NEUTRAL").GetComponent<UM_Editor>().upgrades) {
			print(u);
		}
		print("---------------------------------------------------");
		print("CUSTOM A");
		foreach (Upgrades u in a) {
			print(u);
		}
		print("---------------------------------------------------");
		print("CUSTOM B");
		foreach (Upgrades u in b) {
			print(u);
		}
	}
	public void D() {
		a[0] = Upgrades.ATK_DOT;
	}

	public Upgrades[] b = new Upgrades[8] {
		Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,
	};

	private static Upgrades[] a = new Upgrades[8] {
		Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,
	};
}
