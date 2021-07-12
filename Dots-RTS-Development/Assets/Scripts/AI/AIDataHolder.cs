public class AIDataHolder {

	public enum RelationToAI {
		Ally,
		Target,
		Self,
		Player
	}

	public AIDataHolder(EnemyAI ai, GameCell cell) {
		Sender = cell;
		Relation = RelationToAI.Self;
	}

	public AIDataHolder(GameCell cell) {
		Sender = cell;
		Relation = RelationToAI.Player;
	}

	/// <summary>
	/// When we create a new Holder object, and transform it, the initial AI is changed and the reference is updated to refer to "sender" in relation to initial AI
	/// </summary>
	/// <param name="data">The data to modify</param>
	public static AIDataHolder TransformForAlly(AIDataHolder data) {
		data.Relation = RelationToAI.Ally;

		return data;
	}

	///  <summary>
	/// When we create a new Holder object, and transform it, the initial AI is changed and the reference is updated to refer to "sender" in relation to initial AI
	///  </summary>
	///  <param name="data">The data to modify</param>
	public static AIDataHolder TransformForTarget(AIDataHolder data) {
		data.Relation = RelationToAI.Target;

		return data;
	}

	/// <summary>
	/// Get the cell that triggered creation of this script
	/// </summary>
	public GameCell Sender { get; }

	/// <summary>
	/// Relation of this AI to other
	/// </summary>
	public RelationToAI Relation { get; private set; }
}
