public class AI_Data_Holder {

	public enum RelationToAI {
		ALLY,
		TARGET,
		SELF,
		PLAYER
	}

	public AI_Data_Holder(Enemy_AI AI, GameCell cell) {
		this.AI = AI;
		Sender = cell;
		Relation = RelationToAI.SELF;
	}

	public AI_Data_Holder(GameCell cell) {
		AI = null;
		Sender = cell;
		Relation = RelationToAI.PLAYER;
	}

	/// <summary>
	/// When we create a new Holder object, and transform it, the initial AI is changed and the reference is updated to refer to "sender" in relation to initial AI
	/// </summary>
	/// <param name="data">The data to modify</param>
	/// <param name="target">The Ally we are changing reference to</param>
	public static AI_Data_Holder TransformForAlly(AI_Data_Holder data, Enemy_AI ally) {
		data.AI = ally;
		data.Relation = RelationToAI.ALLY;

		return data;
	}

	/// <summary>
	///When we create a new Holder object, and transform it, the initial AI is changed and the reference is updated to refer to "sender" in relation to initial AI
	/// </summary>
	/// <param name="data">The data to modify</param>
	/// <param name="target">The Target we are changing reference to</param>
	public static AI_Data_Holder TransformForTarget(AI_Data_Holder data, Enemy_AI target) {
		data.AI = target;
		data.Relation = RelationToAI.TARGET;

		return data;
	}

	/// <summary>
	/// The AI for which this configuration is valid
	/// </summary>
	public Enemy_AI AI { get; private set; }

	/// <summary>
	/// Get the cell that triggered creation of this script
	/// </summary>
	public GameCell Sender { get; }

	/// <summary>
	/// Relation of this AI to other
	/// </summary>
	public RelationToAI Relation { get; private set; }
}
