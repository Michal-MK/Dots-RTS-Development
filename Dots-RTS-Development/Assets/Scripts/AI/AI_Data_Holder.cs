using UnityEngine;
using System.Collections;

public class AI_Data_Holder {

	private Enemy_AI _ai;
	private CellBehaviour _senderCell;
	private RelationToAI _relation;
	private bool isModified = false;
	public enum RelationToAI {
		ALLY,
		TARGET,
		MYSELF,
		PLAYER
	}
	//private int[] cellIndexes = new int[3] { -1, -1, -1 };

	//Constructor
	public AI_Data_Holder(Enemy_AI AI, CellBehaviour cell) {
		_ai = AI;
		_senderCell = cell;
		_relation = RelationToAI.MYSELF;
	}

	public AI_Data_Holder(Player player, CellBehaviour cell) {
		_ai = null;
		_senderCell = cell;
		_relation = RelationToAI.PLAYER;
	}

	public static AI_Data_Holder TransformForAlly(AI_Data_Holder data, Enemy_AI ally) {

		data._ai = ally;
		data.isModified = true;

		data._relation = RelationToAI.ALLY;

		return data;
	}

	public static AI_Data_Holder TransformForTarget(AI_Data_Holder data, Enemy_AI target) {
		data._ai = target;
		data.isModified = true;

		data._relation = RelationToAI.TARGET;

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
		get { return _senderCell; }
	}

	/// <summary>
	/// Relation of this AI to other
	/// </summary>
	public RelationToAI getRelation {
		get { return _relation; }
	}

	/// <summary>
	/// Return true if this script was modified for an ally or a target
	/// </summary>
	public bool isModifiedForOther {
		get { return isModified; }
	}
}
