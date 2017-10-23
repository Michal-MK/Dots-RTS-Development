using UnityEngine;
using System.Collections;

public class AI_Data_Holder {

	private Enemy_AI _ai;
	private CellBehaviour senderCell;
	private bool isModified = false;

	private int[] cellIndexes = new int[4] { -1, -1, -1, -1 };

	//Constructor
	public AI_Data_Holder(Enemy_AI AI, CellBehaviour cell) {
		_ai = AI;
		senderCell = cell;

		for (int i = 0; i < AI._aiCells.Count; i++) {
			if (AI._aiCells[i] == cell) {
				cellIndexes[0] = i;
			}
		}
		for (int i = 0; i < AI._targets.Count; i++) {
			if (AI._targets[i] == cell) {
				cellIndexes[1] = i;
			}
		}
		for (int i = 0; i < AI._allies.Count; i++) {
			if (AI._allies[i] == cell) {
				cellIndexes[2] = i;
			}
		}
	}

	public AI_Data_Holder(Player player, CellBehaviour cell) {
		_ai = null;
		senderCell = cell;

		for (int i = 0; i < player.playerCells.Count; i++) {
			if (player.playerCells[i] == cell) {
				cellIndexes[0] = i;
			}
		}
	}

	public static AI_Data_Holder TransformForAlly(AI_Data_Holder data, Enemy_AI ally) {

		data._ai = ally;
		data.isModified = true;

		data.cellIndexes[2] = data.cellIndexes[0];
		data.cellIndexes[0] = -1;

		return data;
	}

	public static AI_Data_Holder TransformForTarget(AI_Data_Holder data, Enemy_AI target) {
		data._ai = target;
		data.isModified = true;

		data.cellIndexes[1] = data.cellIndexes[0];
		data.cellIndexes[0] = -1;

		return data;
	}

	/// <summary>
	/// The AI for which this configuration is valid
	/// </summary>
	public Enemy_AI getAI {
		get { return _ai; }
	}

	/// <summary>
	/// Get the cell that triggered creation of this script
	/// </summary>
	public CellBehaviour getSender {
		get { return senderCell; }
	}

	/// <summary>
	/// Instances in this AI --> [0] = AI, [1] = target, [2] = ally, [3] = neutral
	/// </summary>
	public int[] getCellIndexes {
		get { return cellIndexes; }
	}

	/// <summary>
	/// Return true if this script was modified for an ally or a target
	/// </summary>
	public bool isModifiedForOther {
		get { return isModified; }
	}
}
