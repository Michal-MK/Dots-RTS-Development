using UnityEngine;

public class DebugButton : MonoBehaviour {

	public Enemy_AI aiScript;

	public void ListMembers() {
		print("Targets-----------------------------------------");
		for (int i = 0; i < aiScript._targets.Count; i++) {
			print(aiScript._targets[i].gameObject.name);
		}
		print("AI-----------------------------------------");
		for (int i = 0; i < aiScript._aiCells.Count; i++) {
			print(aiScript._aiCells[i].gameObject.name);
		}
		print("Neutrals-----------------------------------------");
		for (int i = 0; i < aiScript._neutrals.Count; i++) {
			print(aiScript._neutrals[i].gameObject.name);
		}
	}
}
